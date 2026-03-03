using UnityEngine;
using UnityEngine.EventSystems;

public class SlotSc : MonoBehaviour, IDropHandler
{
    public Transform itemHolder;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem droppedItem = dropped.GetComponent<DraggableItem>();

        Transform existingItem = itemHolder.childCount > 0 ? itemHolder.GetChild(0) : null;
        existingItem?.SetParent(droppedItem.parentAfterDrag);

        droppedItem.parentAfterDrag = itemHolder;
    }
}
