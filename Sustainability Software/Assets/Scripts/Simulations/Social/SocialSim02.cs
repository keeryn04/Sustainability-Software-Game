using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocialSim02 : MonoBehaviour
{
    public enum DataCollection { Minimal, Moderate, Aggressive }

    [Header("Inputs")]
    [SerializeField] private Slider transparencySlider;
    [SerializeField] private Slider userControlSlider;

    [Header("Buttons")]
    [SerializeField] private Button minimalButton;
    [SerializeField] private Button moderateButton;
    [SerializeField] private Button aggressiveButton;

    [Header("Outputs")]
    [SerializeField] private Slider trustBar;
    [SerializeField] private Slider engagementBar;
    [SerializeField] private Slider reputationBar;

    private DataCollection currentMode = DataCollection.Minimal;

    void Start()
    {
        minimalButton.onClick.AddListener(() => SetMode(DataCollection.Minimal));
        moderateButton.onClick.AddListener(() => SetMode(DataCollection.Moderate));
        aggressiveButton.onClick.AddListener(() => SetMode(DataCollection.Aggressive));
    }

    void Update()
    {
        float transparency = transparencySlider.value;
        float control = userControlSlider.value;

        float trust = 0f;
        float engagement = 0f;
        float reputation = 0f;

        switch (currentMode)
        {
            case DataCollection.Minimal:
                trust = 0.8f + 0.1f * transparency + 0.1f * control;
                engagement = 0.5f;
                reputation = 0.9f;
                break;

            case DataCollection.Moderate:
                trust = 0.6f + 0.2f * transparency + 0.1f * control;
                engagement = 0.7f;
                reputation = 0.7f;
                break;

            case DataCollection.Aggressive:
                trust = 0.3f + 0.1f * transparency + 0.1f * control;
                engagement = 0.8f;
                reputation = 0.4f;
                break;
        }

        trustBar.value = Mathf.Clamp01(trust);
        engagementBar.value = Mathf.Clamp01(engagement);
        reputationBar.value = Mathf.Clamp01(reputation);
    }

    private void SetMode(DataCollection mode)
    {
        currentMode = mode;
    }
}
