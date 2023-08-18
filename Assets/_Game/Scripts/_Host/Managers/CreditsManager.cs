using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    #region Init

    public static CreditsManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public GameObject endCard;
    public Animator backgroundAnim;
    public GameObject sceneBlocker;

    public Animator creditsAnim;
    [TextArea(5,10)] public string[] creditsOptions;
    public TextMeshProUGUI creditsMesh;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    [Button]
    public void RollCredits()
    {
        AudioManager.Get.Play(AudioManager.LoopClip.CreditsTheme, false);
        StartCoroutine(Credits());
    }

    IEnumerator Credits()
    {
        backgroundAnim.SetTrigger("toggle");
        yield return new WaitForSeconds(3f);
        sceneBlocker.SetActive(true);
        for(int i = 0; i < creditsOptions.Length - 1; i++)
        {
            yield return new WaitForSeconds(1.25f);
            creditsMesh.text = creditsOptions[i];
            creditsAnim.SetTrigger("toggle");
            yield return new WaitForSeconds(3f);
            creditsAnim.SetTrigger("toggle");
        }
        yield return new WaitForSeconds(2.5f);
        creditsMesh.text = creditsOptions[creditsOptions.Length - 1];
        creditsAnim.SetTrigger("toggle");
        yield return new WaitForSeconds(10f);
        creditsMesh.text = "";
        endCard.SetActive(true);
    }
}
