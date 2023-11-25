using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EventLibrary
{
    public enum HostEventType
    {
        Default,
        Validate,
        Validated,
        SecondInstance,

        Information,
        UpdateScore,

        SimpleQuestion,
        MultipleChoiceQuestion,
        MultiSelectQuestion,

        SingleAndMultiResult,

        Leaderboard,
        WrongApp,
        WRONGAPP
    };
    public enum ClientEventType { Default, StoredValidation, SimpleQuestion, MultipleChoiceQuestion, MultiSelectQuestion, PasteAlert };

    public static string GetHostEventTypeString(HostEventType e)
    {
        return e.ToString();
    }
    public static string GetClientEventTypeString(ClientEventType e)
    {
        return e.ToString();
    }

    public static HostEventType GetHostEventType(string eString)
    {
        try
        {
            return (HostEventType)Enum.Parse(typeof(HostEventType), eString);
        }
        catch
        {
            return HostEventType.Default;
        }
    }

    public static ClientEventType GetClientEventType(string eString)
    {
        try
        {
            return (ClientEventType)Enum.Parse(typeof(ClientEventType), eString);
        }
        catch
        {
            return ClientEventType.Default;
        }
    }
}
