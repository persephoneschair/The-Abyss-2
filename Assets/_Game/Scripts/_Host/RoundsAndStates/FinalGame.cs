using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinalGame : Round
{
    public override void UpdateColumnsPostQuestion()
    {
        base.UpdateColumnsPostQuestion();

        ResetForNextQuestion();
    }

    public override void ResetForNextQuestion()
    {
        //Check for all columns empty
        if (ColumnManager.Get.columns.Count(x => x.occupied) <= 1)
        {
            GameplayManager.Get.currentRound = GameplayManager.RoundType.None;
            InstructionsManager.Get.Discs();
            categoryMesh.text = "<font=AbyssSurface><color=#AB0000>GAME OVER</color></font>";

            if (ColumnManager.Get.columns.Count(x => x.occupied) == 1)
            {
                questionMesh.text = $"<font=AbyssSurface><color=blue>{ColumnManager.Get.columns.FirstOrDefault(x => x.occupied).containedPlayer.playerName} has survived\nThe Abyss</color></font>";
                PennyManager.Get.ApplyWinnerList(new List<PlayerObject> { ColumnManager.Get.columns.FirstOrDefault(x => x.occupied).containedPlayer } );
            }
                
            else
                questionMesh.text = "<font=AbyssSurface><color=blue>The Abyss has taken you all...</color></font>";

            GameplayManager.Get.currentStage = GameplayManager.GameplayStage.RollCredits;
            foreach (PlayerObject pl in PlayerManager.Get.players)
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Clear, "");

            foreach (Column c in ColumnManager.Get.columns.Where(x => x.occupied))
                c.SetColumnColor(Column.MaterialChoice.ProfileTexture, Column.MaterialChoice.StandardColor);
        }
        //Then check for if we're out of questions
        else if (QuestionManager.nextQuestionIndex == QuestionManager.GetRoundQCount())
        {
            GameplayManager.Get.currentRound = GameplayManager.RoundType.None;
            InstructionsManager.Get.Discs();
            categoryMesh.text = "<font=AbyssSurface><color=#AB0000>GAME OVER</color></font>";

            //Ugly, ugly logic to do tiebreaker stuff
            List<PlayerObject> orderedPlayers = ColumnManager.Get.columns.Where(x => x.containedPlayer != null).Select(x => x.containedPlayer).ToList();
            orderedPlayers = orderedPlayers.OrderByDescending(x => x.hotseatLives).ThenByDescending(x => x.points).ToList();

            List<PlayerObject> winningPlayers = new List<PlayerObject>();
            for(int i = 0; i < orderedPlayers.Count; i++)
            {
                if (orderedPlayers[i].hotseatLives == orderedPlayers[0].hotseatLives && orderedPlayers[i].points == orderedPlayers[0].points)
                    winningPlayers.Add(orderedPlayers[i]);
                else
                    break;
            }                

            if (winningPlayers.Count == 1)
                questionMesh.text = $"<font=AbyssSurface><color=blue>{winningPlayers[0].playerName} has survived\nThe Abyss</color></font>";
            else
            {
                string concat = "";
                for (int i = 0; i < winningPlayers.Count; i++)
                {
                    concat += winningPlayers[i].playerName;
                    if (i == winningPlayers.Count - 2)
                        concat += " and ";
                    else if (i != winningPlayers.Count - 1)
                        concat += ", ";
                }
                questionMesh.text = $"<font=AbyssSurface><color=blue>{concat} have survived\nThe Abyss</color></font>";
            }
            PennyManager.Get.ApplyWinnerList(winningPlayers);
            GameplayManager.Get.currentStage = GameplayManager.GameplayStage.RollCredits;

            foreach (PlayerObject pl in PlayerManager.Get.players)
                HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Clear, "");
        }
        else
        {
            ResetPlayerVariables();
            LoadQuestion();
        }
    }
}
