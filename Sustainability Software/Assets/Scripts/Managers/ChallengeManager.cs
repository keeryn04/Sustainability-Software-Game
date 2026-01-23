using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeveloperMapping
{
    public string developerName;
    public SustainabilityPillar developerPillar;
}

public class ChallengeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> strategyButtons;
    [SerializeField] private List<Button> developerButtons;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI developerText;
    [SerializeField] private TextMeshProUGUI developerTitleText;
    [SerializeField] private GameObject developer;
    [SerializeField] private GameObject developerBubble;
    [SerializeField] private int hearDuration = 3;

    public List<DeveloperMapping> developers;
    private Dictionary<string, SustainabilityPillar> developerDict;
    private ChallengeService.Quiz currentQuiz;

    private string selectedStrategyId;
    private string selectedDeveloper;
    private int currentDeveloperIndex = 0;
    private int currentQuestionIndex = 0;

    public string topic = "Software Sustainability";

    async void Start()
    {
        developerDict = new Dictionary<string, SustainabilityPillar>();
        foreach (var mapping in developers)
        {
            if (!developerDict.ContainsKey(mapping.developerName))
            {
                developerDict.Add(mapping.developerName, mapping.developerPillar);
            }
            else
            {
                Debug.LogWarning($"Duplicate developer name: {mapping.developerName}");
            }
        }

        if (developerBubble != null)
            developerBubble.SetActive(false);

        SetActiveDeveloper(0);

        currentQuiz = await ChallengeService.GenerateChallengeAsync(topic, 5);

        if (currentQuiz == null || currentQuiz.questions.Count == 0)
        {
            questionText.text = "Failed to load quiz.";
            return;
        }

        DialogueManager.Instance.OnTalkingStateChanged += HandleTalkingStateChanged;

        DisplayQuestion();
    }

    private void OnDisable()
    {
        DialogueManager.Instance.OnTalkingStateChanged -= HandleTalkingStateChanged;
    }

    private void HandleTalkingStateChanged(bool isTalking)
    {
        //Disable buttons when talking, enable after
        foreach (var btn in developerButtons)
        {
            btn.interactable = !isTalking;
        }

        foreach (var btn in strategyButtons)
        {
            btn.interactable = !isTalking;
        }

        submitButton.interactable = !isTalking;
    }

    void SetActiveDeveloper(int index)
    {
        currentDeveloperIndex = (index + developers.Count) % developers.Count;

        selectedDeveloper = developers[currentDeveloperIndex].developerName;
        developerTitleText.text = selectedDeveloper;
        SustainabilityPillar developerPillar = developers[currentDeveloperIndex].developerPillar;

        var appearance = developer.GetComponent<DeveloperAppearanceSwapper>();
        if (appearance != null)
        {
            appearance.ApplyAppearance(developerPillar);
        }
    }

    public void NextDeveloper()
    {
        SetActiveDeveloper(currentDeveloperIndex + 1);
    }

    public void PreviousDeveloper()
    {
        SetActiveDeveloper(currentDeveloperIndex - 1);
    }

    void DisplayQuestion()
    {
        developerText.text = "";
        var question = currentQuiz.questions[currentQuestionIndex];
        questionText.text = question.bossQuestion;

        for (int i = 0; i < strategyButtons.Count; i++)
        {
            if (i < question.strategies.Length)
            {
                strategyButtons[i].gameObject.SetActive(true);
                int strategyIndex = i;
                strategyButtons[strategyIndex].onClick.RemoveAllListeners(); 
                strategyButtons[strategyIndex].onClick.AddListener(() => StartCoroutine(DisplayStrategy(question.strategies[strategyIndex])));
            }
            else
            {
                strategyButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator DisplayStrategy(ChallengeService.Strategy strategy)
    {
        developerText.text = "";
        developerBubble.SetActive(true);
        selectedStrategyId = strategy.id;

        foreach (var btn in developerButtons)
            btn.interactable = false;

        yield return StartCoroutine(DialogueManager.Instance.TypeText(strategy.description, developerText));

        foreach (var btn in developerButtons)
            btn.interactable = true;
    }

    public void SubmitAnswer()
    {
        StartCoroutine(SubmitAnswerCoroutine());
    }

    private IEnumerator SubmitAnswerCoroutine()
    {
        yield return StartCoroutine(CheckAnswer(selectedDeveloper, selectedStrategyId));
    }

    private IEnumerator CheckAnswer(string selectedDeveloper, string selectedStrategyId)
    {
        var question = currentQuiz.questions[currentQuestionIndex];

        bool developerCorrect = selectedDeveloper == question.correctDeveloper;
        bool strategyCorrect = selectedStrategyId == question.correctStrategyId;

        developerText.text = "";
        developerBubble.SetActive(true);
        foreach (var btn in strategyButtons)
            btn.interactable = false;

        if (developerCorrect && strategyCorrect)
        {
            yield return StartCoroutine(DialogueManager.Instance.TypeText(
                "Correct! " + question.explanation,
                developerText
            ));
        }
        else
        {
            string feedback = "Incorrect. ";

            if (!developerCorrect && !strategyCorrect)
                feedback += "Neither the developer nor the strategy fits this situation. ";
            else if (!developerCorrect)
                feedback += "The chosen developer is not best suited for this challenge. ";
            else
                feedback += "The strategy does not effectively address the boss's concern. ";

            feedback += question.explanation;

            yield return StartCoroutine(DialogueManager.Instance.TypeText(feedback, developerText));
        }

        yield return new WaitForSeconds(hearDuration);

        developerBubble.SetActive(false);

        foreach (var btn in strategyButtons)
            btn.interactable = true;

        NextQuestion();
    }

    void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex < currentQuiz.questions.Count)
        {
            DisplayQuestion();
        }
        else
        {
            questionText.text = "Done";
        }
    }
}
