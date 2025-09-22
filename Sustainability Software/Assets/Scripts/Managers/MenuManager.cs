using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private ScenarioData[] scenarios;
    private ScenarioData currentScenario;

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

    //Getters
    public ScenarioData CurrentScenario => currentScenario;

    public void LoadScenarioScene(ScenarioData scenario, string sceneName = "PlayingScene")
    {
        if (scenario == null)
        {
            Debug.LogError("Scenario is null. Cannot load scene.");
            return;
        }

        PrepareScenario(scenario);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void PrepareScenario(ScenarioData scenario)
    {
        currentScenario = scenario;
        GameTracker.Instance.StartGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.SetupUI();
        }
        else
        {
            Debug.LogError("DialogueManager not found in scene.");
        }
    }

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
        return filtered.Length == 0 ? null : filtered[Random.Range(0, filtered.Length)];
    }

    //Helpers for UI reference
    public void LoadEnvironmental() => LoadPillar(SustainabilityPillar.Environmental);
    public void LoadSocial() => LoadPillar(SustainabilityPillar.Social);
    public void LoadEconomic() => LoadPillar(SustainabilityPillar.Economic);
    public void LoadTechnical() => LoadPillar(SustainabilityPillar.Technical);
}
