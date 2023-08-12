using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System.Linq;
using Control;

public class GameplayManager : MonoBehaviour
{
    [Header("Rounds")]
    public Round[] rounds;

    [Header("Question Data")]
    public static int nextQuestionIndex = 0;

    public enum GameplayStage
    {
        RunTitles,
        OpenLobby,
        LockLobby,
        RevealInstructions,
        HideInstructions,
        RunSection,

        RollCredits,
        DoNothing
    };
    public GameplayStage currentStage = GameplayStage.DoNothing;

    public enum RoundType { None, MainGame, FinalGame };
    public RoundType currentRound = RoundType.None;
    public int roundsPlayed = 0;

    #region Init

    public static GameplayManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    [Button]
    public void ProgressGameplay()
    {
        switch (currentStage)
        {
            case GameplayStage.RunTitles:
                //If in recovery mode, we need to call Restore Players to restore specific player data (client end should be handled by the reload host call)
                //Also need to call Restore gameplay state to bring us back to where we need to be (skipping titles along the way)
                //Reveal instructions would probably be a sensible place to go to, though check that doesn't iterate any game state data itself
                break;

            case GameplayStage.OpenLobby:
                break;

            case GameplayStage.LockLobby:
                break;

            case GameplayStage.RevealInstructions:
                break;

            case GameplayStage.HideInstructions:
                break;

            case GameplayStage.RunSection:
                break;

            case GameplayStage.RollCredits:
                break;

            case GameplayStage.DoNothing:
                break;
        }
    }
}
