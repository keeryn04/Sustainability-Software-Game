using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

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
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private Transform reflectionGrid;
    [SerializeField] private Animator typingAnimator;
    [SerializeField] private AudioClip[] textSounds;
    [SerializeField] private AudioSource audioSource;

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
    ResourceBar newResourceBar,
    Animator newTypingAnimator,
    AudioSource newAudioSource)
    {
        clientText = newClientText;
        objectiveText = newObjectiveText;
        scoreText = newScoreText;
        choiceButtons = newChoiceButtons;
        resourceBar = newResourceBar;
        typingAnimator = newTypingAnimator;
        audioSource = newAudioSource;
    }

    public void AssignReflectionUI(
    GameObject newSpeechBubble,
    Transform newReflectionGrid)
    {
        speechBubble = newSpeechBubble;
        reflectionGrid = newReflectionGrid;
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

    public async void SetupReflectionUI()
    {
        currentScenario = MenuManager.Instance.CurrentScenario;

        if (currentScenario == null)
        {
            Debug.LogError("No scenario found for reflection scene.");
            return;
        }

        string textToShow = currentScenario.reflectionFeedback;
        clientText.text = "";
        await TypeTextGeneral(textToShow, clientText);  

        foreach (var decision in playerDecisions)
        {
            string decisionText = $"Choice: {decision.choiceText}\nReflection: {decision.reflection}";

            //Spawn reflection bubble and position
            GameObject reviewBubble = Instantiate(speechBubble, reflectionGrid);
            reviewBubble.transform.localScale = Vector3.one;
            reviewBubble.transform.localPosition = Vector3.zero;

            //Get text box of review bubble and type in it
            TextMeshProUGUI reviewTextBox = reviewBubble.GetComponentInChildren<TextMeshProUGUI>();
            if (reviewTextBox != null)
            {
                await TypeTextGeneral(decisionText, reviewTextBox);
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI found in prefab!");
            }
        }
    }

    private async Task TypeTextPlay(string text)
    {
        //Lock buttons so unmatching inputs aren't accepted
        if (choiceButtons != null && choiceButtons.Length > 0)
        {
            foreach (Button button in choiceButtons)
            {
                button.interactable = false;
            }
        }

        clientText.text = ""; //Clear previous text

        if (typingAnimator != null)
            typingAnimator.SetBool("IsTyping", true);

        foreach (char c in text)
        {
            clientText.text += c;
            if (textSounds.Length > 0 && Random.value < 0.3f)
            {
                int randomIndex = Random.Range(0, textSounds.Length);
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(textSounds[randomIndex]);
            }

            await Task.Delay((int)(typingSpeed * 1000));
        }

        if (typingAnimator != null)
            typingAnimator.SetBool("IsTyping", false);


        //Reenable buttons after text is done
        if (choiceButtons != null && choiceButtons.Length > 0)
        {
            foreach (Button button in choiceButtons)
            {
                button.interactable = true;
            }
        }
    }

    private async Task TypeTextGeneral(string text, TextMeshProUGUI textBox)
    {
        textBox.text = ""; //Clear the bubble first

        if (typingAnimator != null)
            typingAnimator.SetBool("IsTyping", true);

        foreach (char c in text)
        {
            textBox.text += c; 
            if (textSounds.Length > 0 && Random.value < 0.5f)
            {
                int randomIndex = Random.Range(0, textSounds.Length);
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(textSounds[randomIndex]);
            }

            await Task.Delay((int)(typingSpeed * 1000));
        }

        if (typingAnimator != null)
            typingAnimator.SetBool("IsTyping", false);
    }

    private async void OnChoiceSelected(int choiceIndex)
    {
        string playerChoice = choiceButtons[choiceIndex].GetComponentInChildren<TextMeshProUGUI>().text;

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
        await TypeTextPlay(parsed.clientResponse);

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
