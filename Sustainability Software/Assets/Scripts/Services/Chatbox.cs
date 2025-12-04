using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using System.Linq;
using System.Threading.Tasks;

public class ChatBox : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private TextMeshProUGUI botTextBox;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private float waitDuration = 2f;

    private void Awake()
    {
        sendButton.onClick.AddListener(OnSendClicked);
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    private void OnSendClicked()
    {
        StartCoroutine(SendMessageRoutine());
    }

    private IEnumerator SendMessageRoutine()
    {
        string userMessage = inputField.text.Trim();
        if (string.IsNullOrEmpty(userMessage))
            yield break;

        inputField.text = "";
        inputField.interactable = false;
        sendButton.interactable = false;

        GameStage currentStage = GameStage.None;
        string context = "";

        if (MenuManager.Instance.CurrentStage == GameStage.Learning)
        {
            context = GetHearMoreText();
            currentStage = GameStage.Learning;
        }
        else if (MenuManager.Instance.CurrentStage == GameStage.Playing)
        {
            context = MenuManager.Instance.CurrentScenario.clientBrief;
            currentStage = GameStage.Playing;
        }

        botTextBox.text = "";
        speechBubble.SetActive(true);

        //Wait for the bot to finish typing
        yield return StartCoroutine(GetBotResponseCoroutine(currentStage, userMessage, context, botTextBox));

        //Keep bubble visible for additional duration
        yield return new WaitForSeconds(waitDuration);

        speechBubble.SetActive(false);

        //Re-enable inputs
        inputField.interactable = true;
        sendButton.interactable = true;
        inputField.ActivateInputField();
    }
    private string GetHearMoreText()
    {
        var slide = SlideshowViewer.Instance.slideshow.slides[SlideshowViewer.Instance.CurrentSlideIndex];
        var hearMoreElement = slide.elements.FirstOrDefault(e => e.type == SlideData.InteractiveElement.ElementType.HearMore);
        if (hearMoreElement != null)
        {
            string context = HearMoreManager.Instance.GetHearMoreText(hearMoreElement.targetID);
            return context;
        }

        return "";
    }

    public IEnumerator GetBotResponseCoroutine(GameStage currentStage, string userMessage, string context, TextMeshProUGUI targetBox)
    {
        Task<string> botTask = ChatService.SendChatAsync(currentStage, userMessage, context);

        //Wait until the task is done
        yield return new WaitUntil(() => botTask.IsCompleted);

        if (botTask.IsFaulted)
        {
            Debug.LogError("LLM task failed: " + botTask.Exception);
            yield break;
        }

        string botResponse = botTask.Result;

        //Use your TypeText coroutine to animate the response
        yield return StartCoroutine(DialogueManager.Instance.TypeText(botResponse, targetBox));
    }
}
