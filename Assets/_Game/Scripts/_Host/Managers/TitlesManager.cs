using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitlesManager : MonoBehaviour
{
    #region Init

    public static TitlesManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public Animator titlesAnim;
    public Animator backdropFader;
    public TextMeshProUGUI titlesMesh;
    [TextArea(2, 3)] public string[] titlesOptions;

    public GameObject sceneBlocker;

    [Button]
    public void RunTitleSequence()
    {
        if (Operator.Get.skipOpeningTitles)
            EndOfTitleSequence();
        else
        {
            GameplayManager.Get.currentStage = GameplayManager.GameplayStage.DoNothing;
            StartCoroutine(TitleSequence());
        }           
    }

    IEnumerator TitleSequence()
    {
        for(int i = 0; i < titlesOptions.Length - 1; i++)
        {
            titlesMesh.text = titlesOptions[i];
            titlesAnim.SetTrigger("toggle");
            yield return new WaitForSeconds(7.75f);
        }
        titlesMesh.text = titlesOptions[titlesOptions.Length-1];
        titlesAnim.SetTrigger("final");
        yield return new WaitForSeconds(6f);
        sceneBlocker.SetActive(false);
        backdropFader.SetTrigger("toggle");
        yield return new WaitForSeconds(3f);
        EndOfTitleSequence();
    }

    void EndOfTitleSequence()
    {
        sceneBlocker.SetActive(false);
        GameplayManager.Get.currentStage = GameplayManager.GameplayStage.OpenLobby;
        this.gameObject.SetActive(false);
    }
}
