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
    public Disc currentQDisc;
    public Animator currentQAnim;

    private void Update()
    {
        currentQTimeMesh.text = GetSecondsPerQuestionRemaining(currentQDisc.AngRadiansEnd * 57.2958f).ToString("#0");
        if (questionClockRunning)
            QuestionTimer();
        else
            elapsedTime = 0;
    }

    private void Start()
    {
        //Run the clock down to zero, rather than changing the default animation state (will thank myself when it comes to doing the QCounter anim)
        currentQAnim.SetTrigger("toggle");
    }

    public void StartClock()
    {
        questionClockRunning = true;
        currentQAnim.SetTrigger("toggle");
    }

    public void StopClock()
    {
        questionClockRunning = false;
    }

    public void ResetClock(bool startOnReset)
    {
        currentQAnim.SetTrigger("toggle");
        if (startOnReset)
            Invoke("StartClock", 3f);
    }

    void QuestionTimer()
    {
        elapsedTime += (1f * Time.deltaTime);
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
