using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private string acceptedType;

    public void OnDrop(PointerEventData eventData)
    {
        var item = eventData.pointerDrag.GetComponent<DraggableObject>();
        if (item != null)
        {
            if (item.itemType == acceptedType)
            {
                Debug.Log($"Correctly sorted {item.name} into {acceptedType}");
                item.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            }
            else
            {
                Debug.Log($"Incorrectly sorted {item.name} into {acceptedType}");
            }
        }
    }
}
