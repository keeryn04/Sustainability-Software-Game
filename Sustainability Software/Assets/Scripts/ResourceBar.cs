using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image resourceBarImage; //Use an Image with Fill type set to "Horizontal"

    [Header("Values")]
    [Range(0f, 1f)][SerializeField] private float currentValue = 0.5f;
    [SerializeField] private float targetValue = 0f;

    [Header("Animation")]
    [SerializeField] private float fillSpeed = 2f;

    void Update()
    {
        if (resourceBarImage == null) return;

        //Smoothly move towards the target value
        if (Mathf.Abs(resourceBarImage.fillAmount - targetValue) > 0.001f)
        {
            resourceBarImage.fillAmount = Mathf.Lerp(
                resourceBarImage.fillAmount,
                targetValue,
                Time.deltaTime * fillSpeed
            );
        }
    }

    public void SetValue(float value)
    {
        targetValue = Mathf.Clamp01(value);
        currentValue = targetValue;
    }

    public void AddValue(float amount)
    {
        SetValue(currentValue + amount);
    }

    public void SetScenario(ScenarioData scenario)
    {
        if (scenario == null) return;

        //Swap the sprite if assigned
        if (scenario.resourceBarSprite != null && resourceBarImage != null)
        {
            resourceBarImage.sprite = scenario.resourceBarSprite;
        }
    }
}
