using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Round : MonoBehaviour
{
    public Question currentQuestion = null;
    public Animator questionLozengeAnim;
    public TextMeshProUGUI categoryMesh;
    public TextMeshProUGUI questionMesh;

    public TextMeshPro qCountMesh;

    public static bool entryDisabled;

    public virtual void LoadQuestion()
    {
        //Back up data
        entryDisabled = true;
        QuestionCounter.Get.UpdateQuestionNumber();
        currentQuestion = QuestionManager.GetQuestion();
        foreach (PlayerObject p in PlayerManager.Get.players)
            p.justEliminated = false;
        QuestionManager.nextQuestionIndex++;
        RunQuestion();
    }

    public virtual void RunQuestion()
    {
        if (QuestionManager.nextQuestionIndex == 1)
            questionLozengeAnim.SetTrigger("toggle");

        foreach (PlayerObject po in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.Information, "Get ready for the question...");

        //Warning beeps and countdown on devices
        GlobalTimeManager.Get.ResetClock(true);
        StartCoroutine(ClockTickCountdown());
        Invoke("QuestionRunning", 3f);
    }

    public virtual IEnumerator ClockTickCountdown()
    {
        for(int i = 0; i < 4; i++)
        {
            AudioManager.Get.Play(AudioManager.OneShotClip.ClockTick);
            yield return new WaitForSeconds(1f);
        }
    }

    public virtual void QuestionRunning()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.GoToFinal);
        foreach (PlayerObject po in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.SimpleQuestion, $"<size=75%><u>{currentQuestion.category} ({QuestionManager.nextQuestionIndex}/{QuestionManager.GetRoundQCount()})</u></size>\n{currentQuestion.question} <color=yellow>{QuestionManager.GetClueLength(currentQuestion.validAnswers.ToArray())}|14");

        categoryMesh.text = $"<u>{ currentQuestion.category }</u>";
        questionMesh.text = currentQuestion.question + " <color=yellow>" + QuestionManager.GetClueLength(currentQuestion.validAnswers.ToArray());
        Invoke("OnQuestionEnded", 16f);
        entryDisabled = false;
    }

    public virtual void OnQuestionEnded()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.ChuteLoseLife);
        GlobalTimeManager.Get.StopClock();
        questionMesh.text = currentQuestion.validAnswers[0];

        foreach (Column c in ColumnManager.Get.columns)
            c.SetColumnColor(Column.MaterialChoice.ProfileTexture, Column.MaterialChoice.UnlitBlack);

        foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
        {
            pl.points++;
            pl.lastFive[(QuestionManager.nextQuestionIndex - 1) % 5] = true;
            pl.strap.SetCorrectOrIncorrectColor(true);
            pl.cloneStrap.SetCorrectOrIncorrectColor(true);

            var x = ColumnManager.Get.columns.FirstOrDefault(x => x.containedPlayer == pl);
            if (x != null)
            {
                x.SetColumnColor(Column.MaterialChoice.ProfileTexture, Column.MaterialChoice.CorrectColor);
                if(GameplayManager.Get.currentRound == GameplayManager.RoundType.MainGame)
                    x.SetConsecutiveLights(pl.lastFive.Count(x => x));
            }                
        }
        foreach(PlayerObject pl in PlayerManager.Get.players.Where(x => !x.wasCorrect))
        {
            if(pl.strap != null)
            {
                pl.strap.SetCorrectOrIncorrectColor(false);
                pl.cloneStrap.SetCorrectOrIncorrectColor(false);
            }

            if (!pl.inHotseat)
                pl.lastFive[(QuestionManager.nextQuestionIndex - 1) % 5] = false;
            else
            {
                pl.lastFive = new bool[5];
                pl.hotseatLives--;
                var x = ColumnManager.Get.columns.FirstOrDefault(x => x.containedPlayer == pl);
                if (x != null)
                {
                    x.SetColumnColor(Column.MaterialChoice.ProfileTexture, Column.MaterialChoice.IncorrectColor);
                    x.OpenNextDoor();
                }
            }
        }

        foreach (PlayerObject po in PlayerManager.Get.players)
        {
            HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.SingleAndMultiResult, currentQuestion.validAnswers[0] + "|" + (po.wasCorrect ? "Correct" : "Incorrect"));
            HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.UpdateScore, $"Points: {po.points}");
        }

        LeaderboardManager.Get.ReorderBoard();
        Invoke("UpdateColumnsPostQuestion", 5f);
    }

    public virtual void UpdateColumnsPostQuestion()
    {
        categoryMesh.text = "";
        questionMesh.text = "";
    }    


    public virtual void ResetForNextQuestion()
    {
        
    }

    public virtual void ResetPlayerVariables()
    {
        categoryMesh.text = "";
        questionMesh.text = "";
        foreach (PlayerObject po in PlayerManager.Get.players)
        {
            po.submission = "";
            po.wasCorrect = false;
            po.strap.SetBackgroundColor(po.inHotseat);
            po.cloneStrap.SetBackgroundColor(po.inHotseat);
        }
        if ((GameplayManager.Get.currentRound == GameplayManager.RoundType.MainGame && QuestionManager.nextQuestionIndex > 4) || GameplayManager.Get.currentRound == GameplayManager.RoundType.FinalGame)
            foreach (Column col in ColumnManager.Get.columns.Where(x => x.occupied))
                col.SetColumnColor(Column.MaterialChoice.ProfileTexture, Column.MaterialChoice.GlowingColor);
    }
}
