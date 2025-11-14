using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private string acceptedType;
    [SerializeField] private RectTransform dropArea;

    public void OnDrop(PointerEventData eventData)
    {
        var item = eventData.pointerDrag.GetComponent<GreenItem>();
        if (item != null)
        {
            if (item.itemType == acceptedType)
            {
                item.GetComponent<Image>().color = new Color32(175, 225, 175, 100);
            }
            else
            {
                item.ResetToRandomPosition(dropArea);
            }
        }
    }
}
