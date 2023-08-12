using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    #region Init

    public static LeaderboardManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public GlobalLeaderboardStrap[] straps;
    public GlobalLeaderboardStrap[] cloneStraps;
    public float reorderDuration = 2f;

    public List<GlobalLeaderboardStrap> originalStraps = new List<GlobalLeaderboardStrap>();

    private void Start()
    {
        foreach (GlobalLeaderboardStrap strap in straps)
            strap.SetUpStrap();
        foreach (GlobalLeaderboardStrap strap in cloneStraps)
            strap.SetUpStrap();

        originalStraps = straps.ToList();
    }

    [Button]
    public void ReorderBoard()
    {
        straps = straps.OrderByDescending(x => x.containedPlayer != null).ThenByDescending(x => x.containedPlayer?.points).ThenBy(x => x.containedPlayer?.playerName).ToArray();
        cloneStraps = cloneStraps.OrderByDescending(x => x.containedPlayer != null).ThenByDescending(x => x.containedPlayer?.points).ThenBy(x => x.containedPlayer?.playerName).ToArray();
        //Extensions.ShuffleParallel(straps, cloneStraps);
        for (int i = 0; i < straps.Length; i++)
        {
            straps[i].MoveStrap(originalStraps[i].startPos, i);
            cloneStraps[i].MoveStrap(originalStraps[i].startPos, i);
        }
    }

    public void PlayerHasJoined(PlayerObject po)
    {
        straps.FirstOrDefault(x => x.containedPlayer == null).PopulateStrap(po, false);
        cloneStraps.FirstOrDefault(x => x.containedPlayer == null).PopulateStrap(po, true);
        ReorderBoard();
    }
}
