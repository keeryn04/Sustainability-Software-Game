using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechButton : MonoBehaviour
{
    [TextArea(2, 5)]
    [SerializeField] private string speechText;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private float appearanceDuration; //Disappears after x seconds

    private void Awake()
    {
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    private async void OnButtonClicked()
    {
        if (DialogueManager.Instance != null)
        {
            speechBubble.SetActive(true);
            await DialogueManager.Instance.TypeTextGeneral(speechText, textBox);
            Invoke(nameof(HideSpeechBubble), appearanceDuration);
        }
    }

    private void HideSpeechBubble()
    {
        speechBubble.SetActive(false);
    }
}