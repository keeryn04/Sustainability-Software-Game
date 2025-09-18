using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTracker : MonoBehaviour
{
    public static GameTracker Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private float playerScore = 0f;
    [SerializeField] private int decisionsMade = 0;

    [Header("Scenario Settings")]
    [SerializeField] private GoalData[] goals;
    [SerializeField] private GoalData currentGoal;
    [SerializeField] private float scoreThreshold;
    [SerializeField] private float resourceThreshold;
    [SerializeField] private int decisionLimit;

    [Header("In-Engine Assignments")]
    [SerializeField] private ResourceBar resourceBar;


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

    public void StartGame()
    {
        decisionsMade = 0;
        playerScore = 0f;

        currentGoal = goals[UnityEngine.Random.Range(0, goals.Length)];
        DialogueManager.Instance.SetObjectiveText(currentGoal.objective);
        scoreThreshold = currentGoal.pointThreshold;
        resourceThreshold = currentGoal.resourceThreshold;
    }

    public void RegisterDecision(float resourceImpact)
    {
        decisionsMade++;

        //Add points to score
        playerScore += Mathf.Max(0, resourceImpact * 100);
        DialogueManager.Instance.SetPlayerScore(playerScore);

        //Check if game ends
        if (decisionsMade > decisionLimit)
        {
            CheckEndConditions();
        }
    }

    private void CheckEndConditions()
    {
        if (currentGoal == null) return; //no goal, nothing to check

        bool success = false;

        switch (currentGoal.goalType)
        {
            case GoalType.PointLevel:
                success = playerScore >= scoreThreshold;
                break;
            case GoalType.ResourceLevel:
                success = resourceBar.GetValue() >= resourceThreshold;
                break;
            //More goal types here
            default:
                Debug.LogWarning("Unknown goal type: " + currentGoal.goalType);
                return;
        }

        EndScenario(success);
    }

    private void EndScenario(Boolean status)
    {
        if (status == false)
        {
            Debug.Log("Fail");
        } else
        {
            Debug.Log("Success!");
        }
    }
}
