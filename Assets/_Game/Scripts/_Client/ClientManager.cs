using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
using TMPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class ClientManager : MonoBehaviour
{
    public Client client;

    #region Init

    public static ClientManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    #region Connection

    public void AttemptToConnectToRoom(string name, string roomCode)
    {
        client.Connect(name, roomCode);
    }

    public void OnConnected(string roomCode)
    {
        Invoke("CheckConnection", 2f);
    }

    void CheckConnection()
    {
        if (!client.Connected)
            ClientLandingPageManager.Get.OnCouldNotConnectToRoom();
    }

    #endregion

    #region Payload Management

    public void OnPayloadReceived(DataMessage dm)
    {
        string data = (string)dm.Data;
        EventLibrary.HostEventType ev = EventLibrary.GetHostEventType(dm.Key);        

        switch (ev)
        {
            case EventLibrary.HostEventType.Validate:
                ClientLandingPageManager.Get.OnValidateAccount(data);
                break;

            case EventLibrary.HostEventType.Validated:
                string[] dataArr = data.Split('|');
                ClientLandingPageManager.Get.OnValidated(dataArr);
                break;

            case EventLibrary.HostEventType.SecondInstance:
                ClientMainGame.Get.NewInstanceOpened();
                break;

            case EventLibrary.HostEventType.Leaderboard:
                ClientMainGame.Get.UpdateLeaderboard(data);
                break;

            case EventLibrary.HostEventType.PrepForQuestion:
                ClientMainGame.Get.ResetForNewQuestion();
                break;

            case EventLibrary.HostEventType.QuestionPacket:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplayQuestion(dataArr);
                break;

            case EventLibrary.HostEventType.AnswerPacket:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplayAnswer(dataArr);
                break;

            case EventLibrary.HostEventType.DataFields:
                dataArr = data.Split('|');
                ClientMainGame.Get.UpdateDataFields(dataArr);
                break;

            case EventLibrary.HostEventType.Clear:
                ClientMainGame.Get.ClearScreen();
                break;

            default:
                break;
        }
    }

    public void SendPayloadToHost(string payload, EventLibrary.ClientEventType eventType)
    {
        var js = JsonConvert.SerializeObject(payload);
        JObject j = new JObject
        {
            { EventLibrary.GetClientEventTypeString(eventType), js }
        };
        client.SendEvent(EventLibrary.GetClientEventTypeString(eventType), j);
    }

    public void TestButton()
    {
        ClientLandingPageManager.Get.gameObject.SetActive(true);
    }

    #endregion
}
