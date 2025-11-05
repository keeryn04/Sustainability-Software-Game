using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private ScenarioData[] scenarios;
    private ScenarioData currentScenario;
    private Boolean reflectionStatus = false;
    private SustainabilityPillar pendingLearnType;

    public static MenuManager Instance { get; private set; }
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

    //Setters & Getters
    public ScenarioData CurrentScenario => currentScenario;
    public bool ReflectionStatus
    {
        get => reflectionStatus;
        set => reflectionStatus = value;
    }

    public void LoadLearnScene(SustainabilityPillar learnType, string sceneName = "LearningScene")
    {
        // Store parameter so we can access it later
        pendingLearnType = learnType;

        // Subscribe to sceneLoaded before starting load
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Start loading the new scene
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Always unsubscribe right away to prevent duplicate triggers
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Now the scene is fully loaded, so SlideshowViewer should exist
        SlideshowViewer.Instance.SetSlideshow(pendingLearnType);
    }

    public void LoadScenarioScene(ScenarioData scenario, string sceneName = "PlayingScene")
    {
        if (scenario == null)
        {
            Debug.LogError("Scenario is null. Cannot load scene.");
            return;
        }

        currentScenario = scenario;
        GameTracker.Instance.StartGame();

        //Directly load the scene, SceneInitializer handles setup
        SceneManager.LoadScene(sceneName);
    }

    public void LoadReflectionScene(string sceneName = "ReflectionScene")
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("PillarSelectScene");
    }

    //Fetches random pillar based on pillar type, and feeds info back to scene
    public void LoadPillar(SustainabilityPillar pillar)
    {
        var scenario = GetRandomByPillar(pillar);
        if (scenario != null)
            LoadScenarioScene(scenario);
        else
            Debug.LogWarning($"No scenarios available for pillar {pillar}");
    }

    public ScenarioData GetRandomByPillar(SustainabilityPillar targetPillar)
    {
        var filtered = scenarios.Where(c => c.pillar == targetPillar).ToArray();
        return filtered.Length == 0 ? null : filtered[UnityEngine.Random.Range(0, filtered.Length)];
    }

    //Helpers for UI reference
    public void LoadEnvironmental() => LoadPillar(SustainabilityPillar.Environmental);
    public void LoadSocial() => LoadPillar(SustainabilityPillar.Social);
    public void LoadEconomic() => LoadPillar(SustainabilityPillar.Economic);
    public void LoadTechnical() => LoadPillar(SustainabilityPillar.Technical);

    public void LoadEnvironmentalLearn() => LoadLearnScene(SustainabilityPillar.Environmental);
    public void LoadSocialLearn() => LoadLearnScene(SustainabilityPillar.Social);
    public void LoadEconomicLearn() => LoadLearnScene(SustainabilityPillar.Economic);
    public void LoadTechnicalLearn() => LoadLearnScene(SustainabilityPillar.Technical);
}
