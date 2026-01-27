using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SpeakerType
{
    Developer,
    Boss
}

[System.Serializable]
public class CutsceneLine
{
    public SpeakerType speaker;

    [TextArea(2, 4)]
    public string dialogue;
}

[System.Serializable]
public class TopicDialogue
{
    public string topic;

    [TextArea(2, 4)]
    public string bossLine1;

    [TextArea(2, 4)]
    public string developerLine1;

    [TextArea(2, 4)]
    public string bossLine2;

    [TextArea(2, 4)]
    public string developerLine2;
}
public class CutsceneManager : MonoBehaviour
{
    [Header("Intro Cutscene")]
    [SerializeField] private float cutscenePause = 0.5f;

    [Header("Topic Dialogue")]
    [SerializeField] private List<TopicDialogue> topicDialogues;

    [Header("TextUI")]
    [SerializeField] private TextMeshProUGUI bossText;
    [SerializeField] private TextMeshProUGUI developerText;

    [Header("Developers & Boss")]
    [SerializeField] private GameObject developer;
    [SerializeField] private Animator developerAnimator;
    [SerializeField] private AudioSource developerAudio;
    [SerializeField] private GameObject developerBubble;
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private AudioSource bossAudio;
    [SerializeField] private GameObject bossBubble;

    private string currentTopic;
    public void SetTopic(string topic)
    {
        currentTopic = topic;
    }
    public bool IsFinished { get; private set; }

    public IEnumerator PlayChallengeIntroCutscene()
    {
        IsFinished = false;

        bossText.text = "";
        developerText.text = "";

        developerBubble.SetActive(true);
        bossBubble.SetActive(true);

        var topicDialogue = GetDialogueForCurrentTopic();

        if (topicDialogue == null)
        {
            Debug.LogWarning($"No dialogue found for topic: {currentTopic}");
            IsFinished = true;
            yield break;
        }

        yield return PlayLine(
            SpeakerType.Boss,
            "Thanks for coming in, I have a couple issues with our system that I wanted to discuss."
        );

        yield return PlayLine(
            SpeakerType.Developer,
            "No problem! What's up?"
        );

        yield return PlayLine(
            SpeakerType.Boss,
            topicDialogue.bossLine1
        );

        yield return PlayLine(
            SpeakerType.Developer,
            topicDialogue.developerLine1
        );

        yield return PlayLine(
            SpeakerType.Boss,
            topicDialogue.bossLine2
        );

        yield return PlayLine(
            SpeakerType.Developer,
            topicDialogue.developerLine2
        );

        developerBubble.SetActive(false);
        bossBubble.SetActive(false);

        IsFinished = true;

        developerBubble.SetActive(false);
        bossBubble.SetActive(false);

        IsFinished = true;
    }
    private IEnumerator PlayLine(SpeakerType speaker, string dialogue)
    {
        TextMeshProUGUI currentTextBox;

        if (speaker == SpeakerType.Boss)
        {
            DialogueManager.Instance.AssignDeveloperUI(bossAnimator, bossAudio);
            currentTextBox = bossText;
            developerBubble.SetActive(false);
            bossBubble.SetActive(true);
        }
        else
        {
            DialogueManager.Instance.AssignDeveloperUI(developerAnimator, developerAudio);
            currentTextBox = developerText;
            developerBubble.SetActive(true);
            bossBubble.SetActive(false);
        }

        yield return StartCoroutine(
            DialogueManager.Instance.TypeText(dialogue, currentTextBox)
        );

        yield return new WaitForSeconds(cutscenePause);
    }
    private TopicDialogue GetDialogueForCurrentTopic()
    {
        return topicDialogues.Find(t => t.topic == currentTopic);
    }
}
