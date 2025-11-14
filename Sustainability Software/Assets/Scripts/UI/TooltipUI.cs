using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance; //Singleton
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private RectTransform backgroundRect;

    private void Awake()
    {
        Instance = this;
        HideTooltip();
    }

    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}

