using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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

        //Subscribe to dialogue talking state
        DialogueManager.Instance.OnTalkingStateChanged += OnTalkingStateChanged;

        //Set initial state
        SetInputsInteractable(!DialogueManager.Instance.isTalking);
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnTalkingStateChanged -= OnTalkingStateChanged;
    }

    private void OnTalkingStateChanged(bool isTalking)
    {
        //Disable inputs while talking, enable when done
        SetInputsInteractable(!isTalking);
    }

    private void SetInputsInteractable(bool interactable)
    {
        inputField.interactable = interactable;
        sendButton.interactable = interactable;
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
        inputField.ActivateInputField();

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
        else if (MenuManager.Instance.CurrentStage == GameStage.Reflection)
        {
            List<ChoiceData> contextList = DialogueManager.Instance.PlayerDecisions;
            context = string.Join("\n", contextList.Select(d => $"Choice: {d.choiceText}\nReflection: {d.reflection}"));
            currentStage = GameStage.Reflection;
        }

        //Wait for bot response
        yield return StartCoroutine(GetBotResponseCoroutine(currentStage, userMessage, context, botTextBox));

        //Keep bubble visible for additional duration
        yield return new WaitForSeconds(waitDuration);
        speechBubble.SetActive(false);
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
        speechBubble.SetActive(false);

        Task<string> botTask = ChatService.SendChatAsync(currentStage, userMessage, context);

        //Wait until the task is done
        yield return new WaitUntil(() => botTask.IsCompleted);

        if (botTask.IsFaulted)
        {
            Debug.LogError("LLM task failed: " + botTask.Exception);
            yield break;
        }

        string botResponse = botTask.Result;

        speechBubble.SetActive(true);
        targetBox.text = "";

        yield return StartCoroutine(DialogueManager.Instance.TypeText(botResponse, targetBox));
    }
}