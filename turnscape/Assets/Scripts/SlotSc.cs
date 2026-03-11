using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotSc : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //public Image image;

    public string uniqueName;
    public InventorySc inventory;

    public Transform itemHolder;
    private Image itemImage;

    public static SlotSc dragSlot;
    public static SlotSc returnSlot;

    public InventoryManSc invMan => InventoryManSc.Instance;
    public string inventoryName => (inventory != null) ? inventory.uniqueName : "";
    public bool hasItem => itemHolder.gameObject.activeSelf;

    private void Awake()
    {
        itemImage = itemHolder.GetComponent<Image>();

        if (uniqueName == "DragSlot") dragSlot = this;

        UpdateUI(itemImage.sprite);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        returnSlot = this;

        if (dragSlot == null) dragSlot = GameManagerSc.Instance.miscCanvas.transform.GetChild(0).GetComponent<SlotSc>();

        invMan.SwitchSlots(this, dragSlot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragSlot == null) return;

        Vector3 mousePos = Input.mousePosition; 
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        RectTransform dragRect = dragSlot.GetComponent<RectTransform>();
        dragRect.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void OnDrop(PointerEventData eventData)
    {
        invMan.SwitchSlots(dragSlot, this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (invMan.SwitchSlots(dragSlot, returnSlot))
        {
            returnSlot = null;
        }

        if (dragSlot != null)
        {
            invMan.SwitchSlots(dragSlot, invMan.InventoryObjects["PlayerInventory"]);
        }

        StatisticsSc.Instance.RecalculateStats();
    }

    public void UpdateUI(Sprite icon = null)
    {
        if (icon != null)
        {
            itemImage.sprite = icon;
            itemHolder.gameObject.SetActive(true);
        }
        else
        {
            itemHolder.gameObject.SetActive(false);
            itemImage.sprite = null;
        }
    }

    public Sprite GetItemSprite()
    {
        return itemImage.sprite;
    }
}