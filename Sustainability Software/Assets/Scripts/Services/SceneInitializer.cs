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

            default:
                Debug.LogWarning("SceneInitializer called without a valid GameStage.");
                break;
        }
    }

    private void SetupLearningScene()
    {
        if (SlideshowViewer.Instance != null)
        {
            SlideshowViewer.Instance.SetSlideshow(MenuManager.Instance.PendingLearnType);
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
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AssignReflectionUI(reflectionFeedbackText, speechBubble, reflectionGrid, reflectionText);
            DialogueManager.Instance.BeginReflection(MenuManager.Instance.CurrentScenario);
        }
    }
}