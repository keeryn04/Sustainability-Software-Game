using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI clientText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private ResourceBar resourceBar;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private Transform reflectionGrid;
    [SerializeField] private TextMeshProUGUI reflectionTitle;
    [SerializeField] private Animator typingAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] textSounds;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    private ScenarioData currentScenario;
    private GoalData currentGoal;
    private string[] currentChoices;
    private float playerScore;
    private List<ChoiceData> playerDecisions = new List<ChoiceData>();

    public static DialogueManager Instance { get; private set; }
    public List<ChoiceData> PlayerDecisions => playerDecisions;
    private bool _isTalking;
    public bool isTalking
    {
        get => _isTalking;
        private set
        {
            if (_isTalking != value)
            {
                _isTalking = value;
                OnTalkingStateChanged?.Invoke(_isTalking); //Notify observers
            }
        }
    }

    public event Action<bool> OnTalkingStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    //Used by SceneInitializer to update UI elements in object
    public void AssignUI(TextMeshProUGUI client, TextMeshProUGUI objective, TextMeshProUGUI score,
                         Button[] choices, ResourceBar bar)
    {
        clientText = client;
        objectiveText = objective;
        scoreText = score;
        choiceButtons = choices;
        resourceBar = bar;
    }

    public void AssignDeveloperUI(Animator animator, AudioSource source)
    {
        typingAnimator = animator;
        audioSource = source;
    }

    public void AssignReflectionUI(TextMeshProUGUI reflectionFeedback, GameObject bubblePrefab, Transform grid, TextMeshProUGUI reflectionText)
    {
        clientText = reflectionFeedback;
        speechBubble = bubblePrefab;
        reflectionGrid = grid;
        reflectionTitle = reflectionText;
    }

    public IEnumerator TypeText(string text, TextMeshProUGUI targetBox)
    {
        isTalking = true;

        if (typingAnimator != null)
            typingAnimator.SetBool("IsTyping", true);

        targetBox.text = "";

        foreach (char c in text)
        {
            targetBox.text += c;
            PlayTextSound();
            yield return new WaitForSeconds(typingSpeed);
        }

        if (typingAnimator != null)
            typingAnimator.SetBool("IsTyping", false);

        isTalking = false;
    }

    private void PlayTextSound()
    {
        if (textSounds == null || textSounds.Length == 0 || audioSource == null)
            return;

        if (UnityEngine.Random.value < 0.3f)
        {
            int i = UnityEngine.Random.Range(0, textSounds.Length);
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(textSounds[i]);
        }
    }

    public void BeginScenario(ScenarioData scenario, GoalData goal)
    {
        currentScenario = scenario;
        currentGoal = goal;
        playerScore = 0;

        if (objectiveText != null)
            objectiveText.text = "Objective: " + goal.objective;

        if (clientText != null)
            clientText.text = scenario.clientBrief;

        SetupChoiceButtons(scenario.choices);
        resourceBar?.SetResourceScenario(scenario);
    }

    private void SetupChoiceButtons(ChoiceData[] choices)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i].choiceText;
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

    public void OnChoiceSelected(int choiceIndex)
    {
        StartCoroutine(OnChoiceSelectedRoutine(choiceIndex));
    }

    private IEnumerator OnChoiceSelectedRoutine(int choiceIndex)
    {
        //Disable buttons
        foreach (var btn in choiceButtons)
        {
            btn.interactable = false;
        }

        string playerChoice = choiceButtons[choiceIndex].GetComponentInChildren<TextMeshProUGUI>().text;

        //LLM response
        var llmTask = LLMService.SendChoiceAsync(currentScenario, playerChoice);
        yield return new WaitUntil(() => llmTask.IsCompleted);

        if (llmTask.IsFaulted)
        {
            Debug.LogError("LLM task failed: " + llmTask.Exception);
            yield break;
        }

        string jsonResponse = llmTask.Result;
        LLMResponse parsed = JsonUtility.FromJson<LLMResponse>(jsonResponse);

        //Log choice
        playerDecisions.Add(new ChoiceData
        {
            choiceText = playerChoice,
            reflection = parsed.reflection
        });

        //Update resources and score
        resourceBar?.AddValue(parsed.resourceImpact);
        GameManager.Instance.RegisterDecision(parsed.resourceImpact);
        if (MenuManager.Instance.CurrentStage == GameStage.Reflection) //If game is over
        {
            yield break; 
        }

        playerScore = GameManager.Instance.PlayerScore;
        if (scoreText != null) scoreText.text = playerScore.ToString();

        //Show dialogue response
        yield return StartCoroutine(TypeText(parsed.clientResponse, clientText));

        //Refresh available choices
        currentChoices = parsed.choices;
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (MenuManager.Instance.CurrentStage != GameStage.Reflection) //Don't assign new options if game is over
            {
                bool active = i < parsed.choices.Length;
                choiceButtons[i].gameObject.SetActive(active);
                if (active)
                    choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = parsed.choices[i];
            }
        }

        //Enable buttons if done talking
        foreach (var btn in choiceButtons)
        {
            btn.interactable = true;
        }
    }

    public IEnumerator BeginReflection()
    {
        reflectionTitle.text = GameManager.Instance.gameStatus;

        yield return StartCoroutine(TypeText(currentScenario.reflectionFeedback, clientText));

        yield return StartCoroutine(DisplayReflectionsSequentially(playerDecisions));
    }

    public IEnumerator DisplayReflectionsSequentially(List<ChoiceData> playerDecisions)
    {
        foreach (var decision in playerDecisions)
        {
            GameObject bubble = Instantiate(speechBubble, reflectionGrid);
            bubble.transform.localScale = Vector3.one;
            TextMeshProUGUI bubbleText = bubble.GetComponentInChildren<TextMeshProUGUI>();

            if (bubbleText != null)
            {
                string text = $"Choice: {decision.choiceText}\nReflection: {decision.reflection}";

                //Wait for last bubble to finish before starting next
                yield return StartCoroutine(TypeText(text, bubbleText));
            }
        }
    }
}
