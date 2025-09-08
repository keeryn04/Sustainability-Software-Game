using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Image fillImage; //Use an Image with Fill type set to "Horizontal"

    [Header("Values")]
    [Range(0f, 1f)] public float currentValue = 0f;
    private float targetValue = 0f;

    [Header("Animation")]
    public float fillSpeed = 2f;

    void Update()
    {
        //Smoothly move towards the target value
        if (Mathf.Abs(fillImage.fillAmount - targetValue) > 0.001f)
        {
            fillImage.fillAmount = Mathf.Lerp(
                fillImage.fillAmount,
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
}
