using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI clientText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Scenario Elements")]
    [SerializeField] private ScenarioData currentScenario;
    [SerializeField] private string goalObjective;
    [SerializeField] private float playerScore;

    [Header("In-Engine Assignments")]
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private ResourceBar resourceBar;

    private string[] currentChoices;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Persist across scenes
        }
    }

    public void InitializeScenario(ScenarioData currentScenario)
    {
        objectiveText.text = "Objective: " + goalObjective;
        clientText.text = currentScenario.clientBrief;
        currentChoices = new string[currentScenario.choices.Length];
        resourceBar.SetScenario(currentScenario);

        for (int i = 0; i < currentScenario.choices.Length; i++)
        {
            currentChoices[i] = currentScenario.choices[i].choiceText;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentScenario.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentScenario.choices[i].choiceText;

                int choiceIndex = i; //capture index for closure
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private async void OnChoiceSelected(int choiceIndex)
    {
        string playerChoice = currentChoices[choiceIndex];

        //Get LLM response
        string jsonResponse = await LLMService.SendChoiceAsync(currentScenario, playerChoice);
        LLMResponse parsed = JsonUtility.FromJson<LLMResponse>(jsonResponse);

        //Update resource bar with LLM's resourceImpact
        resourceBar.AddValue(parsed.resourceImpact);
        
        //Update choices
        currentChoices = parsed.choices;

        //Update UI with response and choices
        clientText.text = parsed.clientResponse;
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(i < parsed.choices.Length);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = parsed.choices[i];
        }

        //Update overall score, decisions made, and check for end condition
        GameTracker.Instance.RegisterDecision(parsed.resourceImpact);

        //Update score
        scoreText.text = "Score: " + playerScore.ToString();
    }

    public void SetObjectiveText(string objective) { this.goalObjective = objective; }
    public void SetPlayerScore(float playerScore) { this.playerScore = playerScore; }
}

[System.Serializable]
public class LLMResponse
{
    public string clientResponse;
    public string[] choices;
    public float resourceImpact;
}
