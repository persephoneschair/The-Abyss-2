using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
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

    public Renderer backRend;


    [Button]
    public void OpenNextDoor()
    {
        OpenADoor(nextDoorToOpen);
        if (nextDoorToOpen != Trapdoor.Bottom)
            nextDoorToOpen++;
        else nextDoorToOpen = Trapdoor.ScoreArray;
    }

    [Button]
    public void ElevateBall()
    {
        if (instancedBall == null || instancedBall.elevate)
            return;

        if (nextDoorToOpen > Trapdoor.Top)
        {
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

    #endregion
}
