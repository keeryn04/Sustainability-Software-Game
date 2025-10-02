using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    [Header("Optional UI Elements (assign per scene)")]
    public TextMeshProUGUI clientText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI scoreText;
    public Button[] choiceButtons;
    public ResourceBar resourceBar;
    public GameObject speechBubble;
    public Transform reflectionGrid;
    public Animator typingAnimator;
    public AudioSource audioSource;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeScene();
    }

    public void InitializeScene()
    {
        //Update DialogueManager with UI elements in this scene
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AssignUI(clientText, objectiveText, scoreText, choiceButtons, resourceBar, typingAnimator, audioSource);

            //Decide whether this is a reflection or scenario scene
            if (MenuManager.Instance.CurrentScenario != null && MenuManager.Instance.ReflectionStatus == false)
            {
                DialogueManager.Instance.SetupScenarioUI();
            }
            else
            {
                DialogueManager.Instance.AssignReflectionUI(speechBubble, reflectionGrid);
                DialogueManager.Instance.SetupReflectionUI();
            }
        }

        if (GameTracker.Instance != null)
        {
            GameTracker.Instance.AssignUI(resourceBar);
        }
    }
}