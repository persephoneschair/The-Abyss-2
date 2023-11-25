using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
using Newtonsoft.Json;
using System.Linq;

public class HostManager : MonoBehaviour
{
    [Header("Controlling Class")]
    public Host host;

    #region Init

    public static HostManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    #region Join Room & Validation

    public void OnRoomConnected(string roomCode)
    {
        DebugLog.Print($"PLAYERS MAY NOW JOIN THE ROOM WITH THE CODE {host.RoomCode}", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Blue);
    }

    public void OnPlayerJoins(Player joinedPlayer)
    {        
        if (PlayerManager.Get.players.Count >= Operator.Get.playerLimit && Operator.Get.playerLimit != 0)
        {
            //Do something slightly better than this
            return;
        }

        PlayerObject pl = new PlayerObject(joinedPlayer);
        pl.playerClientID = joinedPlayer.UserID;
        PlayerManager.Get.pendingPlayers.Add(pl);
        
        if(Operator.Get.recoveryMode)
        {
            DebugLog.Print($"{joinedPlayer.Name} HAS BEEN RECOVERED", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Orange);
            SaveManager.RestorePlayer(pl);
            if (pl.twitchName != null)
                StartCoroutine(RecoveryValidation(pl));
            else
            {
                pl.otp = "";
                //pl.podium.containedPlayer = null;
                //pl.podium = null;
                pl.playerClientRef = null;
                pl.playerName = "";
                PlayerManager.Get.players.Remove(pl);
                DebugLog.Print($"{joinedPlayer.Name} HAS BEEN CLEARED", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
                return;
            }
        }
        else if (Operator.Get.fastValidation)
            StartCoroutine(FastValidation(pl));

        DebugLog.Print($"{joinedPlayer.Name} HAS JOINED THE LOBBY", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Green);
        SendPayloadToClient(joinedPlayer, EventLibrary.HostEventType.Validate, $"{pl.otp}");
    }

    private IEnumerator FastValidation(PlayerObject pl)
    {
        yield return new WaitForSeconds(1f);
        TwitchManager.GetTwitchControl.testUsername = pl.playerName;
        TwitchManager.GetTwitchControl.testMessage = pl.otp;
        TwitchManager.GetTwitchControl.SendTwitchWhisper();
        TwitchManager.GetTwitchControl.testUsername = "";
        TwitchManager.GetTwitchControl.testMessage = "";
    }

    private IEnumerator RecoveryValidation(PlayerObject pl)
    {
        yield return new WaitForSeconds(1f);
        TwitchManager.GetTwitchControl.RecoveryValidation(pl.twitchName, pl.otp);
    }

    #endregion

    #region Payload Management

    public void SendPayloadToClient(PlayerObject pl, EventLibrary.HostEventType e, string data)
    {
        if (!pl.verified)
            return;
        host.UpdatePlayerData(pl.playerClientRef, EventLibrary.GetHostEventTypeString(e), data);
    }

    public void SendPayloadToClient(Control.Player pl, EventLibrary.HostEventType e, string data)
    {
        host.UpdatePlayerData(pl, EventLibrary.GetHostEventTypeString(e), data);
    }

    public void OnReceivePayloadFromClient(EventMessage e)
    {
        PlayerObject p = GetPlayerFromEvent(e);
        EventLibrary.ClientEventType eventType = EventLibrary.GetClientEventType(e.EventName);

        string s = (string)e.Data[e.EventName];
        var data = JsonConvert.DeserializeObject<string>(s);

        switch (eventType)
        {
            case EventLibrary.ClientEventType.SimpleQuestion:
                p.AnswerReceived(QuestionManager.CheckSubmission(GameplayManager.Get.rounds[(int)GameplayManager.Get.currentRound - 1].currentQuestion.validAnswers, data), data);
                break;

            case EventLibrary.ClientEventType.StoredValidation:
                string[] str = data.Split('|').ToArray();
                TwitchManager.GetTwitchControl.testUsername = str[0];
                TwitchManager.GetTwitchControl.testMessage = str[1];
                TwitchManager.GetTwitchControl.SendTwitchWhisper();
                TwitchManager.GetTwitchControl.testUsername = "";
                TwitchManager.GetTwitchControl.testMessage = "";
                break;

            case EventLibrary.ClientEventType.PasteAlert:
                //Silent alarm indicating some text has been pasted into an answer box
                DebugLog.Print($"A PASTE ALERT WAS RAISED BY {p.playerName} ({p.twitchName}): {data}", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Purple);
                string currentQ = "";
                switch(GameplayManager.Get.currentRound)
                {
                    case GameplayManager.RoundType.None:
                        currentQ = "No live question";
                        break;

                    case GameplayManager.RoundType.MainGame:
                        if (GameplayManager.Get.rounds[0].currentQuestion != null)
                            currentQ = GameplayManager.Get.rounds[0].currentQuestion.question;
                        break;

                    case GameplayManager.RoundType.FinalGame:
                        if (GameplayManager.Get.rounds[1].currentQuestion != null)
                            currentQ = GameplayManager.Get.rounds[1].currentQuestion.question;
                        break;
                }
                PasteAlertEvent.Log(p, data ,currentQ);
                EventLogger.PrintPasteLog();
                break;

            default:
                break;
        }
    }

    /*public void UpdateClientLeaderboards()
    {
        List<PlayerObject> lb = PlayerManager.Get.players.OrderByDescending(x => x.points).ThenBy(x => x.playerName).ToList();

        string payload = "";
        for (int i = 0; i < lb.Count; i++)
        {
            string sc = lb[i].points.ToString();
            string hs = lb[i].inHotseat ? "TRUE" : "FALSE";

            payload += $"{lb[i].playerName.ToUpperInvariant()}|{sc}|{hs}";
            if (i + 1 != lb.Count)
                payload += "¬";
        }

        foreach (PlayerObject pl in lb)
            SendPayloadToClient(pl, EventLibrary.HostEventType.Leaderboard, payload);
    }*/

    #endregion

    #region Helpers

    public PlayerObject GetPlayerFromEvent(EventMessage e)
    {
        return PlayerManager.Get.players.FirstOrDefault(x => x.playerClientRef == e.Player);
    }

    #endregion
}
