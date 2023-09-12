using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Column : MonoBehaviour
{
    public float doorDelay = 3f;

    public GameObject ballToInstance;
    public Transform ballSpawnPoint;
    public TextMeshPro nameMesh;
    public Color fontColor;
    public GameObject[] consecutiveIndicators;

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
    public Animator nameAnim;

    private void Start()
    {
        nameAnim.speed = UnityEngine.Random.Range(0.2f, 1f);
    }

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
                backRend.material = columnMats[(int)MaterialChoice.UnlitBlack];
        }
    }

    public void ActivateColumn()
    {
        occupied = true;
        containedPlayer = PlayerManager.Get.SelectARandomPlayer();

        if(containedPlayer != null)
        {
            nameMesh.color = fontColor;
            nameMesh.text = containedPlayer.playerName;
            DebugLog.Print($"{containedPlayer.playerName} has joined The Abyss Arena.", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Blue);
            containedPlayer.inHotseat = true;
            containedPlayer.lastFive = new bool[5];
            containedPlayer.hotseatLives = 3;
            scoreMesh.text = containedPlayer.points.ToString();
            containedPlayer.UpdateSideStraps();
            //HostManager.Get.SendPayloadToClient(containedPlayer, EventLibrary.HostEventType.DataFields, $"{containedPlayer.lastFive.Count(x => x)}/5|{containedPlayer.points}|{containedPlayer.hotseatLives}");
        }
        else
            scoreMesh.text = "--";

        SetColumnColor(MaterialChoice.ProfileTexture, MaterialChoice.StandardColor);
        AudioManager.Get.Play(AudioManager.OneShotClip.ChuteJoin);
        OpenNextDoor();
        instancedBall.rend.material = columnMats[(int)MaterialChoice.GlowingColor];
        //HostManager.Get.UpdateClientLeaderboards();
        StartCoroutine(ActivationFlicker(false));
    }

    IEnumerator ActivationFlicker(bool kill)
    {
        yield return new WaitForSeconds(0.03f);
        for(int i = 0; i < 5; i++)
        {
            SetColumnColor(MaterialChoice.UnlitBlack, MaterialChoice.UnlitBlack);
            nameMesh.enabled = false;
            scoreMesh.enabled = false;
            yield return new WaitForSeconds(0.03f);

            if (kill && i == 4)
                break;

            SetColumnColor(kill ? MaterialChoice.IncorrectColor : MaterialChoice.ProfileTexture, kill ? MaterialChoice.IncorrectColor : MaterialChoice.StandardColor);
            nameMesh.enabled = true;
            scoreMesh.enabled = true;
            yield return new WaitForSeconds(0.03f);
        }
    }

    [Button]
    public void OpenNextDoor()
    {
        ClearAllConsecutiveStraps();
        OpenADoor(nextDoorToOpen);
        if (nextDoorToOpen != Trapdoor.Bottom)
            nextDoorToOpen++;
        else
        {
            nextDoorToOpen = Trapdoor.ScoreArray;
            EliminatePlayerFromColumn();
        }
    }

    public void ElevateBall(bool debug)
    {
        if (instancedBall == null || instancedBall.elevate)
            return;

        containedPlayer.lastFive = new bool[5];
        if (nextDoorToOpen > Trapdoor.Top)
        {
            AudioManager.Get.PlayUnique(AudioManager.OneShotClip.ChuteExtraLife);
            if(debug)
                DebugLog.Print($"{containedPlayer.playerName} got five right in a row and earned an extra life.", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Yellow);
            containedPlayer.hotseatLives++;
            nextDoorToOpen--;
            OpenNextDoor();
            nextDoorToOpen--;
            instancedBall.Elevate();
        }
        SetConsecutiveLights(0);
        //HostManager.Get.SendPayloadToClient(containedPlayer, EventLibrary.HostEventType.DataFields, $"{containedPlayer.lastFive.Count(x => x)}/5|{containedPlayer.points}|{containedPlayer.hotseatLives}");
    }

    public void SetConsecutiveLights(int totalStraps)
    {
        foreach (GameObject go in consecutiveIndicators)
            go.SetActive(false);
        for(int i = 0; i < totalStraps; i++)
            consecutiveIndicators[i].SetActive(true);
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
            nameMesh.text = "";
            AudioManager.Get.PlayUnique(AudioManager.OneShotClip.ChuteDrop);
            DebugLog.Print($"{containedPlayer.playerName} has dropped into The Abyss and is out of the game!", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Orange);
            containedPlayer.inHotseat = false;
            containedPlayer.lastFive = new bool[5];
            containedPlayer.UpdateSideStraps();
            containedPlayer.justEliminated = true;
            containedPlayer = null;
        }
        occupied = false;
        StartCoroutine(ActivationFlicker(true));
    }

    private void ClearAllConsecutiveStraps()
    {
        foreach (GameObject go in consecutiveIndicators)
            go.SetActive(false);
    }

    #endregion
}
