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

    public virtual void LoadQuestion()
    {
        //Back up data
        QuestionCounter.Get.UpdateQuestionNumber();
        currentQuestion = QuestionManager.GetQuestion();
        QuestionManager.nextQuestionIndex++;
        RunQuestion();
    }

    public virtual void RunQuestion()
    {
        if (QuestionManager.nextQuestionIndex == 1)
            questionLozengeAnim.SetTrigger("toggle");

        foreach (PlayerObject po in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.PrepForQuestion, "");

        //Warning beeps and countdown on devices
        GlobalTimeManager.Get.ResetClock(true);
        Invoke("QuestionRunning", 3f);
    }

    public virtual void QuestionRunning()
    {
        foreach (PlayerObject po in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.QuestionPacket, $"{currentQuestion.category}|{currentQuestion.question} <color=yellow>{QuestionManager.GetClueLength(currentQuestion.validAnswers.ToArray())}");

        categoryMesh.text = $"<u>{ currentQuestion.category }</u>";
        questionMesh.text = currentQuestion.question + " <color=yellow>" + QuestionManager.GetClueLength(currentQuestion.validAnswers.ToArray());
        Invoke("OnQuestionEnded", 16f);
    }

    public virtual void OnQuestionEnded()
    {
        GlobalTimeManager.Get.StopClock();
        questionMesh.text = currentQuestion.validAnswers[0];

        if(GameplayManager.Get.currentRound != GameplayManager.RoundType.MainGame || (GameplayManager.Get.currentRound != GameplayManager.RoundType.MainGame && QuestionManager.nextQuestionIndex > 5))
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
                x.SetColumnColor(Column.MaterialChoice.ProfileTexture, Column.MaterialChoice.CorrectColor);
        }
        foreach(PlayerObject pl in PlayerManager.Get.players.Where(x => !x.wasCorrect))
        {
            pl.strap.SetCorrectOrIncorrectColor(false);
            pl.cloneStrap.SetCorrectOrIncorrectColor(false);

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
            HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.AnswerPacket, currentQuestion.validAnswers[0] + "|" + po.wasCorrect.ToString().ToUpperInvariant());
            if(GameplayManager.Get.currentRound == GameplayManager.RoundType.MainGame)
                HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.DataFields, $"{po.lastFive.Count(x => x).ToString() + "/" + (QuestionManager.nextQuestionIndex > 4 ? "5" : QuestionManager.nextQuestionIndex.ToString())}|{po.points}|{po.hotseatLives.ToString()}");            
            else
                HostManager.Get.SendPayloadToClient(po, EventLibrary.HostEventType.DataFields, $"<color=#9f9f9f15>--|{po.points}|{po.hotseatLives.ToString()}");
        }

        LeaderboardManager.Get.ReorderBoard();
        HostManager.Get.UpdateClientLeaderboards();
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
