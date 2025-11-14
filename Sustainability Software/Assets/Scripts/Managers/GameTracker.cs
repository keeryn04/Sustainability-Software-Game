using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private float playerScore = 0f;
    [SerializeField] private int decisionsMade = 0;
    [SerializeField] public string gameStatus;
    private bool gameSuccess = false;

    [Header("Scenario Settings")]
    [SerializeField] private GoalData[] goals;
    private GoalData currentGoal;
    private float scoreThreshold;
    private float resourceThreshold;
    [SerializeField] private int decisionLimit;

    [Header("UI Elements")]
    [SerializeField] private ResourceBar resourceBar;

    public float PlayerScore => playerScore;
    public GoalData CurrentGoal => currentGoal;
    public float ScoreThreshold => scoreThreshold;
    public float ResourceThreshold => resourceThreshold;

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

    public void AssignUI(ResourceBar bar)
    {
        resourceBar = bar;
    }

    public void StartScenario()
    {
        decisionsMade = 0;
        playerScore = 0f;

        currentGoal = goals[UnityEngine.Random.Range(0, goals.Length)];
        scoreThreshold = currentGoal.pointThreshold;
        resourceThreshold = currentGoal.resourceThreshold;

        resourceBar?.SetResourceScenario(MenuManager.Instance.CurrentScenario);
        DialogueManager.Instance.BeginScenario(MenuManager.Instance.CurrentScenario, currentGoal);
    }

    public void RegisterDecision(float resourceImpact)
    {
        decisionsMade++;

        // Add points to score
        playerScore += Mathf.Max(0, resourceImpact * 1000);
        resourceBar?.AddValue(resourceImpact);

        // Check if scenario ends
        if (decisionsMade >= decisionLimit)
        {
            EndScenario();
        }
    }

    private void EndScenario()
    {
        if (currentGoal == null) return;

        gameSuccess = currentGoal.goalType switch
        {
            GoalType.PointLevel => playerScore >= scoreThreshold,
            GoalType.ResourceLevel => resourceBar != null && resourceBar.GetValue() >= resourceThreshold,
            _ => false
        };

        if (gameSuccess) { 
            gameStatus = "Scenario Success"; 
        } else { 
            gameStatus = "Scenario Fail"; 
        }

        Debug.Log(gameSuccess ? "Scenario Success!" : "Scenario Fail");

        //Transition to Reflection stage via MenuManager
        MenuManager.Instance.LoadReflection();
    }
}
