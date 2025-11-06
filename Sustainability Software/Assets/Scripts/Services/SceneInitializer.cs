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
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private Transform reflectionGrid;

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
    }

    private void SetupPlayingScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AssignUI(resourceBar);
            GameManager.Instance.StartScenario();
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AssignUI(
                clientText,
                objectiveText,
                scoreText,
                choiceButtons,
                resourceBar,
                typingAnimator,
                audioSource
            );
        }
    }

    private void SetupReflectionScene()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AssignReflectionUI(speechBubble, reflectionGrid);
            DialogueManager.Instance.BeginReflection(MenuManager.Instance.CurrentScenario);
        }
    }
}