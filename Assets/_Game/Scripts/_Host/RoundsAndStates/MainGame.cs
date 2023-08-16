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
        else if (ColumnManager.Get.columns.Any(x => !x.occupied) && QuestionManager.nextQuestionIndex > 4 && PlayerManager.Get.players.Count(x => !x.inHotseat) != 0)
            StartCoroutine(GettingPlayers());
        else
            ResetForNextQuestion();
    }

    IEnumerator ExtraLives()
    {
        foreach (Column c in ColumnManager.Get.columns.Where(x => x.containedPlayer != null && x.containedPlayer.lastFive.Count(x => x) == 5))
            c.ElevateBall(true);

        HostManager.Get.UpdateClientLeaderboards();
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

        HostManager.Get.UpdateClientLeaderboards();
        for (int i = 0; i < 2; i++)
        {
            foreach (PlayerObject p in PlayerManager.Get.players)
            {
                if (i == 0)
                {
                    HostManager.Get.SendPayloadToClient(p, EventLibrary.HostEventType.Clear, "");
                    p.lastFive = new bool[5];
                }
                HostManager.Get.SendPayloadToClient(p, EventLibrary.HostEventType.DataFields, $"<color=#9f9f9f15>--|{p.points}|{p.hotseatLives.ToString()}");
            }
            if (ColumnManager.Get.columns.Any(x => x.occupied && x.containedPlayer.hotseatLives < 3))
            {
                foreach (Column c in ColumnManager.Get.columns.Where(x => x.occupied && x.containedPlayer.hotseatLives < 3))
                    c.ElevateBall(false);
                yield return new WaitForSeconds(5f);
            }
        }
        ResetPlayerVariables();
        QuestionManager.nextQuestionIndex = 0;
        GameplayManager.Get.currentStage = GameplayManager.GameplayStage.RevealInstructions;
        InstructionsManager.Get.Discs();
    }
}
