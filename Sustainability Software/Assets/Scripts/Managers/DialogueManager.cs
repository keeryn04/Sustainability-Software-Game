using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI clientText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("UI Elements")]
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private ResourceBar resourceBar;

    private ScenarioData currentScenario;
    private GoalData currentGoal;
    private string[] currentChoices;
    private float playerScore;
    private List<ChoiceData> playerDecisions = new List<ChoiceData>();

    //Singleton
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

    //Used by SceneInitializer to update UI elements in object
    public void AssignUI(
    TextMeshProUGUI newClientText,
    TextMeshProUGUI newObjectiveText,
    TextMeshProUGUI newScoreText,
    Button[] newChoiceButtons,
    ResourceBar newResourceBar)
    {
        clientText = newClientText;
        objectiveText = newObjectiveText;
        scoreText = newScoreText;
        choiceButtons = newChoiceButtons;
        resourceBar = newResourceBar;
    }

    public void SetupScenarioUI()
    {
        //Assign local variables for UI info
        currentGoal = GameTracker.Instance.CurrentGoal;
        currentScenario = MenuManager.Instance.CurrentScenario;

        if (currentScenario == null || currentGoal == null)
        {
            Debug.LogError("Scenario and Goal Data not set!");
            return;
        }

        //Adjust player stats from Current Goal (Game Tracker)
        objectiveText.text = "Objective: " + currentGoal.objective;
        playerScore = 0;

        //Adjust scenario details from Current Scenario (Menu Manager)
        clientText.text = currentScenario.clientBrief;
        currentChoices = new string[currentScenario.choices.Length];
        resourceBar.SetResourceScenario(currentScenario);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentScenario.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentScenario.choices[i].choiceText;

                int index = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetupReflectionUI()
    {
        currentScenario = MenuManager.Instance.CurrentScenario;

        if (currentScenario == null)
        {
            Debug.LogError("No scenario found for reflection scene.");
            return;
        }

        string textToShow = currentScenario.reflectionFeedback + "\n\n";

        foreach (var decision in playerDecisions)
        {
            textToShow += $"Choice: {decision.choiceText}\nReflection: {decision.reflection}\n\n";
        }

        clientText.text = "";
        clientText.text = textToShow;
        Debug.Log(textToShow);
    }

    private async Task TypeText(string text)
    {
        //Lock buttons so unmatching inputs aren't accepted
        foreach (Button button in choiceButtons)
        {
            button.interactable = false;
        }

        clientText.text = ""; //Clear previous text

        foreach (char c in text)
        {

            clientText.text += c;
            await Task.Delay((int)(typingSpeed * 1000));
        }

        //Reenable buttons after text is done
        foreach (Button button in choiceButtons)
        {
            button.interactable = true;
        }
    }

    private async void OnChoiceSelected(int choiceIndex)
    {
        string playerChoice = currentChoices[choiceIndex];

        // Get LLM response
        string jsonResponse = await LLMService.SendChoiceAsync(currentScenario, playerChoice);
        LLMResponse parsed = JsonUtility.FromJson<LLMResponse>(jsonResponse);

        // Store choice in decisions list
        playerDecisions.Add(new ChoiceData
        {
            choiceText = playerChoice,
            reflection = parsed.reflection
        });

        // Update resource bar
        resourceBar.AddValue(parsed.resourceImpact);

        // Update choices
        currentChoices = parsed.choices;

        // Show response with typing effect
        await TypeText(parsed.clientResponse);

        // Update game state
        GameTracker.Instance.RegisterDecision(parsed.resourceImpact);
        playerScore = GameTracker.Instance.PlayerScore; //keep UI in sync
        scoreText.text = playerScore.ToString();

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(i < parsed.choices.Length);
            if (i < parsed.choices.Length)
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = parsed.choices[i];
        }
    }
}
