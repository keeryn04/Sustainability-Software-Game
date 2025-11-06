using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HearMoreManager : MonoBehaviour
{
    public static HearMoreManager Instance { get; private set; }

    [Header("Hear More Settings")]
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
            hearMoreBubble.SetActive(false);
    }

    public async void LoadHearMore(string speechID, TextMeshProUGUI textBox, Button triggerButton = null)
    {
        if (hearMoreBubble == null || textBox == null) return;

        // Disable button while talking
        if (triggerButton != null)
            triggerButton.interactable = false;

        // Look up HearMore text
        HearMoreData data = hearMorePrefabs.Find(p => p.HearMoreName == speechID);
        if (data == null)
        {
            Debug.LogWarning($"HearMore ID '{speechID}' not found.");
            triggerButton.interactable = true;
            return;
        }

        hearMoreBubble.SetActive(true);

        await DialogueManager.Instance.TypeText(data.HearMoreText, textBox);

        // Wait for display duration
        await Task.Delay((int)(hearDuration * 1000));

        hearMoreBubble.SetActive(false);

        // Re-enable button
        if (triggerButton != null)
            triggerButton.interactable = true;
    }
}