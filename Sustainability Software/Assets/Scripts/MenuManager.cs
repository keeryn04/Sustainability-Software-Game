using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private ScenarioData[] scenarios;
    private ScenarioData pendingScenario;

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

    public void LoadScenarioScene(ScenarioData scenario, string sceneName = "PlayingScene")
    {
        pendingScenario = scenario; //Store scenario to pass later
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (DialogueManager.Instance != null && pendingScenario != null)
        {
            DialogueManager.Instance.InitializeScenario(pendingScenario);
            pendingScenario = null;
        }
        else
        {
            Debug.LogError("DialogueManager not found in scene or scenario is null.");
        }
    }

    public void LoadPillar(SustainabilityPillar pillar)
    {
        ScenarioData scenario = GetRandomByPillar(pillar);
        if (scenario != null)
        {
            LoadScenarioScene(scenario);
        }
        else
        {
            Debug.LogWarning($"No scenarios available for pillar {pillar}");
        }
    }

    //Wrappers to support enum
    public void LoadEnvironmental() => LoadPillar(SustainabilityPillar.Environmental);
    public void LoadSocial() => LoadPillar(SustainabilityPillar.Social);
    public void LoadEconomic() => LoadPillar(SustainabilityPillar.Economic);
    public void LoadTechnical() => LoadPillar(SustainabilityPillar.Technical);

    public ScenarioData GetRandomByPillar(SustainabilityPillar targetPillar)
    {
        var filtered = scenarios.Where(c => c.pillar == targetPillar).ToList();

        if (filtered.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, filtered.Count);
        return filtered[index];
    }
}
