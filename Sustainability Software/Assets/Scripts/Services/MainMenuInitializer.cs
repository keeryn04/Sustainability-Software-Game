using UnityEngine;
using UnityEngine.UI;

public class MainMenuInitializer : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button environmentalPlayButton;
    [SerializeField] private Button socialPlayButton;
    [SerializeField] private Button economicPlayButton;
    [SerializeField] private Button technicalPlayButton;
    [SerializeField] private Button environmentalLearnButton;
    [SerializeField] private Button socialLearnButton;
    [SerializeField] private Button economicLearnButton;
    [SerializeField] private Button technicalLearnButton;
    [SerializeField] private Button generalLearnButton;


    private void Start()
    {
        //Clear any old listeners left over by Unity editor
        environmentalPlayButton.onClick.RemoveAllListeners();
        socialPlayButton.onClick.RemoveAllListeners();
        economicPlayButton.onClick.RemoveAllListeners();
        technicalPlayButton.onClick.RemoveAllListeners();
        environmentalLearnButton.onClick.RemoveAllListeners();
        socialLearnButton.onClick.RemoveAllListeners();
        economicLearnButton.onClick.RemoveAllListeners();
        technicalLearnButton.onClick.RemoveAllListeners();
        generalLearnButton.onClick.RemoveAllListeners();

        //Assign persistent MenuManager methods
        environmentalPlayButton.onClick.AddListener(MenuManager.Instance.PlayEnvironmentalScenario);
        socialPlayButton.onClick.AddListener(MenuManager.Instance.PlaySocialScenario);
        economicPlayButton.onClick.AddListener(MenuManager.Instance.PlayEconomicScenario);
        technicalPlayButton.onClick.AddListener(MenuManager.Instance.PlayTechnicalScenario);

        environmentalLearnButton.onClick.AddListener(MenuManager.Instance.LoadEnvironmentalLearn);
        socialLearnButton.onClick.AddListener(MenuManager.Instance.LoadSocialLearn);
        economicLearnButton.onClick.AddListener(MenuManager.Instance.LoadEconomicLearn);
        technicalLearnButton.onClick.AddListener(MenuManager.Instance.LoadTechnicalLearn);
        generalLearnButton.onClick.AddListener(MenuManager.Instance.LoadGeneralLearn);
    }
}
