using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Column : MonoBehaviour
{
    public float doorDelay = 3f;

    public GameObject ballToInstance;
    public Transform ballSpawnPoint;

    public enum Trapdoor { ScoreArray, Top, Middle, Bottom };
    public Trapdoor nextDoorToOpen = Trapdoor.ScoreArray;
    public Animator[] trapdoors;
    private Ball instancedBall;

    public PlayerObject containedPlayer;
    public bool occupied;

    public Renderer backRend;
    public Renderer[] otherRends;

    public enum MaterialChoice { UnlitBlack, ProfileTexture, StandardColor, GlowingColor, CorrectColor, IncorrectColor };
    public Material[] columnMats;

    public TextMeshPro scoreMesh;

    public void SetColumnColor(MaterialChoice back, MaterialChoice rest)
    {
        foreach (Renderer r in otherRends)
            r.material = columnMats[(int)rest];

        backRend.material = columnMats[(int)back];
        if(back == MaterialChoice.ProfileTexture)
        {
            if (containedPlayer != null)
            {
                backRend.material.mainTexture = containedPlayer.profileImage;
                scoreMesh.text = containedPlayer.points.ToString();
            }
            else
                backRend.material = columnMats[(int)MaterialChoice.StandardColor];
        }
    }

    public void ActivateColumn()
    {
        occupied = true;
        containedPlayer = PlayerManager.Get.SelectARandomPlayer();

        if(containedPlayer != null)
        {
            DebugLog.Print($"{containedPlayer.playerName} has joined The Abyss Arena.", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Blue);
            containedPlayer.inHotseat = true;
            containedPlayer.lastFive = new bool[5];
            containedPlayer.hotseatLives = 3;
            scoreMesh.text = containedPlayer.points.ToString();
            containedPlayer.UpdateSideStraps();
            HostManager.Get.SendPayloadToClient(containedPlayer, EventLibrary.HostEventType.DataFields, $"{containedPlayer.lastFive.Count(x => x)}/5|{containedPlayer.points}|{containedPlayer.hotseatLives}");
        }
        else
            scoreMesh.text = "--";

        SetColumnColor(MaterialChoice.ProfileTexture, MaterialChoice.StandardColor);
        OpenNextDoor();
        instancedBall.rend.material = columnMats[(int)MaterialChoice.GlowingColor];
        HostManager.Get.UpdateClientLeaderboards();
    }

    [Button]
    public void OpenNextDoor()
    {
        OpenADoor(nextDoorToOpen);
        if (nextDoorToOpen != Trapdoor.Bottom)
            nextDoorToOpen++;
        else
        {
            nextDoorToOpen = Trapdoor.ScoreArray;
            EliminatePlayerFromColumn();
        }
    }

    [Button]
    public void ElevateBall(bool debug)
    {
        if (instancedBall == null || instancedBall.elevate)
            return;

        containedPlayer.lastFive = new bool[5];
        if (nextDoorToOpen > Trapdoor.Top)
        {
            //SFX
            if(debug)
                DebugLog.Print($"{containedPlayer.playerName} got five right in a row and earned an extra life.", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Yellow);
            containedPlayer.hotseatLives++;
            nextDoorToOpen--;
            OpenNextDoor();
            nextDoorToOpen--;
            instancedBall.Elevate();
        }
    }

    #region Private Methods

    private void OpenADoor(Trapdoor doorToOpen)
    {
        trapdoors[(int)doorToOpen].SetTrigger("toggle");
        StartCoroutine(AutoCloseTrapdoor(doorToOpen));
        if (doorToOpen == Trapdoor.ScoreArray)
            instancedBall = Instantiate(ballToInstance.GetComponent<Ball>(), ballSpawnPoint);
        else
            instancedBall.AddForce();
    }

    private IEnumerator AutoCloseTrapdoor(Trapdoor doorToClose)
    {
        yield return new WaitForSeconds(doorDelay);
        trapdoors[(int)doorToClose].SetTrigger("toggle");
    }

    private void EliminatePlayerFromColumn()
    {
        SetColumnColor(MaterialChoice.UnlitBlack, MaterialChoice.UnlitBlack);
        scoreMesh.text = "";
        if(containedPlayer != null)
        {
            DebugLog.Print($"{containedPlayer.playerName} has dropped into The Abyss and is out of the game!", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Orange);
            containedPlayer.inHotseat = false;
            containedPlayer.lastFive = new bool[5];
            containedPlayer.UpdateSideStraps();
            containedPlayer = null;
        }
        occupied = false;
    }

    #endregion
}
