using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected string itemDescription;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public Vector2 originalPosition;
    public bool isDraggable = true;
    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.ShowTooltip(itemDescription);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.HideTooltip();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void ResetToRandomPosition(RectTransform dropArea = null)
    {
        RectTransform area = dropArea != null ? dropArea : rectTransform.parent as RectTransform;

        if (area == null)
        {
            Debug.LogWarning("No valid area found for ResetToRandomPosition.");
            return;
        }

        float halfWidth = area.rect.width / 2f;
        float halfHeight = area.rect.height / 2f;

        float randomX = Random.Range(-halfWidth * 0.5f, halfWidth * 0.5f);
        float randomY = Random.Range(-halfHeight * 0.5f, halfHeight * 0.5f);

        rectTransform.anchoredPosition = new Vector2(randomX, randomY);
    }
}
