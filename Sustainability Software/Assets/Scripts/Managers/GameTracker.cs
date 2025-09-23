using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTracker : MonoBehaviour
{
    public static GameTracker Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private float playerScore = 0f;
    [SerializeField] private int decisionsMade = 0;

    [Header("Scenario Settings")]
    [SerializeField] private GoalData[] goals;
    private GoalData currentGoal;
    private float scoreThreshold;
    private float resourceThreshold;
    [SerializeField] private int decisionLimit;

    [Header("UI Elements")]
    [SerializeField] private ResourceBar resourceBar;

    //Getters
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

    public void AssignUI(ResourceBar newResourceBar)
    {
        resourceBar = newResourceBar;
    }

    public void StartGame()
    {
        decisionsMade = 0;
        playerScore = 0f;

        currentGoal = goals[UnityEngine.Random.Range(0, goals.Length)];
        scoreThreshold = currentGoal.pointThreshold;
        resourceThreshold = currentGoal.resourceThreshold;
    }

    public void RegisterDecision(float resourceImpact)
    {
        decisionsMade++;

        //Add points to score
        playerScore += Mathf.Max(0, resourceImpact * 1000);

        //Check if game ends
        if (decisionsMade > decisionLimit)
        {
            CheckEndConditions();
        }
    }

    private void CheckEndConditions()
    {
        if (currentGoal == null) return;

        bool success = currentGoal.goalType switch
        {
            GoalType.PointLevel => playerScore >= scoreThreshold,
            GoalType.ResourceLevel => resourceBar.GetValue() >= resourceThreshold,
            _ => false
        };

        MenuManager.Instance.ReflectionStatus = true;
        EndScenario(success);
    }

    private void EndScenario(Boolean status)
    {
        //float currentResourceValue = resourceBar.GetValue();
        Debug.Log(status ? "Success!" : "Fail");
        MenuManager.Instance.LoadReflectionScene();
    }
}
