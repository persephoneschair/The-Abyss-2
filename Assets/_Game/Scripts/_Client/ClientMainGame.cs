using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ClientMainGame : MonoBehaviour
{
    #region Init

    public static ClientMainGame Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    [Header("Top Data Fields")]
    public GameObject topDataFieldsObj;
    public TextMeshProUGUI playerNameMesh;
    public TextMeshProUGUI playerScoreMesh;
    public TextMeshProUGUI previousFiveMesh;
    public TextMeshProUGUI submittedAnswerMesh;
    public TextMeshProUGUI livesOrInactiveMesh;

    [Header("Misc Gameplay Area")]
    public Animator timerAnim;
    public bool enterSubmits;

    [Header("Category")]
    public GameObject catObj;
    public TextMeshProUGUI catMesh;

    [Header("Question")]
    public GameObject questionObj;
    public TextMeshProUGUI questionMesh;

    [Header("Answer Input")]
    public GameObject ansInputObj;
    public TMP_InputField ansInput;
    public Button submitButton;

    [Header("Answer")]
    public GameObject ansObj;
    public TextMeshProUGUI ansMesh;

    [Header("Leaderboard")]
    public ClientLeaderboardManager leaderboardManager;

    [Header("Fixed Message")]
    public GameObject fixedMessageObj;
    public TextMeshProUGUI fixedMessageMesh;

    private void Update()
    {
        submitButton.interactable = ansInput.text.Length <= 0 ? false : true;

        if (enterSubmits && ansInput.text.Length > 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            OnSubmitAnswer();
    }

    public void Initialise(string[] otpArr)
    {
        playerNameMesh.text = otpArr[0];
        playerScoreMesh.text = otpArr[1];
        previousFiveMesh.text = "0/5";
        livesOrInactiveMesh.text = "IN THE WINGS";
        timerAnim.gameObject.SetActive(true);
        leaderboardManager.gameObject.SetActive(true);
        topDataFieldsObj.gameObject.SetActive(true);
    }

    public void UpdateLeaderboard(string data)
    {
        string[] players = data.Split('¬');
        for(int i = 0; i < players.Length; i++)
        {
            //[0] = Name
            //[1] = Score
            //[2] = In hotseat?
            string[] splitData = players[i].Split('|');
            leaderboardManager.PopulateStrap(splitData, i);
        }
        leaderboardManager.RefreshScrollRect();
    }

    public void DisplayCountdown()
    {

    }

    public void DisplayQuestion()
    {

    }

    public void OnSubmitAnswer()
    {
        ClientManager.Get.SendPayloadToHost("ANSWER GOES HERE", EventLibrary.ClientEventType.Answer);
    }

    public void DisplayResponse(string[] data)
    {

    }

    public void DisplayAnswer(string data)
    {

    }

    public void UpdateCurrentScore(string data)
    {

    }

    public void ResetForNewQuestion()
    {
        
    }

    public void NewInstanceOpened()
    {
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "THE TWITCH ACCOUNT ASSOCIATED WITH THIS CONTROLLER HAS BEEN ASSOCIATED WITH ANOTHER CONTROLLER.\n\n" +
            "THIS CONTROLLER WILL NOT RECEIVE FURTHER DATA FROM THE GAME AND CAN NOW BE CLOSED.\n\n" +
            "IF YOU DID  NOT VALIDATE A NEW CONTROLLER, PLEASE CONTACT THE HOST.";
    }

    public void EndOfGameAlert(string pennyValue)
    {
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "THE GAME HAS NOW CONCLUDED AND THIS CONTROLLER CAN BE CLOSED.";
    }
}
