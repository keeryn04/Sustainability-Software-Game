using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Token : DraggableObject
{
    public float fairnessEffect;
    public float profitEffect;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ResetToken()
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.anchoredPosition = originalPosition;

        CanvasGroup cg = this.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = true;
    }
}
