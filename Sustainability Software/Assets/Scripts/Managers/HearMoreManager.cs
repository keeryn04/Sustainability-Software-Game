using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HearMoreManager : MonoBehaviour
{
    public static HearMoreManager Instance { get; private set; }
    public bool isTalking { get; set; } = false;
    [SerializeField] private List<HearMoreData> hearMorePrefabs;
    [SerializeField] private GameObject hearMoreBubble;
    [SerializeField] private float hearDuration = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (hearMoreBubble != null)
        {
            hearMoreBubble.SetActive(false);
        }
    }

    //Called by SlideManager
    public async void LoadHearMore(string speechID, TextMeshProUGUI textBox, Button triggerButton = null)
    {
        hearMoreBubble.SetActive(true);
        triggerButton.interactable = false;
        HearMoreData hearMoreData = hearMorePrefabs
            .Find(p => p.HearMoreName == speechID);
        isTalking = true;
        await DialogueManager.Instance.TypeTextGeneral(hearMoreData.HearMoreText, textBox);
        Invoke(nameof(HideSpeechBubble), hearDuration);
        triggerButton.interactable = true;
        isTalking = false;
    }

    private void HideSpeechBubble() => hearMoreBubble.SetActive(false);
}