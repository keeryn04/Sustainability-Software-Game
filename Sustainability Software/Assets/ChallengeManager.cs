using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeveloperMapping
{
    public string developerName;
    public GameObject developerObject;
}
public class ChallengeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> developerButtons;
    [SerializeField] private List<Button> strategyButtons;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private GameObject developerBubble;
    [SerializeField] private int hearDuration = 3;

    public List<DeveloperMapping> developers;
    private Dictionary<string, GameObject> developerDict;
    private ChallengeService.Quiz currentQuiz;
    private int currentIndex = 0;
    private string selectedStrategy;
    private string selectedDeveloper;

    public string topic = "Software Sustainability";

    async void Start()
    {
        developerDict = new Dictionary<string, GameObject>();
        foreach (var mapping in developers)
        {
            if (!developerDict.ContainsKey(mapping.developerName))
            {
                developerDict.Add(mapping.developerName, mapping.developerObject);
            }
            else
            {
                Debug.LogWarning($"Duplicate developer name: {mapping.developerName}");
            }
        }

        currentQuiz = await ChallengeService.GenerateChallengeAsync(topic, 5);

        if (currentQuiz == null || currentQuiz.questions.Count == 0)
        {
            questionText.text = "Failed to load quiz.";
            return;
        }

        DisplayQuestion();

        if (developerBubble != null)
            developerBubble.SetActive(false);
    }

    public GameObject GetDeveloperObject(string developerName)
    {
        if (developerDict.TryGetValue(developerName, out var obj))
            return obj;
        Debug.LogWarning($"Developer not found: {developerName}");
        return null;
    }

    void DisplayQuestion()
    {
        explanationText.text = "";
        var question = currentQuiz.questions[currentIndex];
        questionText.text = question.bossQuestion;

        for (int i = 0; i < strategyButtons.Count; i++)
        {
            if (i < question.strategies.Length)
            {
                strategyButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.strategies[i].description;
                strategyButtons[i].gameObject.SetActive(true);
                int strategyIndex = i;
                strategyButtons[i].onClick.RemoveAllListeners();
                strategyButtons[i].onClick.AddListener(() => StartCoroutine(CheckAnswer(selectedDeveloper, question.strategies[strategyIndex].id)));
            }
            else
            {
                strategyButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator CheckAnswer(string selectedDeveloper, string selectedStrategyId)
    {
        var question = currentQuiz.questions[currentIndex];

        bool developerCorrect = selectedDeveloper == question.correctDeveloper;

        bool strategyCorrect = selectedStrategyId == question.correctStrategyId;

        GameObject correctDeveloper = GetDeveloperObject(question.correctDeveloper);

        //Set specific developer based on correct
        DialogueManager.Instance.AssignDeveloperUI(
            correctDeveloper.GetComponent<Animator>(),
            correctDeveloper.GetComponent<AudioSource>()
        );

        developerBubble.SetActive(true);

        if (developerCorrect && strategyCorrect)
        {
            yield return StartCoroutine(DialogueManager.Instance.TypeText(
                "Correct! " + question.explanation,
                explanationText
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

            yield return StartCoroutine(DialogueManager.Instance.TypeText(feedback, explanationText));
        }

        yield return new WaitForSeconds(hearDuration);

        developerBubble.SetActive(false);

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
            questionText.text = "Done";
        }
    }

    public void OnDeveloperSelected(string developer)
    {
        selectedDeveloper = developer;
    }
}
