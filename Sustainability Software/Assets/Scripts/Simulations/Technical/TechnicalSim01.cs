using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechnicalSim : MonoBehaviour
{
    public TextMeshProUGUI codeTextDisplay;
    public TextMeshProUGUI feedbackText;
    public List<CodeSnippet> snippets;

    private int currentIndex = 0;
    private int score = 0;

    public void Awake()
    {
        ShowNextSnippet();
    }

    void ShowNextSnippet()
    {
        if (currentIndex < snippets.Count)
        {
            codeTextDisplay.text = snippets[currentIndex].codeText;
            feedbackText.text = "";
        }
        else
        {
            codeTextDisplay.text = "Simulation Complete!";
            feedbackText.text = "Final Score: " + score;
        }
    }

    public void ChooseGood()
    {
        CheckAnswer(true);
    }

    public void ChooseBad()
    {
        CheckAnswer(false);
    }

    void CheckAnswer(bool userChoice)
    {
        CodeSnippet snippet = snippets[currentIndex];
        if (userChoice == snippet.isGoodCode)
        {
            feedbackText.text = snippet.feedbackGood;
            score += 1;
        }
        else
        {
            feedbackText.text = snippet.feedbackBad;
        }

        currentIndex++;
        Invoke("ShowNextSnippet", 1.5f); //Wait 1.5s then show next snippet
    }
}
