using System.Collections;
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
    public void LoadHearMore(string speechID, TextMeshProUGUI textBox, Button triggerButton = null)
    {
        StartCoroutine(LoadHearMoreRoutine(speechID, textBox, triggerButton));
    }

    private IEnumerator LoadHearMoreRoutine(string speechID, TextMeshProUGUI textBox, Button triggerButton = null)
    {
        if (hearMoreBubble == null || textBox == null) yield break;

        // Disable button while talking
        if (triggerButton != null)
            triggerButton.interactable = false;

        // Look up HearMore text
        HearMoreData data = hearMorePrefabs.Find(p => p.HearMoreName == speechID);
        if (data == null)
        {
            Debug.LogWarning($"HearMore ID '{speechID}' not found.");
            triggerButton.interactable = true;
            yield break;
        }

        hearMoreBubble.SetActive(true);

        yield return StartCoroutine(DialogueManager.Instance.TypeText(data.HearMoreText, textBox));

        // After typing finishes, wait remaining duration
        yield return new WaitForSeconds(hearDuration);

        hearMoreBubble.SetActive(false);

        // Re-enable button
        if (triggerButton != null)
            triggerButton.interactable = true;
    }

    public string GetHearMoreText(string speechID)
    {
        if (hearMorePrefabs == null) return "";

        HearMoreData data = hearMorePrefabs.Find(p => p.HearMoreName == speechID);
        if (data == null)
        {
            Debug.LogWarning($"HearMore ID '{speechID}' not found.");
            return "";
        }

        return data.HearMoreText;
    }
}