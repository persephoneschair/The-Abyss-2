using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientLeaderboardStrap : MonoBehaviour
{
    public TextMeshProUGUI positionMesh;
    public TextMeshProUGUI nameMesh;
    public TextMeshProUGUI pointsMesh;
    public BetterBox box;
    public Color[] boxOptions;
    public Color[] borderOptions;

    public void PopulateStrap(string[] data, int index)
    {
        positionMesh.text = Extensions.AddOrdinal(index + 1);
        nameMesh.text = data[0];
        pointsMesh.text = data[1];

        SetBackgroundColor(data[2] == "TRUE");
        SetBorderColor(nameMesh.text.ToUpperInvariant().Trim() == ClientMainGame.Get.playerNameMesh.text.ToUpperInvariant().Trim());
    }

    public void SetBackgroundColor(bool hotseat)
    {
        box.boxColorScheme = hotseat ? boxOptions[0] : boxOptions[1];
        box.background.color = hotseat ? boxOptions[0] : boxOptions[1];
    }

    public void SetBorderColor(bool isClient)
    {
        box.borderColor = isClient ? borderOptions[0] : borderOptions[1];
        box.border.color = isClient ? borderOptions[0] : borderOptions[1];
    }
}
