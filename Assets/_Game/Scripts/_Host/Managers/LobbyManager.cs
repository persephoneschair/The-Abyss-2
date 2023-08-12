using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    #region Init

    public static LobbyManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public TextMeshProUGUI welcomeMessageMesh;
    public Animator lobbyCodeAnim;
    private const string welcomeMessage = "Welcome to the game [ABCD]";
    private const string permaMessage = "Perma Message [ABCD]";

    public Animator permaCodeAnim;
    public TextMeshProUGUI permaCodeMesh;

    [Button]
    public void OnOpenLobby()
    {
        lobbyCodeAnim.SetTrigger("toggle");
        welcomeMessageMesh.text = welcomeMessage.Replace("[ABCD]", HostManager.Get.host.RoomCode.ToUpperInvariant());
    }

    [Button]
    public void OnLockLobby()
    {
        lobbyCodeAnim.SetTrigger("toggle");
        permaCodeMesh.text = permaMessage.Replace("[ABCD]", HostManager.Get.host.RoomCode.ToUpperInvariant());
        Invoke("TogglePermaCode", 1f);
    }

    public void TogglePermaCode()
    {
        permaCodeAnim.SetTrigger("toggle");
    }
}
