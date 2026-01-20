using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public List<Button> optionButtons;
    public TextMeshProUGUI explanationText;

    private QuizService.Quiz currentQuiz;
    private int currentIndex = 0;

    private List<string> topics = new List<string>
    {
        "Reducing energy consumption in software systems",
        "Maintainable and scalable software architectures",
        "Cost efficiency and resource optimization in software",
        "Accessibility and user well-being in software"             
    };

    async void Start()
    {
        string topic = topics[Random.Range(0, topics.Count)];

        currentQuiz = await QuizService.GenerateQuizAsync(topic, 5);

        if (currentQuiz == null || currentQuiz.questions.Count == 0)
        {
            questionText.text = "Failed to load quiz.";
            return;
        }

        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (currentQuiz == null || currentQuiz.questions == null || currentQuiz.questions.Count == 0)
        {
            Debug.LogError("Quiz data missing or empty");
            return;
        }

        if (currentIndex < 0 || currentIndex >= currentQuiz.questions.Count)
        {
            Debug.LogError($"Invalid question index: {currentIndex}");
            return;
        }

        explanationText.text = "";
        var question = currentQuiz.questions[currentIndex];
        questionText.text = question.question;

        if (question.options == null || question.options.Count == 0)
    {
            Debug.LogError("Question options are null or empty");
            return;
        }

        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i < question.options.Count)
            {
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[i];
                optionButtons[i].gameObject.SetActive(true);
                int index = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => CheckAnswer(index));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        var question = currentQuiz.questions[currentIndex];
        if (selectedIndex == question.correctIndex)
        {
            StartCoroutine(DialogueManager.Instance.TypeText("Correct! " + question.explanation, explanationText));
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.TypeText("Incorrect. " + question.explanation, explanationText));
        }

        NextQuestion();
    }

    void NextQuestion()
    {
        currentIndex++;

        if (currentIndex < currentQuiz.questions.Count)
        {
            DisplayQuestion();
        }
        else
        {
            questionText.text = "Quiz complete!";
            foreach (var btn in optionButtons)
                btn.gameObject.SetActive(false);
        }
    }
}
