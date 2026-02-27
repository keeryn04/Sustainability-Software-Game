using System;
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
    [SerializeField] private CutsceneManager cutsceneController;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> strategyButtons;
    [SerializeField] private List<Button> developerButtons;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI developerText;
    [SerializeField] private TextMeshProUGUI developerTitleText;
    [SerializeField] private Slider developerBar;
    [SerializeField] private Slider bossBar;

    [Header("Objects")]
    [SerializeField] private GameObject developer;
    [SerializeField] private Animator developerAnimator;
    [SerializeField] private AudioSource developerAudio;
    [SerializeField] private GameObject developerBubble;
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private AudioSource bossAudio;
    [SerializeField] private GameObject bossBubble;

    [Header("Health Settings")]
    [SerializeField] private float bossDamage = 25f;
    [SerializeField] private float smallPlayerDamage = 20f;
    [SerializeField] private float bigPlayerDamage = 30f;

    [Header("Adjustables")]
    [SerializeField] private int hearDuration = 3;
    [SerializeField] private float barFillSpeed = 0.3f;

    public List<DeveloperMapping> developers;
    private Dictionary<string, SustainabilityPillar> developerDict;
    private ChallengeService.Challenge currentChallenge;

    private string selectedStrategyId;
    private string selectedDeveloper;
    private int currentDeveloperIndex = 0;
    private int currentQuestionIndex = 0;
    private float maxDeveloperHealth = 100f;
    private float maxBossHealth = 100f;

    private bool challengeActive = true;
    private bool quizLoaded = false;
    private bool quizFailed = false;

    [Header("Topics")]
    [SerializeField] private Dictionary<SustainabilityPillar, string> topics = new Dictionary<SustainabilityPillar, string>
    {
        { SustainabilityPillar.Environmental, "Reducing energy consumption in software systems"},
        { SustainabilityPillar.Technical, "Maintainable and scalable software architectures"},
        { SustainabilityPillar.Economic, "Cost efficiency and resource optimization in software"},
        { SustainabilityPillar.Social, "Accessibility and user well-being in software"}
    };

    private string currentTopic;

    private async void Start()
    {
        //Choose random topic
        currentTopic = ChooseRandomTopic();
        cutsceneController.SetTopic(currentTopic);

        //Turn off buttons for cutscene
        foreach (var btn in developerButtons)
        {
            btn.interactable = false;
        }

        foreach (var btn in strategyButtons)
        {
            btn.interactable = false;
        }

        submitButton.interactable = false;

        //Start cutscene
        StartCoroutine(cutsceneController.PlayChallengeIntroCutscene());

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

        SetActiveDeveloper(0);

        //Load quiz
        try
        {
            currentChallenge = await ChallengeService.GenerateChallengeAsync(currentTopic, 5);
            if (currentChallenge == null || currentChallenge.questions.Count == 0)
                quizFailed = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            quizFailed = true;
        }
        finally
        {
            quizLoaded = true;
        }

        DialogueManager.Instance.OnTalkingStateChanged += HandleTalkingStateChanged;

        //Set health stats
        developerBar.maxValue = maxDeveloperHealth;
        bossBar.maxValue = maxBossHealth;
        developerBar.value = maxDeveloperHealth;
        bossBar.value = maxBossHealth;

        StartCoroutine(WaitForIntroAndStart());
    }

    private IEnumerator WaitForIntroAndStart()
    {
        yield return new WaitUntil(() =>
            cutsceneController.IsFinished && quizLoaded
        );

        if (quizFailed)
        {
            questionText.text = "Actually, I'm having some trouble coming up with questions right now. Could we try again later?";
            yield break;
        }

        StartCoroutine(DisplayQuestion());
    }

    private void OnDisable()
    {
        DialogueManager.Instance.OnTalkingStateChanged -= HandleTalkingStateChanged;
    }

    private void HandleTalkingStateChanged(bool isTalking)
    {
        //Not available during cutscene
        if (cutsceneController.IsFinished)
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
    }
    private string ChooseRandomTopic()
    {
        List<SustainabilityPillar> pillarsVisited = MenuManager.Instance.PillarsVisited;
        SustainabilityPillar chosenPillar;

        if (pillarsVisited.Count > 0)
        {
            chosenPillar = pillarsVisited[UnityEngine.Random.Range(0, pillarsVisited.Count)];
        } else
        {
            chosenPillar = SustainabilityPillar.Environmental; //Default to environmental if no lessons visited
        }

        string topic = topics[chosenPillar];

        return topic;
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

    private IEnumerator DisplayQuestion()
    {
        developerText.text = "";
        var question = currentChallenge.questions[currentQuestionIndex];


        DialogueManager.Instance.AssignDeveloperUI(bossAnimator, bossAudio);
        bossBubble.SetActive(true);
        yield return StartCoroutine(DialogueManager.Instance.TypeText(question.bossQuestion, questionText));

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
        DialogueManager.Instance.AssignDeveloperUI(developerAnimator, developerAudio);

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
        var question = currentChallenge.questions[currentQuestionIndex];

        bool developerCorrect = selectedDeveloper.ToLower() == question.correctDeveloper.ToLower();
        bool strategyCorrect = selectedStrategyId.ToLower() == question.correctStrategyId.ToLower();

        yield return StartCoroutine(ApplyOutcome(developerCorrect, strategyCorrect));

        if (challengeActive)
        {
            developerText.text = "";
            developerBubble.SetActive(false);

            DialogueManager.Instance.AssignDeveloperUI(bossAnimator, bossAudio);

            if (developerCorrect && strategyCorrect)
            {
                yield return StartCoroutine(DialogueManager.Instance.TypeText(
                    "Great Idea! " + question.explanation,
                    questionText
                ));
            }
            else
            {
                string feedback = "I'm not sure on that. ";

                if (!developerCorrect && !strategyCorrect)
                    feedback += "I think a different developer and idea could fit for this. ";
                else if (!developerCorrect)
                    feedback += "I think a different developer may be more fit for this question. ";
                else
                    feedback += "Maybe a different strategy could work better here. ";

                feedback += question.explanation;

                yield return StartCoroutine(DialogueManager.Instance.TypeText(feedback, questionText));
            }

            yield return new WaitForSeconds(hearDuration);

            foreach (var btn in strategyButtons)
                btn.interactable = true;

            yield return StartCoroutine(NextQuestion());
        }
    }

    private IEnumerator ApplyOutcome(bool developerCorrect, bool strategyCorrect)
    {
        float newDeveloperHealth = developerBar.value; 
        float newBossHealth = bossBar.value; 
        
        //Correct Developer, Correct Attack
        if (developerCorrect && strategyCorrect) { newBossHealth -= bossDamage; } 
        //Correct Developer, Correct Defend
        else if (developerCorrect && !strategyCorrect) { newDeveloperHealth -= smallPlayerDamage; } 
        //Incorrect Developer, Correct Attack or Defend
        else if (!developerCorrect && strategyCorrect) { newDeveloperHealth -= smallPlayerDamage; } 
        //Incorrect Developer, Incorrect Attack or Defend
        else { newDeveloperHealth -= bigPlayerDamage; } 
        
        newDeveloperHealth = Mathf.Clamp(newDeveloperHealth, 0, maxDeveloperHealth); 
        newBossHealth = Mathf.Clamp(newBossHealth, 0, maxBossHealth); 

        StartCoroutine(SmoothFill(developerBar, newDeveloperHealth));
        StartCoroutine(SmoothFill(bossBar, newBossHealth));

        if (newDeveloperHealth <= 0)
        {
            //Failed challenge
            challengeActive = false;
            yield return StartCoroutine(DialogueManager.Instance.TypeText("I appreciate your help, but I think I might ask another developer for some help. Thanks anyways!", questionText));
            yield return new WaitForSeconds(hearDuration);

            bossBubble.SetActive(false);
        }
    }

    private IEnumerator NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex < currentChallenge.questions.Count)
        {
            StartCoroutine(DisplayQuestion());
        }
        else
        {
            //Finished challenge
            yield return StartCoroutine(DialogueManager.Instance.TypeText("Great work! Thanks for your help!", questionText));

            foreach (var btn in strategyButtons)
                btn.interactable = false;

            foreach (var btn in developerButtons)
                btn.interactable = false;

            submitButton.interactable = false;

            yield return new WaitForSeconds(hearDuration);

            bossBubble.SetActive(false);
        }
    }

    IEnumerator SmoothFill(Slider targetBar, float targetValue)
    {
        float startValue = targetBar.value;
        float elapsed = 0f;

        while (elapsed < barFillSpeed)
        {
            elapsed += Time.deltaTime;
            targetBar.value = Mathf.Lerp(startValue, targetValue, elapsed / barFillSpeed);
            yield return null;
        }

        targetBar.value = targetValue;
    }
}
