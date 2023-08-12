using Control;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerObject
{
    public string playerClientID;
    public Player playerClientRef;
    public GlobalLeaderboardStrap strap;
    public GlobalLeaderboardStrap cloneStrap;
    public string otp;
    public string playerName;

    public string twitchName;
    public Texture profileImage;

    public bool inHotseat;

    public int points;
    public string submission;
    public float submissionTime;
    public bool wasCorrect;

    public PlayerObject(Player pl)
    {
        playerClientRef = pl;
        otp = OTPGenerator.GenerateOTP();
        playerName = pl.Name;
        points = 0;
    }

    public void ApplyProfilePicture(string name, Texture tx, bool bypassSwitchAccount = false)
    {
        //Player refreshs and rejoins the same game
        if(PlayerManager.Get.players.Count(x => (!string.IsNullOrEmpty(x.twitchName)) && x.twitchName.ToLowerInvariant() == name.ToLowerInvariant()) > 0 && !bypassSwitchAccount)
        {
            PlayerObject oldPlayer = PlayerManager.Get.players.FirstOrDefault(x => x.twitchName.ToLowerInvariant() == name.ToLowerInvariant());
            if (oldPlayer == null)
                return;

            HostManager.Get.SendPayloadToClient(oldPlayer, EventLibrary.HostEventType.SecondInstance, "");

            oldPlayer.playerClientID = playerClientID;
            oldPlayer.playerClientRef = playerClientRef;
            oldPlayer.playerName = playerName;
            oldPlayer.strap.PopulateStrap(oldPlayer, false);
            oldPlayer.cloneStrap.PopulateStrap(oldPlayer, true);
            HostManager.Get.SendPayloadToClient(oldPlayer, EventLibrary.HostEventType.Validated, $"{oldPlayer.playerName}|{oldPlayer.points.ToString()}");

            otp = "";
            strap = null;
            cloneStrap = null;
            playerClientRef = null;
            playerName = "";

            PlayerManager.Get.players.Remove(this);
            HostManager.Get.UpdateClientLeaderboards();
            return;
        }
        otp = "";
        twitchName = name.ToLowerInvariant();
        profileImage = tx;
        points = 0;
        inHotseat = false;
        LeaderboardManager.Get.PlayerHasJoined(this);
        HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.Validated, $"{playerName}|{points.ToString()}");
        HostManager.Get.UpdateClientLeaderboards();
    }
}
