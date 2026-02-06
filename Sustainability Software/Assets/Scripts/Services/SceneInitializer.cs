using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    [Header("Playing Scene")]
    [SerializeField] private TextMeshProUGUI clientText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private ResourceBar resourceBar;

    [Header("Reflection Scene")]
    [SerializeField] private TextMeshProUGUI reflectionFeedbackText;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private Transform reflectionGrid;
    [SerializeField] private TextMeshProUGUI reflectionText;

    [Header("General")]
    [SerializeField] private Animator typingAnimator;
    [SerializeField] private AudioSource audioSource;

    public void InitializeScene(GameStage stage)
    {
        switch (stage)
        {
            case GameStage.Learning:
                SetupLearningScene();
                break;

            case GameStage.Playing:
                SetupPlayingScene();
                break;

            case GameStage.Reflection:
                SetupReflectionScene();
                break;

            case GameStage.Quiz:
                SetupQuizScene();
                break;

            case GameStage.Challenge:
                SetupChallengeScene();
                break;

            default:
                Debug.LogWarning("SceneInitializer called without a valid GameStage.");
                break;
        }
    }

    private void SetupLearningScene()
    {
        if (SlideshowViewer.Instance != null)
        {
            SlideshowViewer.Instance.SetSlideshow(MenuManager.Instance.CurrentPillar);
        }

        DialogueManager.Instance.AssignDeveloperUI(typingAnimator, audioSource);
    }

    private void SetupPlayingScene()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AssignUI(
                clientText,
                objectiveText,
                scoreText,
                choiceButtons,
                resourceBar
            );

            DialogueManager.Instance.AssignDeveloperUI(typingAnimator, audioSource);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AssignUI(resourceBar);
            GameManager.Instance.StartScenario();
        }
    }

    private void SetupReflectionScene()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManager.Instance is null!");
            return;
        }

        // Guard all UI references
        if (reflectionFeedbackText == null) Debug.LogWarning("reflectionFeedbackText is null!");
        if (speechBubble == null) Debug.LogWarning("speechBubble prefab is null!");
        if (reflectionGrid == null) Debug.LogWarning("reflectionGrid is null!");
        if (reflectionText == null) Debug.LogWarning("reflectionText is null!");

        DialogueManager.Instance.AssignReflectionUI(reflectionFeedbackText, speechBubble, reflectionGrid, reflectionText);
        DialogueManager.Instance.AssignDeveloperUI(typingAnimator, audioSource);

        if (reflectionFeedbackText != null && speechBubble != null && reflectionGrid != null && reflectionText != null)
        {
            StartCoroutine(DialogueManager.Instance.BeginReflection());
        }
        else
        {
            Debug.LogWarning("Cannot start reflection because UI references are missing!");
        }
    }

    private void SetupQuizScene()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AssignDeveloperUI(typingAnimator, audioSource);
        }
    }
    private void SetupChallengeScene()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AssignDeveloperUI(typingAnimator, audioSource);
        }
    }
}