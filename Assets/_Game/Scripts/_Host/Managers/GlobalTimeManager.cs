using Shapes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalTimeManager : MonoBehaviour
{
    #region Init

    public static GlobalTimeManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public bool questionClockRunning;
    [ShowOnly] public float elapsedTime;

    public TextMeshPro currentQTimeMesh;
    public Disc currentQAnim;

    private void Update()
    {
        if (questionClockRunning)
            QuestionTimer();
        else
            elapsedTime = 0;
    }

    void QuestionTimer()
    {
        elapsedTime += (1f * Time.deltaTime);
        currentQTimeMesh.text = GetSecondsPerQuestionRemaining(currentQAnim.AngRadiansEnd * 57.2958f).ToString("#0");
        Debug.Log(currentQAnim.AngRadiansEnd);
    }

    public float GetRawTimestamp()
    {
        return elapsedTime;
    }

    public string GetFormattedTimestamp()
    {
        return elapsedTime.ToString("#0.00");
    }
    
    public float GetSecondsPerQuestionRemaining(float animationAngle)
    {
        return Mathf.FloorToInt((animationAngle - 90) / 24);
    }
}
