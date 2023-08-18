using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Init

    public static PlayerManager Get { get; private set; }

    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public List<PlayerObject> players = new List<PlayerObject>();

    [Header("Controls")]
    public bool pullingData = true;
    [Range(0,39)] public int playerIndex;


    private PlayerObject _focusPlayer;
    public PlayerObject FocusPlayer
    {
        get { return _focusPlayer; }
        set
        {
            if(value != null)
            {
                _focusPlayer = value;
                playerName = value.playerName;
                twitchName = value.twitchName;
                profileImage = value.profileImage;
                wasCorrect = value.wasCorrect;
                inHotseat = value.inHotseat;

                points = value.points;
                submission = value.submission;
            }
            else
            {
                playerName = "OUT OF RANGE";
                twitchName = "OUT OF RANGE";
                profileImage = null;
                wasCorrect = false;
                inHotseat = false;

                points = 0;
                submission = "OUT OF RANGE";
                submissionTime = 0;
            }                
        }
    }

    [Header("Fixed Fields")]
    [ShowOnly] public string playerName;
    [ShowOnly] public string twitchName;
    public Texture profileImage;
    [ShowOnly] public bool wasCorrect;
    [ShowOnly] public bool inHotseat;

    [Header("Variable Fields")]
    public int points;
    public string submission;
    public float submissionTime;

    void UpdateDetails()
    {
        if (playerIndex >= players.Count)
            FocusPlayer = null;
        else
            FocusPlayer = players.OrderBy(x => x.playerName).ToList()[playerIndex];
    }

    private void Update()
    {
        if (pullingData)
            UpdateDetails();
    }

    [Button]
    public void SetPlayerDetails()
    {
        if (pullingData)
            return;
        SetDataBack();
    }

    [Button]
    public void RestoreOrEliminatePlayer()
    {
        if (pullingData)
            return;
        pullingData = true;

    }

    void SetDataBack()
    {
        FocusPlayer.points = points;
        FocusPlayer.submission = submission;
        pullingData = true;
    }

    public PlayerObject SelectARandomPlayer()
    {
        List<PlayerObject> playerDraw = new List<PlayerObject>();
        foreach (PlayerObject p in players.Where(x => !x.inHotseat && !x.justEliminated && x.verified))
        {
            playerDraw.Add(p);
            for (int i = 0; i < p.lastFive.Count(x => x); i++)
                playerDraw.Add(p);
        }
        if (playerDraw.Count > 0)
            return playerDraw[UnityEngine.Random.Range(0, playerDraw.Count)];

        else
            return null;
    }

    public void WaitForCountdown(PlayerObject p)
    {
        StartCoroutine(CountdownDelayRoutine(p));
    }

    IEnumerator CountdownDelayRoutine(PlayerObject p)
    {
        yield return new WaitUntil(() => !Round.entryDisabled);
        p.verified = true;
        LeaderboardManager.Get.PlayerHasJoined(p);
        HostManager.Get.SendPayloadToClient(p, EventLibrary.HostEventType.Validated, $"{playerName}|{points.ToString()}");
        HostManager.Get.UpdateClientLeaderboards();
    }
}
