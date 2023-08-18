using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

public class Question
{
    public string category;
    public string question;
    public string singleStringAnswers;
    public List<string> validAnswers = new List<string>();
}
