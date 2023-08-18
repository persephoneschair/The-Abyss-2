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
    private const string welcomeMessage = "Welcome to <font=Abyss><color=blue>The Abyss</color></font>\nJoin the game at <color=yellow>https://persephoneschair.itch.io/abyss</color> using the room code <color=green>[ABCD]</color>";
    private const string permaMessage = "Join the game at <color=yellow>https://persephoneschair.itch.io/abyss</color> using the room code <color=green>[ABCD]</color>";

    public Animator permaCodeAnim;
    public TextMeshProUGUI permaCodeMesh;

    [Button]
    public void OnOpenLobby()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.OpenAndLockLobby);
        lobbyCodeAnim.SetTrigger("toggle");
        welcomeMessageMesh.text = welcomeMessage.Replace("[ABCD]", HostManager.Get.host.RoomCode.ToUpperInvariant());
    }

    [Button]
    public void OnLockLobby()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.OpenAndLockLobby);
        lobbyCodeAnim.SetTrigger("toggle");
        permaCodeMesh.text = permaMessage.Replace("[ABCD]", HostManager.Get.host.RoomCode.ToUpperInvariant());
        Invoke("TogglePermaCode", 1f);
    }

    public void TogglePermaCode()
    {
        permaCodeAnim.SetTrigger("toggle");
    }
}
