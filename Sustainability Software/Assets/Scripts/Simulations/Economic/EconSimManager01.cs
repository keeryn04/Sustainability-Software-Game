using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EconSimManager01 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private Button planButton;
    [SerializeField] private Button ignoreButton;

    private string[] stages = { "Development", "Maintenance", "Support", "Updates", "Decommissioning" };
    private Dictionary<string, bool> stagePlanned = new Dictionary<string, bool>();
    private int currentStageIndex = 0;

    private float baseCost = 1000f;

    void Start()
    {
        foreach (var stage in stages)
            stagePlanned[stage] = false; //default to ignored

        summaryText.gameObject.SetActive(false);

        ShowCurrentStage();
    }

    void ShowCurrentStage()
    {
        if (currentStageIndex < stages.Length)
        {
            stageText.text = $"Stage: {stages[currentStageIndex]}";
            feedbackText.text = "Choose to Plan or Ignore this stage.";
        }
        else
        {
            ShowSummary();
        }
    }

    public void ChoosePlan(bool plan)
    {
        string stage = stages[currentStageIndex];
        stagePlanned[stage] = plan;

        feedbackText.text = plan ? $"You planned {stage}. Future costs reduced." : $"You ignored {stage}. Future costs may rise.";

        currentStageIndex++;
        Invoke("ShowCurrentStage", 1.0f); //wait a second before moving to next stage
    }

    void ShowSummary()
    {
        float totalCost = baseCost;
        float sustainability = 100f;
        int ignoredCount = 0;

        foreach (var stage in stagePlanned)
        {
            if (!stage.Value)
            {
                totalCost *= 1.2f; //20% cost increase per ignored stage
                sustainability -= 15f;
                ignoredCount++;
            }
        }

        summaryText.gameObject.SetActive(true);
        summaryText.text = $"Simulation Complete!\nIgnored Stages: {ignoredCount}\nTotal Projected Cost: ${totalCost:F2}\nSustainability Score: {sustainability}%";

        //Hide stage selection buttons
        planButton.gameObject.SetActive(false);
        ignoreButton.gameObject.SetActive(false);
        stageText.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);
    }
}
