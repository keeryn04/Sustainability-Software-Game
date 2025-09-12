using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI clientText;
    public Button[] choiceButtons;
    public ScenarioData currentScenario;
    private string[] currentChoices;
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

    public void LoadScenario(ScenarioData currentScenario)
    {
        //Initialize DialogueManager based on scenario
        clientText.text = currentScenario.clientBrief;
        currentChoices = new string[currentScenario.choices.Length];

        for (int i = 0; i < currentScenario.choices.Length; i++)
        {
            currentChoices[i] = currentScenario.choices[i].choiceText;
        }

        resourceBar.SetValue(0.5f);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentScenario.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentScenario.choices[i].choiceText;

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
        string playerChoice = currentChoices[choiceIndex];

        //Get LLM response
        string jsonResponse = await LLMService.SendChoiceAsync(currentScenario, playerChoice);
        LLMResponse parsed = JsonUtility.FromJson<LLMResponse>(jsonResponse);

        //Update resource bars with LLM's resourceImpact
        resourceBar.AddValue(parsed.resourceImpact);
        
        //Update choices
        currentChoices = parsed.choices;

        //Update UI with response and choices
        clientText.text = parsed.clientResponse;
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(i < parsed.choices.Length);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = parsed.choices[i];
        }
    }
}

[System.Serializable]
public class LLMResponse
{
    public string clientResponse;
    public string[] choices;
    public float resourceImpact;
}
