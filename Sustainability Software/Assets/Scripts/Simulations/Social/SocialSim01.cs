using UnityEngine;
using UnityEngine.UI;

public class SocialSimulation : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private Slider timeSpentSlider;
    [SerializeField] private Slider meaningfulInteractionSlider;
    [SerializeField] private Slider accessibilityInclusionSlider;
    [SerializeField] private Slider communityModerationSlider;
    [SerializeField] private Slider userControlSlider;

    [Header("Outputs")]
    [SerializeField] private Slider happinessBar;
    [SerializeField] private Slider engagementBar;
    [SerializeField] private Slider trustBar;
    [SerializeField] private Slider communityHealthBar;

    private void Update()
    {
        float t = timeSpentSlider.value;
        float mi = meaningfulInteractionSlider.value;
        float a = accessibilityInclusionSlider.value;
        float c = communityModerationSlider.value;
        float u = userControlSlider.value;

        //Relationships
        float happiness = Mathf.Clamp01(0.5f * mi + 0.2f * a + 0.2f * u - 0.3f * t);
        float engagement = Mathf.Clamp01(0.5f * t + 0.4f * mi + 0.1f * a);
        float trust = Mathf.Clamp01(0.4f * u + 0.3f * c + 0.2f * mi - 0.2f * t);
        float communityHealth = Mathf.Clamp01(0.4f * c + 0.3f * mi + 0.2f * a - 0.2f * t);

        happinessBar.value = happiness;
        engagementBar.value = engagement;
        trustBar.value = trust;
        communityHealthBar.value = communityHealth;

        /*
        Time Spent boosts engagement but harms happiness, trust, and community.
        Meaningful Interaction improves nearly everything.
        Accessibility & Inclusion improves happiness and community health.
        Community Moderation raises trust and community health.
        User Control increases trust and happiness.
        */
    }
}
