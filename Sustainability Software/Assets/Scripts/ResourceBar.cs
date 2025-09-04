using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI pillarLabel;
    public Image fillImage; //Use an Image with Fill type set to "Horizontal"

    [Header("Values")]
    [Range(0f, 100f)] public float currentValue = 50f;

    public void SetPillar(string pillarName)
    {
        pillarLabel.text = pillarName;
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp01(value);
        fillImage.fillAmount = currentValue;
    }

    public void AddValue(float amount)
    {
        SetValue(currentValue + amount);
    }
}
