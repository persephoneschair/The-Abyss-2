using NaughtyAttributes;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    #region Init

    public static InstructionsManager Get { get; private set; }
    private void Awake()
    {
        if (Get != null && Get != this)
            Destroy(this);
        else
            Get = this;
    }

    #endregion

    public Animator timerRevealAnim;
    public Animator qCounterRevealAnim;

    public TextMeshProUGUI instructionsMesh;
    public Animator instructionsAnim;
    private readonly string[] instructions = new string[2]
    {
        "You will face [###] questions across five different categories. Use the crossword clues to help you as SPELLING MATTERS.\n\n" +
        "" +
        "After five questions, nine players will be selected to join <font=AbyssSurface><color=blue>The Abyss</color></font> arena. Inside the arena, wrong answers or abstentions cost you a life. Five consecutive correct answers earns you an extra life (up to a maximum of three).\n\n" +
        "" +
        "If you exhaust your lives, you fall into <font=AbyssSurface><color=blue>The Abyss</color></font> and are out of the game. A player from the wings will replace you.\n\n" +
        "" +
        "The nine players inside the arena at the conclusion of question [###] go onto the final. Good luck.",


        "You will face up to [###] questions across the same five categories.\n\n" +

        "Players in <font=AbyssSurface><color=blue>The Abyss</color></font> have had their lives refreshed.\n\n" +

        "Gameplay is identical to previously, except that extra lives are no longer awarded for five consecutive correct answers and players who drop into <font=AbyssSurface><color=blue>The Abyss</color></font> are permanently out of the game and will not be replaced.\n\n" +

        "The last player left standing is declared the winner. In the event no winner has emerged after [###] questions, the winner is the player who is higher up their column. In the event of a tie, the player with the highest score is declared the winner. Good luck."
    };

    [Button]
    public void OnShowInstructions()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.OpenAndLockLobby);
        instructionsAnim.SetTrigger("toggle");
        instructionsMesh.text = instructions[(int)GameplayManager.Get.currentRound - 1].Replace("[###]", Extensions.NumberToWords(QuestionManager.GetRoundQCount()));
    }

    [Button]
    public void OnHideInstructions()
    {
        AudioManager.Get.Play(AudioManager.OneShotClip.GoToFinal);
        instructionsAnim.SetTrigger("toggle");
        Invoke("Discs", 1f);
    }

    public void Discs()
    {
        timerRevealAnim.SetTrigger("toggle");
        qCounterRevealAnim.SetTrigger("toggle");
        if (GameplayManager.Get.currentRound == GameplayManager.RoundType.FinalGame)
            QuestionCounter.Get.UpdateQuestionNumber();
    }
}
