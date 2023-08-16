using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.PlayerLoop;
using UnityStandardAssets.Effects;
using System.Linq;

public static class QuestionManager
{
    public static Pack currentPack = null;
    public static List<Question> mainGameQuestions = new List<Question>();
    public static List<Question> endGameQuestions = new List<Question>();

    public static int nextQuestionIndex = 0;

    public static void DecompilePack(TextAsset tx)
    {
        currentPack = JsonConvert.DeserializeObject<Pack>(tx.text);
        CompilePlayOrders();
    }

    public static void CompilePlayOrders()
    {
        int[] mainGameOrder = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
        mainGameOrder.Shuffle();
        for(int i = 0; i < 8; i++)
        {
            List<Question> questionBlock = new List<Question>
            {
                currentPack.cat1Qs[mainGameOrder[i]],
                currentPack.cat2Qs[mainGameOrder[i]],
                currentPack.cat3Qs[mainGameOrder[i]],
                currentPack.cat4Qs[mainGameOrder[i]],
                currentPack.cat5Qs[mainGameOrder[i]]
            };
            questionBlock.Shuffle();
            mainGameQuestions.AddRange(questionBlock);
        }

        int[] endGameOrder = new int[2] { 8, 9 };
        endGameOrder.Shuffle();
        for(int i = 0; i < 2; i++)
        {
            List<Question> questionBlock = new List<Question>
            {
                currentPack.cat1Qs[endGameOrder[i]],
                currentPack.cat2Qs[endGameOrder[i]],
                currentPack.cat3Qs[endGameOrder[i]],
                currentPack.cat4Qs[endGameOrder[i]],
                currentPack.cat5Qs[endGameOrder[i]]
            };
            questionBlock.Shuffle();
            endGameQuestions.AddRange(questionBlock);
        }
        DebugLog.Print($"{mainGameQuestions.Count} questions in the main game.", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Blue);
        DebugLog.Print($"{endGameQuestions.Count} questions in the end game.", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Blue);
    }

    public static int GetRoundQCount()
    {
        switch (GameplayManager.Get.currentRound)
        {
            case GameplayManager.RoundType.MainGame:
                return mainGameQuestions.Count;

            case GameplayManager.RoundType.FinalGame:
                return endGameQuestions.Count;

            default:
                return 0;
        }
    }

    public static Question GetQuestion()
    {
        switch (GameplayManager.Get.currentRound)
        {
            case GameplayManager.RoundType.MainGame:
                return mainGameQuestions[QuestionManager.nextQuestionIndex];

            case GameplayManager.RoundType.FinalGame:
                return endGameQuestions[QuestionManager.nextQuestionIndex];

            default:
                return null;
        }
    }

    public static string GetClueLength(string[] validAnswers)
    {
        List<string> stubs = new List<string>();
        for(int i = 0; i < validAnswers.Length; i++)
        {
            string[] wordsInAnswer = validAnswers[i].Split(' ');

            //Strip non-letter/non-digit characters from answer clue (e.g. Ben's becomes Bens for the purpose of the clue)
            for(int j = 0; j < wordsInAnswer.Length; j++)
            {
                wordsInAnswer[j] = new string((from c in wordsInAnswer[j]
                                  where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                  select c
                       ).ToArray());
            }

            //Create a stub
            string stub = "[";
            for (int j = 0; j < wordsInAnswer.Length; j++)
            {
                stub += wordsInAnswer[j].Length;
                if (j < wordsInAnswer.Length - 1)
                    stub += ", ";
            }
            stub += "]";

            //Add to the stubs list if there is no identical clue
            if (!stubs.Contains(stub))
                stubs.Add(stub);
        }
        return string.Join(" OR ", stubs);
    }

    public static bool CheckSubmission(List<string> validAnswers, string submission)
    {
        //Strip all valid answers of punctuation and white space and add to the valid answer list
        List<string> valid = new List<string>( validAnswers );
        foreach(string answer in validAnswers)
        {
            string x = new string ((from c in answer
                        where char.IsLetterOrDigit(c)
                        select c
                       ).ToArray());
            if (!valid.Contains(x))
                valid.Add(x);
        }

        foreach(string potential in valid)
            if (potential.ToUpperInvariant() == submission.ToUpperInvariant())
                return true;

        //Strip white space and non-number/non-letter characters from submission and try again
        string x2 = new string((from c in submission
                                where char.IsLetterOrDigit(c)
                                select c).ToArray());

        foreach (string potential in valid)
            if (potential.ToUpperInvariant() == x2.ToUpperInvariant())
                return true;

        return false;
    }
}
