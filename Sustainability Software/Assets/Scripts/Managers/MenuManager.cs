using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//Marks what scene the game is currently on
public enum GameStage
{
    None,
    Menu,
    Learning,
    Playing,
    Reflection,
    Quiz
}
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    [SerializeField] private ScenarioData[] scenarios;

    private ScenarioData currentScenario;
    private GameStage currentStage = GameStage.None;
    private SustainabilityPillar currentPillar = SustainabilityPillar.General;

    public GameStage CurrentStage => currentStage;
    public ScenarioData CurrentScenario => currentScenario;
    public SustainabilityPillar CurrentPillar => currentPillar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLearning(SustainabilityPillar learnType)
    {
        currentPillar = learnType; //Store current pillar type
        currentStage = GameStage.Learning;

        SceneManager.sceneLoaded += OnSceneInitialized;
        SceneManager.LoadScene("LearningScene");
    }

    public void LoadPlaying(ScenarioData scenario)
    {
        if (scenario == null)
        {
            Debug.LogError("Cannot load null scenario.");
            return;
        }

        currentScenario = scenario;
        currentPillar = scenario.pillar; //Store current pillar type
        currentStage = GameStage.Playing;

        SceneManager.sceneLoaded += OnSceneInitialized;
        SceneManager.LoadScene("PlayingScene");
    }

    private ScenarioData RandomScenario(SustainabilityPillar pillar)
    {
        var filtered = scenarios.Where(s => s.pillar == pillar).ToArray();
        if (filtered.Length == 0)
        {
            Debug.LogWarning($"No scenarios found for pillar {pillar}");
            return null;
        }
        return filtered[UnityEngine.Random.Range(0, filtered.Length)];
    }

    public void LoadReflection()
    {
        currentStage = GameStage.Reflection;

        SceneManager.sceneLoaded += OnSceneInitialized;
        SceneManager.LoadScene("ReflectionScene");
    }

    public void LoadQuizScene()
    {
        currentStage = GameStage.Quiz;

        SceneManager.sceneLoaded += OnSceneInitialized;
        SceneManager.LoadScene("QuizScene");
    }

    public void LoadMenuScene()
    {
        //No menu movement if talking
        if (!DialogueManager.Instance.isTalking)
        {
            currentStage = GameStage.Menu;

            SceneManager.LoadScene("PillarSelectScene");
        } 
    }

    private void OnSceneInitialized(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneInitialized;

        SceneInitializer initializer = FindObjectOfType<SceneInitializer>();
        if (initializer != null)
        {
            initializer.InitializeScene(currentStage);
        }
        else
        {
            Debug.LogWarning("SceneInitializer not found in loaded scene.");
        }
    }

    public void LoadGeneralLearn() => LoadLearning(SustainabilityPillar.General);
    public void LoadEnvironmentalLearn() => LoadLearning(SustainabilityPillar.Environmental);
    public void LoadSocialLearn() => LoadLearning(SustainabilityPillar.Social);
    public void LoadEconomicLearn() => LoadLearning(SustainabilityPillar.Economic);
    public void LoadTechnicalLearn() => LoadLearning(SustainabilityPillar.Technical);
    public void PlayEnvironmentalScenario() => LoadPlaying(RandomScenario(SustainabilityPillar.Environmental));
    public void PlaySocialScenario() => LoadPlaying(RandomScenario(SustainabilityPillar.Social));
    public void PlayEconomicScenario() => LoadPlaying(RandomScenario(SustainabilityPillar.Economic));
    public void PlayTechnicalScenario() => LoadPlaying(RandomScenario(SustainabilityPillar.Technical));
    public void LoadQuiz() => LoadQuizScene();
}
