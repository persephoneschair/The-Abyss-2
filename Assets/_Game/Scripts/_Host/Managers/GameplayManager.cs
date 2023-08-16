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
    public Round[] rounds;

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
    public GameplayStage currentStage = GameplayStage.RunTitles;

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
                if(Operator.Get.recoveryMode)
                {
                    //THIS IS NOT GOING TO WORK IF THE QUESTIONS AREN'T SEEDED IN THE SAME ORDER - try and think of a way around this
                    Operator.Get.skipOpeningTitles = true;
                    foreach(PlayerObject p in PlayerManager.Get.players)
                    {
                        SaveManager.RestorePlayer(p);
                    }
                    SaveManager.RestoreGameplayState();
                    currentStage = GameplayStage.RevealInstructions;
                    Operator.Get.recoveryMode = false;
                }
                else
                {
                    currentStage = GameplayStage.DoNothing;
                    TitlesManager.Get.RunTitleSequence();
                }
                break;

            case GameplayStage.OpenLobby:
                LobbyManager.Get.OnOpenLobby();
                currentStage++;
                break;

            case GameplayStage.LockLobby:
                LobbyManager.Get.OnLockLobby();
                currentStage++;
                break;

            case GameplayStage.RevealInstructions:
                currentRound++;
                InstructionsManager.Get.OnShowInstructions();
                currentStage++;
                break;

            case GameplayStage.HideInstructions:
                InstructionsManager.Get.OnHideInstructions();
                currentStage++;
                break;

            case GameplayStage.RunSection:
                rounds[(int)currentRound - 1].LoadQuestion();
                currentStage = GameplayStage.DoNothing;
                break;

            case GameplayStage.RollCredits:
                CreditsManager.Get.RollCredits();
                //Pennys
                currentStage++;
                break;

            case GameplayStage.DoNothing:
                break;
        }
    }
}
