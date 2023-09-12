using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainGame : Round
{
    public override void UpdateColumnsPostQuestion()
    {
        base.UpdateColumnsPostQuestion();

        if (ColumnManager.Get.columns.Where(x => x.containedPlayer != null && x.containedPlayer.lastFive.Count(x => x) == 5).Count() != 0)
            StartCoroutine(ExtraLives());
        else if (ColumnManager.Get.columns.Any(x => !x.occupied) && QuestionManager.nextQuestionIndex > 4 && PlayerManager.Get.players.Count(x => !x.inHotseat && !x.justEliminated) != 0)
            StartCoroutine(GettingPlayers());
        else
            ResetForNextQuestion();
    }

    IEnumerator ExtraLives()
    {
        bool animationRequired = false;

        foreach (Column c in ColumnManager.Get.columns.Where(x => x.containedPlayer != null && x.containedPlayer.lastFive.Count(x => x) == 5))
        {
            if (c.containedPlayer.hotseatLives != 3 || animationRequired)
                animationRequired = true;
            c.ElevateBall(true);
        }

        if(animationRequired)
            yield return new WaitForSeconds(5f);

        if (ColumnManager.Get.columns.Any(x => !x.occupied) && QuestionManager.nextQuestionIndex > 4 && PlayerManager.Get.players.Count(x => !x.inHotseat) != 0)
            StartCoroutine(GettingPlayers());
        else
            ResetForNextQuestion();
    }

    IEnumerator GettingPlayers()
    {
        ColumnManager.Get.BringInPlayers();

        //Wait until all of the columns are occupied, then a final five seconds after the last one has been filled
        yield return new WaitUntil(() => ColumnManager.Get.columns.All(x => x.occupied) || PlayerManager.Get.players.Count(x => !x.inHotseat) == 0);
        yield return new WaitForSeconds(5f);
        ResetForNextQuestion();
    }

    public override void ResetForNextQuestion()
    {
        if (QuestionManager.nextQuestionIndex == QuestionManager.GetRoundQCount())
            StartCoroutine(EndOfMainGameReset());
        else
        {
            ResetPlayerVariables();
            LoadQuestion();
        }
    }

    IEnumerator EndOfMainGameReset()
    {
        questionLozengeAnim.SetTrigger("toggle");
        QuestionCounter.Get.UpdateQuestionNumber();

        for (int i = 0; i < 2; i++)
        {
            foreach (PlayerObject p in PlayerManager.Get.players)
            {
                if (i == 0)
                    p.lastFive = new bool[5];

                HostManager.Get.SendPayloadToClient(p, EventLibrary.HostEventType.UpdateScore, $"Points: {p.points}");
            }
            if (ColumnManager.Get.columns.Any(x => x.occupied && x.containedPlayer.hotseatLives < 3))
            {
                foreach (Column c in ColumnManager.Get.columns.Where(x => x.occupied && x.containedPlayer.hotseatLives < 3))
                    c.ElevateBall(false);
                yield return new WaitForSeconds(5f);
            }
            foreach (Column c in ColumnManager.Get.columns)
                c.SetConsecutiveLights(0);
        }
        ResetPlayerVariables();
        QuestionManager.nextQuestionIndex = 0;
        GameplayManager.Get.currentStage = GameplayManager.GameplayStage.RevealInstructions;
        InstructionsManager.Get.Discs();
    }
}
