using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI clientText;
    public Button[] choiceButtons;
    public ScenarioData currentScenario;
    public ResourceBar resourceBar;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        LoadScenario(currentScenario);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Persist across scenes
        }
    }

    public void LoadScenario(ScenarioData scenario)
    {
        currentScenario = scenario;
        clientText.text = scenario.clientBrief;
        resourceBar.SetValue(0.5f);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < scenario.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = scenario.choices[i].choiceText;

                int choiceIndex = i; //capture index for closure
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private async void OnChoiceSelected(int choiceIndex)
    {
        var choice = currentScenario.choices[choiceIndex];

        //Update resource bars first
        resourceBar.AddValue(choice.resourceReaction);

        //Call LLM for dynamic client response
        string llmResponse = await LLMService.SendChoiceAsync(currentScenario, choice);
        clientText.text = llmResponse;
    }
}
