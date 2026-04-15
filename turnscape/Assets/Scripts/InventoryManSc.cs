using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class InventoryManSc : LoaderBehaviour<InventoryManSc>
{
    // InventoryName -> SlotName -> ItemData (null if empty)
    public Dictionary<string, Dictionary<string, ItemData>> InventoryData =
        new Dictionary<string, Dictionary<string, ItemData>>();

    // InventoryName -> InventorySc object in scene
    public Dictionary<string, InventorySc> InventoryObjects =
        new Dictionary<string, InventorySc>();

    public InventorySc miscInv;

    public override List<Type> Dependencies => new();

    public bool doNotRebuild = false;

    protected override void Load(string sceneName = "")
    {
        //RebuildSceneInventories();
    }

    protected override void SceneReload(string sceneName = "")
    {
        Debug.Log("Reload " + sceneName);

        if (!doNotRebuild)
        {
            BuildSceneInventories();
        }
        else
        {
            RebuildSceneInventories();
        }

        if (sceneName == "BaseScene") doNotRebuild = true;
    }

    protected override void Apply(string sceneName = "")
    {

    }

    public void BuildSceneInventories()
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

        InventorySc[] inventories = FindObjectsByType<InventorySc>(FindObjectsSortMode.None);

        foreach (var inv in inventories)
        {
            string invName = inv.uniqueName ?? "";

            InventoryObjects[invName] = inv;
            inv.Slots.Clear();

            if (!InventoryData.ContainsKey(invName))
                InventoryData[invName] = new Dictionary<string, ItemData>();

            SlotSc[] slots = inv.GetComponentsInChildren<SlotSc>();

            for (int i = 0; i < slots.Length; i++)
            {
                SlotSc slot = slots[i];

                if (slot.uniqueName == "") slot.uniqueName = i.ToString();
                slot.inventory = inv;

                inv.Slots[slot.uniqueName] = slot;

                if (!InventoryData[invName].ContainsKey(slot.uniqueName))
                {
                    InventoryData[invName][slot.uniqueName] = null;
                }
            }
        }

        SlotSc[] allSlots = FindObjectsByType<SlotSc>(FindObjectsSortMode.None);

        foreach (var slot in allSlots)
        {
            if (slot.inventory == null)
            {
                slot.inventory = miscInv;

                if (!miscInv.Slots.ContainsKey(slot.uniqueName))
                    miscInv.Slots.Add(slot.uniqueName, slot);

                if (!InventoryData[""].ContainsKey(slot.uniqueName))
                    InventoryData[""][slot.uniqueName] = null;
            }
        }

        if (InventoryObjects.ContainsKey("PlayerInventory"))
        {
            GameManagerSc.Instance.downloader.GetInventoriesFromServer(OnDataReceived);
        }
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


        InventorySc[] inventories = FindObjectsByType<InventorySc>(FindObjectsSortMode.None);

        foreach (var inv in inventories)
        {
            string invName = inv.uniqueName ?? "";

            InventoryObjects[invName] = inv;
            inv.Slots.Clear();

            /*if (!InventoryData.ContainsKey(invName))
                InventoryData[invName] = new Dictionary<string, ItemData>();*/

            SlotSc[] slots = inv.GetComponentsInChildren<SlotSc>();

            for (int i = 0; i < slots.Length; i++)
            {
                SlotSc slot = slots[i];

                if (slot.uniqueName == "") slot.uniqueName = i.ToString();
                slot.inventory = inv;

                inv.Slots[slot.uniqueName] = slot;

                if (InventoryData.ContainsKey(invName) && InventoryData[invName].ContainsKey(slot.uniqueName))
                {
                    PutItemInternal(InventoryData[invName][slot.uniqueName]);
                }
            }
        }

        SlotSc[] allSlots = FindObjectsByType<SlotSc>(FindObjectsSortMode.None);

        foreach (var slot in allSlots)
        {
            if (slot.inventory == null)
            {
                slot.inventory = miscInv;

                if (!miscInv.Slots.ContainsKey(slot.uniqueName))
                    miscInv.Slots.Add(slot.uniqueName, slot);

                if (InventoryData[""].ContainsKey(slot.uniqueName))
                {
                    PutItemInternal(InventoryData[""][slot.uniqueName]);
                }
            }
        }


    }

    void PutItemInternal(ItemData item)
    {
        if (item == null)
            return;

        string invName = item.inventoryType;

        if (string.IsNullOrEmpty(invName) || !InventoryObjects.ContainsKey(invName))
            invName = "PlayerInventory";

        InventorySc inv = InventoryObjects[invName];

        int index = item.position;

        int invSize = inv.Slots.Count;

        if (invSize == 0)
        {
            Debug.LogWarning($"Inventory {invName} has no slots.");
            return;
        }

        for (int i = 0; i < invSize; i++)
        {
            string slotKey = index.ToString();

            if (!inv.Slots.ContainsKey(slotKey))
            {
                index = (index + 1) % invSize;
                continue;
            }

            SlotSc slot = inv.Slots[slotKey];

            if (!slot.hasItem)
            {
                InventoryData[invName][slotKey] = item;

                slot.UpdateUI(FileReader.GetTextureSprite(item.category + ".png"));

                return;
            }

            index = (index + 1) % invSize;
        }

        Debug.LogWarning($"Inventory {invName} is full. Could not place item {item.id}");
    }

    public bool SwitchSlots(string inv0, string slot0, string inv1, string slot1)
    {
        inv0 ??= "";
        inv1 ??= "";

        if (!InventoryData.TryGetValue(inv0, out var slots0))
        {
            Debug.LogWarning($"SwitchSlots failed: Inventory '{inv0}' not found.");
            return false;
        }

        if (!InventoryData.TryGetValue(inv1, out var slots1))
        {
            Debug.LogWarning($"SwitchSlots failed: Inventory '{inv1}' not found.");
            return false;
        }

        if (!slots0.ContainsKey(slot0))
        {
            Debug.LogWarning($"SwitchSlots failed: Slot '{slot0}' not found in inventory '{inv0}'.");
            return false;
        }

        if (!slots1.ContainsKey(slot1))
        {
            Debug.LogWarning($"SwitchSlots failed: Slot '{slot1}' not found in inventory '{inv1}'.");
            return false;
        }

        ItemData item0 = slots0[slot0];
        ItemData item1 = slots1[slot1];

        slots0[slot0] = item1;
        slots1[slot1] = item0;

        InventorySc inventory0 = InventoryObjects[inv0];
        InventorySc inventory1 = InventoryObjects[inv1];

        if (item1 != null) item1.inventoryType = (inventory0 != null) ? inventory0.uniqueName : "";
        if (item0 != null) item0.inventoryType = (inventory1 != null) ? inventory1.uniqueName : "";

        SlotSc slotSc0 = inventory0.Slots[slot0];
        SlotSc slotSc1 = inventory1.Slots[slot1];

        if (item1 != null) item1.position = int.TryParse(slotSc0.uniqueName, out int pos1) ? pos1 : 0;
        if (item0 != null) item0.position = int.TryParse(slotSc1.uniqueName, out int pos0) ? pos0 : 0;

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

    private void OnDataReceived(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("Inventory JSON is null");
            return;
        }

        string wrappedJson = "{\"items\":" + json + "}";

        ItemDataList list = JsonUtility.FromJson<ItemDataList>(wrappedJson);

        if (list == null || list.items == null)
        {
            Debug.LogWarning("Failed to parse inventory JSON");
            return;
        }

        foreach (var item in list.items)
        {
            PutItemInternal(item);
        }
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


    public void PrintInventoryData()
    {
        foreach (var invPair in InventoryData)
        {
            string invName = invPair.Key;
            var slots = invPair.Value;

            Debug.Log($"Inventory: {invName}");

            foreach (var slotPair in slots)
            {
                string slotKey = slotPair.Key;
                ItemData item = slotPair.Value;

                if (item != null)
                {
                    Debug.Log($"  Slot {slotKey}: Cate = {item.category}, Pos = {item.position}");
                }
                else
                {
                    Debug.Log($"  Slot {slotKey}: null");
                }
            }
        }
    }
}