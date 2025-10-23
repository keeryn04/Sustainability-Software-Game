using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public string itemType;
    public string itemDescription;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.ShowTooltip(itemDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void ResetToRandomPosition(RectTransform dropArea = null)
    {
        RectTransform area = dropArea != null ? dropArea : rectTransform.parent as RectTransform;

        if (area == null)
        {
            Debug.LogWarning("No valid area found for ResetToRandomPosition.");
            return;
        }

        // Get random anchored position inside the bounds
        float halfWidth = area.rect.width / 2f;
        float halfHeight = area.rect.height / 2f;

        float randomX = Random.Range(-halfWidth * 0.5f, halfWidth * 0.5f);
        float randomY = Random.Range(-halfHeight * 0.5f, halfHeight * 0.5f);

        rectTransform.anchoredPosition = new Vector2(randomX, randomY);
    }
}
