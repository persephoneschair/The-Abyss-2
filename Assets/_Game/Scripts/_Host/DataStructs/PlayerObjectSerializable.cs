using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectSerializable
{
    public string playerClientID;
    public string playerName;

    public string twitchName;

    public bool inHotseat;

    public int points;
    public bool[] lastFive;
}
