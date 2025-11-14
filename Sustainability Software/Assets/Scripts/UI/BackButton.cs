using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    void OnEnable()
    {
        var backButton = this.GetComponent<Button>();
        if (backButton)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(MenuManager.Instance.LoadMenuScene);
        }
    }
}
