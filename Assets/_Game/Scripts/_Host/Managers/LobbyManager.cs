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
    private const string welcomeMessage = "Welcome to <font=AbyssSurface><color=blue>The Abyss</color></font>\nPlaying on a mobile device? Scan the QR code:\n\n\n\n\nOn a desktop or laptop? Please visit:\n<color=yellow>https://persephoneschair.itch.io/gamenight</color>\n<size=350%><color=green>[ABCD]</color>";
    private const string permaMessage = "Join the game at <color=yellow>https://persephoneschair.itch.io/abyss</color> using the room code <color=green>[ABCD]</color>";

    public Animator permaCodeAnim;
    public TextMeshProUGUI permaCodeMesh;
    public Animator distortionBlockerAnim;

    [Button]
    public void OnOpenLobby()
    {
        distortionBlockerAnim.SetTrigger("toggle");
        AudioManager.Get.Play(AudioManager.OneShotClip.OpenAndLockLobby);
        lobbyCodeAnim.SetTrigger("toggle");
        welcomeMessageMesh.text = welcomeMessage.Replace("[ABCD]", HostManager.Get.host.RoomCode.ToUpperInvariant());
    }

    [Button]
    public void OnLockLobby()
    {
        distortionBlockerAnim.SetTrigger("toggle");
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
