using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> optionButtons;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private GameObject explanationBubble;
    [SerializeField] private float hearDuration = 3f;
    [SerializeField] private string currentTopic;

    private QuizService.Quiz currentQuiz;
    private int currentIndex = 0;

    public List<string> topics = new List<string>
    {
        //Environmental Sustainability
        "Energy-Efficient Software Design",
        "Reducing Carbon Footprint in Cloud Computing",
        "Green Software Engineering Practices",
        "Optimizing Software for Lower Energy Consumption",

        //Economic Sustainability
        "Cost-Efficient Software Architecture",
        "Long-Term Maintainability and Technical Debt",
        "Sustainable Software Business Models",
        "Balancing Performance and Infrastructure Costs",

        //Social Sustainability
        "Ethical Software Development",
        "Inclusive and Accessible Software Design",
        "User Privacy and Data Responsibility",
        "Social Impact of Software Systems",

        //Technical Sustainability
        "Modular and Maintainable Software Systems",
        "Scalable and Sustainable Software Architectures",
        "Refactoring for Long-Term Code Health",
        "Designing Software for Longevity"
    };

    async void Start()
    {
        currentTopic = topics[Random.Range(0, topics.Count)];

        //Subscribe to isTalking changes
        DialogueManager.Instance.OnTalkingStateChanged += HandleTalkingStateChanged;

        currentQuiz = await QuizService.GenerateQuizAsync(currentTopic, 5);
        if (currentQuiz == null || currentQuiz.questions.Count == 0)
        {
            questionText.text = "Failed to load quiz.";
            return;
        }

        DisplayQuestion();

        //Set initial button state
        SetButtonsInteractable(!DialogueManager.Instance.isTalking);
    }

    void DisplayQuestion()
    {
        var question = currentQuiz.questions[currentIndex];
        questionText.text = question.question;

        SetButtonsInteractable(!DialogueManager.Instance.isTalking);

        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i < question.options.Count)
            {
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[i];
                optionButtons[i].gameObject.SetActive(true);
                int index = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => StartCoroutine(CheckAnswer(index)));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator CheckAnswer(int selectedIndex)
    {
        var question = currentQuiz.questions[currentIndex];
        SetButtonsInteractable(false);
        explanationBubble.SetActive(true);

        if (selectedIndex == question.correctIndex)
        {
            yield return StartCoroutine(DialogueManager.Instance.TypeText("Correct! " + question.explanation, explanationText));
        }
        else
        {
            yield return StartCoroutine(DialogueManager.Instance.TypeText("Incorrect. " + question.explanation, explanationText));
        }

        yield return new WaitForSeconds(hearDuration);
        explanationBubble.SetActive(false);

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
    private void HandleTalkingStateChanged(bool isTalking)
    {
        SetButtonsInteractable(!isTalking);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in optionButtons)
            btn.interactable = interactable;
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnTalkingStateChanged -= HandleTalkingStateChanged;
    }
}
