using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> optionButtons;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private GameObject explanationBubble;
    [SerializeField] private int waitDuration = 3;

    private QuizService.Quiz currentQuiz;
    private int currentIndex = 0;

    private Dictionary<SustainabilityPillar, string> topics = new Dictionary<SustainabilityPillar, string>
    {
        { SustainabilityPillar.Environmental, "Reducing energy consumption in software systems"},
        { SustainabilityPillar.Technical, "Maintainable and scalable software architectures"},
        { SustainabilityPillar.Economic, "Cost efficiency and resource optimization in software"},
        { SustainabilityPillar.Social, "Accessibility and user well-being in software"}             
    };

    async void Start()
    {

        string topic = ChooseRandomTopic();

        currentQuiz = await QuizService.GenerateQuizAsync(topic, 5);

        if (currentQuiz == null || currentQuiz.questions.Count == 0)
        {
            questionText.text = "Failed to load quiz.";
            return;
        }

        DisplayQuestion();
    }

    private void OnEnable()
    {
        DialogueManager.Instance.OnTalkingStateChanged += HandleTalkingStateChanged;
    }

    private void OnDisable()
    {
        DialogueManager.Instance.OnTalkingStateChanged -= HandleTalkingStateChanged;
    }

    private void HandleTalkingStateChanged(bool isTalking)
    {
        //Disable buttons when talking, enable after
        foreach (var btn in optionButtons)
        {
            btn.interactable = !isTalking;
        }
    }

    private string ChooseRandomTopic()
    {
        List<SustainabilityPillar> pillarsVisited = MenuManager.Instance.PillarsVisited;

        SustainabilityPillar chosenPillar = pillarsVisited[Random.Range(0, pillarsVisited.Count)]; //Pick pillar based on what user has already learned
        string topic = topics[chosenPillar];

        return topic;
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
                optionButtons[i].interactable = true;
                int index = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => StartCoroutine(CheckAnswer(index)));
            }
            else
            {
                optionButtons[i].interactable = false;
            }
        }
    }

    private IEnumerator CheckAnswer(int selectedIndex)
    {
        var question = currentQuiz.questions[currentIndex];

        explanationBubble.SetActive(true);
        if (selectedIndex == question.correctIndex)
        {
            yield return StartCoroutine(DialogueManager.Instance.TypeText("Correct! " + question.explanation, explanationText));
        }
        else
        {
            yield return StartCoroutine(DialogueManager.Instance.TypeText("Incorrect. " + question.explanation, explanationText));
        }

        yield return new WaitForSeconds(waitDuration);

        explanationBubble.SetActive(false);

        NextQuestion();
    }

    private void NextQuestion()
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
                btn.interactable = false;
        }
    }
}
