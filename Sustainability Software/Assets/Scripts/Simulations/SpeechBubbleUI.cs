using UnityEngine;
using TMPro;
using System.Collections;

public class SpeechBubbleUI : MonoBehaviour
{
    public static SpeechBubbleUI Instance;

    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TMP_Text speechText;

    private void Awake()
    {
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    public void ShowSpeech(string message)
    {
        speechBubble.SetActive(true);
        speechText.text = "";
    }

    public void HideSpeech()
    {
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }
}
