using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManSc : MonoBehaviour
{
    // Singleton instance
    public static InventoryManSc Instance { get; private set; }

    // InventoryName -> SlotName -> ItemData (null if empty)
    public Dictionary<string, Dictionary<string, ItemData>> InventoryData =
        new Dictionary<string, Dictionary<string, ItemData>>();

    // InventoryName -> InventorySc object in scene
    public Dictionary<string, InventorySc> InventoryObjects =
        new Dictionary<string, InventorySc>();

    private InventorySc miscInv;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        RebuildSceneInventories();
    }

    public void RebuildSceneInventories()
    {
        InventoryObjects.Clear();

        if (miscInv == null)
        {
            miscInv = new InventorySc();
            miscInv.uniqueName = "";
            miscInv.Slots.Clear();
        }
        InventoryObjects.Add("", miscInv);

        if (!InventoryData.ContainsKey(""))
            InventoryData[""] = new Dictionary<string, ItemData>();

        SlotSc[] slots = FindObjectsByType<SlotSc>(FindObjectsSortMode.None);

        foreach (var slot in slots)
        {
            RegisterSlotInternal(slot);
        }

        GetInventoriesFromServer();

        //ApplyInventoryData();
    }

    void RegisterSlotInternal(SlotSc slot)
    {
        InventorySc parentInv = null;

        if (slot.transform.parent != null)
            parentInv = slot.transform.parent.GetComponent<InventorySc>();

        // scene inventory
        if (parentInv != null)
        {
            string invName = parentInv.uniqueName ?? "";

            if (!InventoryObjects.ContainsKey(invName))
            {
                InventoryObjects[invName] = parentInv;
                parentInv.Slots.Clear();

                if (!InventoryData.ContainsKey(invName))
                    InventoryData[invName] = new Dictionary<string, ItemData>();
            }

            slot.uniqueName = parentInv.Slots.Count.ToString();

            slot.inventory = parentInv;

            if (!parentInv.Slots.ContainsKey(slot.uniqueName))
                parentInv.Slots.Add(slot.uniqueName, slot);

            if (!InventoryData[invName].ContainsKey(slot.uniqueName))
                InventoryData[invName][slot.uniqueName] = null;
        }

        // misc inventory
        else
        {
            slot.inventory = miscInv;

            if (!miscInv.Slots.ContainsKey(slot.uniqueName))
                miscInv.Slots.Add(slot.uniqueName, slot);

            if (!InventoryData[""].ContainsKey(slot.uniqueName))
                InventoryData[""][slot.uniqueName] = null;
        }
    }

    public bool SwitchSlots(string inv0, string slot0, string inv1, string slot1)
    {
        inv0 ??= "";
        inv1 ??= "";

        if (!InventoryData.TryGetValue(inv0, out var slots0)) return false;
        if (!InventoryData.TryGetValue(inv1, out var slots1)) return false;

        if (!slots0.ContainsKey(slot0)) return false;
        if (!slots1.ContainsKey(slot1)) return false;

        ItemData item0 = slots0[slot0];
        ItemData item1 = slots1[slot1];

        slots0[slot0] = item1;
        slots1[slot1] = item0;

        InventorySc inventory0 = InventoryObjects[inv0];
        InventorySc inventory1 = InventoryObjects[inv1];

        SlotSc slotSc0 = inventory0.Slots[slot0];
        SlotSc slotSc1 = inventory1.Slots[slot1];

        Sprite sprite0 = slotSc0.GetItemSprite();
        Sprite sprite1 = slotSc1.GetItemSprite();

        slotSc0.UpdateUI(sprite1);
        slotSc1.UpdateUI(sprite0);

        return true;
    }

    public bool SwitchSlots(SlotSc slot0, SlotSc slot1)
    {
        if (slot0 == null || slot1 == null)
        {
            Debug.LogWarning($"Null slot switch: {slot0} or {slot1}");
            return false;
        }

        return SwitchSlots(
            slot0.inventoryName,
            slot0.uniqueName,
            slot1.inventoryName,
            slot1.uniqueName
        );
    }

    public bool SwitchSlots(SlotSc slot, InventorySc inv)
    {
        if (slot == null || inv == null)
        {
            Debug.LogWarning($"Null slot or inventory switch: {slot} or {inv}");
            return false;
        }

        foreach (var slot1 in inv.Slots.Values)
        {
            if (!slot1.hasItem)
            {
                SwitchSlots(slot, slot1);
                return true;
            }
        }

        return false;
    }

    public bool SwitchSlots(SlotSc[] slots, InventorySc inv)
    {
        if (slots == null || inv == null)
        {
            Debug.LogWarning($"Null slot array or inventory switch");
            return false;
        }

        int k = 0;

        foreach (var slot1 in inv.Slots.Values)
        {
            if (k >= slots.Length)
                break;

            if (!slot1.hasItem)
            {
                SwitchSlots(slots[k], slot1);
                k++;
            }
        }

        return k == slots.Length;
    }

    public void GetInventoriesFromServer()
    {
        //GameManagerSc.Instance.downloader.GetInventoriesFromServer();
    }

    public ItemData GetItemData(string inventoryName, string slotName)
    {
        inventoryName ??= "";

        if (!InventoryData.TryGetValue(inventoryName, out var slots))
            return null;

        if (!slots.TryGetValue(slotName, out var item))
            return null;

        return item;
    }

    void ApplyInventoryData()
    {
        foreach (var invPair in InventoryObjects)
        {
            string invName = invPair.Key;
            InventorySc inv = invPair.Value;

            if (!InventoryData.TryGetValue(invName, out var slots))
                continue;

            foreach (var slotPair in slots)
            {
                if (!inv.Slots.TryGetValue(slotPair.Key, out var slot))
                    continue;

                ItemData item = slotPair.Value;

                slot.UpdateUI((item != null) ? FileReader.GetTextureSprite(item.name) : null);
            }
        }
    }
}