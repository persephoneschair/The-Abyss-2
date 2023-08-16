using NaughtyAttributes;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionCounter : MonoBehaviour
{
    #region Init

    public static QuestionCounter Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public Disc ring;
    public TextMeshPro questionCountMesh;
    public float animationTime = 2f;

    private float currentAngle = 0;
    private float targetAngle = 0;
    private bool updatingRing;
    private float elapsedTime = 0;

    [Button]
    public void UpdateQuestionNumber()
    {
        int remainingQuestions = QuestionManager.GetRoundQCount() - QuestionManager.nextQuestionIndex;
        float anglePerQuestion = 360f / QuestionManager.GetRoundQCount();

        currentAngle = ring.AngRadiansEnd * 57.2958f;
        targetAngle = (anglePerQuestion * remainingQuestions) + 90f;
        questionCountMesh.text = remainingQuestions.ToString();
        elapsedTime = 0;
        updatingRing = true;
        Invoke("EndAnimation", animationTime + 0.1f);
    }

    private void Update()
    {
        if (updatingRing)
            PerformLerp();
    }

    private void PerformLerp()
    {
        elapsedTime += Time.deltaTime;

        float percentageComplete = elapsedTime / animationTime;

        ring.AngRadiansEnd = Mathf.Lerp(currentAngle / 57.2958f, targetAngle / 57.2958f, Mathf.SmoothStep(0, 1, percentageComplete));
    }

    private void EndAnimation()
    {
        updatingRing = false;
    }
}
