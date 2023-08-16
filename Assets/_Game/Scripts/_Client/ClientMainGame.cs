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
    public GameObject timerObj;
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
    public BetterTextBlock ansBetterTextBlock;
    public Color[] answerBlockBorderCols;
    public Color[] answerBlockBackgroundCols;

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
        previousFiveMesh.text = "0/0";
        livesOrInactiveMesh.text = "IN THE WINGS";
        timerObj.SetActive(true);
        leaderboardManager.gameObject.SetActive(true);
        topDataFieldsObj.gameObject.SetActive(true);
        leaderboardManager.RefreshScrollRect();
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
        //leaderboardManager.RefreshScrollRect();
    }

    public void DisplayCountdown()
    {
        timerAnim.SetTrigger("toggle");
    }

    public void DisplayQuestion(string[] data)
    {
        timerAnim.SetTrigger("toggle");
        catObj.SetActive(true);
        catMesh.text = data[0];
        questionObj.SetActive(true);
        questionMesh.text = data[1];
        ansInputObj.SetActive(true);
        submitButton.gameObject.SetActive(true);
        ansInput.ActivateInputField();
        enterSubmits = true;
    }

    public void OnSubmitAnswer()
    {
        enterSubmits = false;
        ansInputObj.SetActive(false);
        submittedAnswerMesh.text = ansInput.text;
        submitButton.gameObject.SetActive(false);
        ClientManager.Get.SendPayloadToHost(ansInput.text, EventLibrary.ClientEventType.Answer);
        ansInput.text = "";
    }

    public void DisplayAnswer(string[] data)
    {
        //[0] = Answer;
        //[1] = WasCorrect as string.ToUpperInvariant();

        ansInputObj.SetActive(false);
        submitButton.gameObject.SetActive(false);
        enterSubmits = false;
        ansObj.SetActive(true);
        ansMesh.text = data[0];
        ansBetterTextBlock.blockColorScheme = answerBlockBackgroundCols[data[1] == "TRUE" ? 0 : 1];
        ansBetterTextBlock.borderColor = answerBlockBorderCols[data[1] == "TRUE" ? 0 : 1];
        ansBetterTextBlock.GetComponent<Image>().color = answerBlockBackgroundCols[data[1] == "TRUE" ? 0 : 1];
        ansBetterTextBlock.GetComponentsInChildren<Image>()[1].color = answerBlockBorderCols[data[1] == "TRUE" ? 0 : 1];
    }

    public void UpdateDataFields(string[] data)
    {
        //[0] = LastFive
        //[1] = Score
        //[2] = HotseatLives.ToString()

        previousFiveMesh.text = data[0];
        playerScoreMesh.text = data[1];
        livesOrInactiveMesh.text = data[2] == "0" ? "IN THE WINGS" : "LIVES: " + data[2];
    }

    public void ResetForNewQuestion()
    {
        ClearScreen();
        DisplayCountdown();
    }

    public void ClearScreen()
    {
        catObj.SetActive(false);
        questionObj.SetActive(false);
        ansObj.SetActive(false);
        submittedAnswerMesh.text = "<color=#9f9f9f15>SUBMITTED ANSWER";
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
