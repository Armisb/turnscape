using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class InventoryManSc : LoaderBehaviour<InventoryManSc>
{
    public Dictionary<string, Dictionary<string, ItemData>> InventoryData = new();
    public Dictionary<string, InventorySc> InventoryObjects = new();

    public InventorySc miscInv;

    public override List<Type> Dependencies => new();

    protected override void Load()
    {
        Debug.Log("[Inventory] Load START");

        CollectInvetoryUI();

        Debug.Log("[Inventory] Requesting server data...");
        GameManagerSc.Instance.downloader.GetInventoriesFromServer(OnDataReceived);
    }

    private void OnDataReceived(string json)
    {
        Debug.Log("[Inventory] Data received from server");

        LoadInventoryDataFromJson(json);
        ApplyInventoryDataToScene();
        SyncInventoryStructureFromScene();

        Debug.Log("[Inventory] Data applied to scene");
    }

    private void CollectInvetoryUI()
    {
        Debug.Log("[Inventory] CollectInvetoryUI");

        InventoryObjects.Clear();

        InitMiscInventory();
        CollectSceneInventories();
        AssignLooseSlots();

        Debug.Log($"[Inventory] UI collected. Inventories: {InventoryObjects.Count}");
    }

    private void InitMiscInventory()
    {
        Debug.Log("[Inventory] InitMiscInventory");

        if (miscInv == null)
        {
            miscInv = new InventorySc();
            miscInv.uniqueName = "";
            miscInv.Slots.Clear();

            Debug.Log("[Inventory] miscInv created");
        }

        InventoryObjects[""] = miscInv;

        if (!InventoryData.ContainsKey(""))
            InventoryData[""] = new Dictionary<string, ItemData>();
    }

    private void CollectSceneInventories()
    {
        Debug.Log("[Inventory] CollectSceneInventories");

        var inventories = FindObjectsByType<InventorySc>(FindObjectsSortMode.None);

        Debug.Log($"[Inventory] Found inventories: {inventories.Length}");

        foreach (var inv in inventories)
        {
            string invName = inv.uniqueName ?? "";

            Debug.Log($"[Inventory] Register inventory: {invName}");

            InventoryObjects[invName] = inv;
            inv.Slots.Clear();

            /*if (!InventoryData.ContainsKey(invName))
                InventoryData[invName] = new Dictionary<string, ItemData>();*/

            RegisterSlots(inv, invName);
        }
    }

    private void RegisterSlots(InventorySc inv, string invName)
    {
        var slots = inv.GetComponentsInChildren<SlotSc>();

        Debug.Log($"[Inventory] RegisterSlots {invName} slots: {slots.Length}");

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];

            if (string.IsNullOrEmpty(slot.uniqueName))
                slot.uniqueName = i.ToString();

            slot.inventory = inv;
            inv.Slots[slot.uniqueName] = slot;

            Debug.Log($"[Inventory] Slot registered {invName}:{slot.uniqueName}");
        }
    }

    private void AssignLooseSlots()
    {
        Debug.Log("[Inventory] AssignLooseSlots");

        var allSlots = FindObjectsByType<SlotSc>(FindObjectsSortMode.None);

        foreach (var slot in allSlots)
        {
            if (slot.inventory != null)
                continue;

            Debug.Log($"[Inventory] Loose slot assigned to misc: {slot.uniqueName}");

            slot.inventory = miscInv;

            if (!miscInv.Slots.ContainsKey(slot.uniqueName))
                miscInv.Slots[slot.uniqueName] = slot;

            /*if (!InventoryData[""].ContainsKey(slot.uniqueName))
                InventoryData[""][slot.uniqueName] = null;*/
        }
    }

    private void PutItemInternal(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Inventory] PutItemInternal: item is null");
            return;
        }

        string invName = ResolveInventory(item.inventoryType);

        if (!InventoryObjects.ContainsKey(invName))
        {
            Debug.LogWarning($"[Inventory] Inventory not found: {invName}");
            return;
        }

        var inv = InventoryObjects[invName];

        int index = item.position;
        int invSize = inv.Slots.Count;

        Debug.Log($"[Inventory] PutItem {item.id} into {invName}, start index {index}");

        if (invSize == 0)
        {
            Debug.LogWarning($"Inventory {invName} has no slots.");
            return;
        }

        for (int i = 0; i < invSize; i++)
        {
            string slotKey = index.ToString();

            if (inv.Slots.TryGetValue(slotKey, out var slot) && !slot.hasItem)
            {
                Debug.Log($"[Inventory] Placed item {item.id} at {invName}:{slotKey}");

                InventoryData[invName][slotKey] = item;
                slot.UpdateUI(FileReader.GetTextureSprite(item.category + ".png"));
                return;
            }

            index = (index + 1) % invSize;
        }

        Debug.LogWarning($"Inventory {invName} full. Item {item.id} not placed.");
    }

    private string ResolveInventory(string invName)
    {
        if (string.IsNullOrEmpty(invName))
            return "";

        if (!InventoryObjects.ContainsKey(invName))
        {
            Debug.LogWarning($"[Inventory] Unknown inventory '{invName}', fallback to ''");
            return "";
        }

        return invName;
    }

    public bool SwitchSlots(string inv0, string slot0, string inv1, string slot1)
    {
        Debug.Log($"[Inventory] SwitchSlots STRING {inv0}:{slot0} <-> {inv1}:{slot1}");

        inv0 ??= "";
        inv1 ??= "";

        if (!InventoryData.TryGetValue(inv0, out var slots0))
        {
            Debug.LogWarning($"SwitchSlots failed: inv0 missing {inv0}");
            return false;
        }

        if (!InventoryData.TryGetValue(inv1, out var slots1))
        {
            Debug.LogWarning($"SwitchSlots failed: inv1 missing {inv1}");
            return false;
        }

        if (!slots0.ContainsKey(slot0) || !slots1.ContainsKey(slot1))
        {
            Debug.LogWarning($"SwitchSlots failed: slot missing");
            return false;
        }

        (slots0[slot0], slots1[slot1]) = (slots1[slot1], slots0[slot0]);

        var inventory0 = InventoryObjects[inv0];
        var inventory1 = InventoryObjects[inv1];

        UpdateItemMeta(slots0[slot0], inventory0, slot0);
        UpdateItemMeta(slots1[slot1], inventory1, slot1);

        UpdateSlotUI(inventory0.Slots[slot0], inventory1.Slots[slot1]);

        Debug.Log("[Inventory] SwitchSlots SUCCESS");

        /*if (inv0 == "PlayerEquipped" || inv1 == "PlayerEquipped")
        {
            PrintInventoryData();
        }*/

        return true;
    }

    public bool SwitchSlots(SlotSc slot0, SlotSc slot1)
    {
        Debug.Log($"[Inventory] SwitchSlots SLOT {slot0?.uniqueName} <-> {slot1?.uniqueName}");

        if (slot0 == null || slot1 == null)
        {
            Debug.LogWarning("Null slot switch");
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
        Debug.Log($"[Inventory] SwitchSlots SLOT->INV {slot?.uniqueName}");

        if (slot == null || inv == null)
            return false;

        foreach (var slot1 in inv.Slots.Values)
        {
            if (!slot1.hasItem)
            {
                return SwitchSlots(slot, slot1);
            }
        }

        return false;
    }

    public bool SwitchSlots(SlotSc[] slots, InventorySc inv)
    {
        Debug.Log($"[Inventory] SwitchSlots ARRAY->INV {slots?.Length}");

        if (slots == null || inv == null)
            return false;

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

    private void LoadInventoryDataFromJson(string json)
    {
        Debug.Log("[Inventory] LoadInventoryDataFromJson");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("Inventory JSON is null");
            return;
        }

        string wrapped = "{\"items\":" + json + "}";

        ItemDataList list = JsonUtility.FromJson<ItemDataList>(wrapped);

        if (list?.items == null)
        {
            Debug.LogWarning("Failed to parse inventory JSON");
            return;
        }

        InventoryData.Clear();

        Debug.Log($"[Inventory] Items received: {list.items.Length}");

        foreach (var item in list.items)
        {
            string invName = ResolveInventory(item.inventoryType);
            string slotKey = item.position.ToString();

            if (!InventoryData.ContainsKey(invName))
                InventoryData[invName] = new Dictionary<string, ItemData>();

            InventoryData[invName][slotKey] = item;

            Debug.Log($"[Inventory] Stored {item.itemType} -> {invName}:{slotKey}");
        }
    }

    private void ApplyInventoryDataToScene()
    {
        Debug.Log("[Inventory] ApplyInventoryDataToScene");

        foreach (var invPair in InventoryData)
        {
            string invName = invPair.Key;

            if (!InventoryObjects.TryGetValue(invName, out InventorySc inv))
            {
                Debug.LogWarning($"Inventory object missing: {invName}");
                continue;
            }

            foreach (var slotPair in invPair.Value)
            {
                string slotKey = slotPair.Key;
                ItemData item = slotPair.Value;

                if (item == null) continue;

                if (!inv.Slots.TryGetValue(slotKey, out SlotSc slot)) continue;

                if (slot.hasItem) continue;

                Debug.Log($"[Inventory] Apply item {item.itemType} -> {invName}:{slotKey}");

                slot.UpdateUI(FileReader.GetTextureSprite(item.category + ".png"));
            }
        }
    }

    private void UpdateItemMeta(ItemData item, InventorySc inv, string slotKey)
    {
        if (item == null) return;

        item.inventoryType = inv?.uniqueName ?? "";
        item.position = int.TryParse(slotKey, out int pos) ? pos : 0;
    }

    private void UpdateSlotUI(SlotSc a, SlotSc b)
    {
        Sprite spriteA = a.GetItemSprite();
        Sprite spriteB = b.GetItemSprite();

        a.UpdateUI(spriteB);
        b.UpdateUI(spriteA);
    }
    private void SyncInventoryStructureFromScene()
    {
        Debug.Log("[Inventory] SyncInventoryStructureFromScene START");

        if (!InventoryObjects.ContainsKey(""))
        {
            Debug.LogWarning("[Inventory] WARNING: \"\" inventory missing in InventoryObjects! Creating fallback.");

            InventoryObjects[""] = miscInv;
        }
        else
        {
            Debug.Log("[Inventory] OK: \"\" inventory exists in InventoryObjects");
        }

        foreach (var invPair in InventoryObjects)
        {
            string invName = invPair.Key;
            InventorySc inv = invPair.Value;

            Debug.Log($"[Inventory] Sync inventory: '{invName}'");

            if (!InventoryData.ContainsKey(invName))
            {
                InventoryData[invName] = new Dictionary<string, ItemData>();
                Debug.Log($"[Inventory] Created InventoryData entry: {invName}");
            }

            SyncInventorySlots(inv, invName);
        }

        if (!InventoryObjects.ContainsKey(""))
        {
            Debug.LogWarning("[Inventory] CRITICAL: dragSlot (\"\") inventory STILL missing after sync!");
        }
        else
        {
            var dragInv = InventoryObjects[""];

            Debug.Log($"[Inventory] dragSlot inventory OK. Slots: {dragInv.Slots.Count}");

            if (dragInv.Slots.Count == 0)
            {
                Debug.LogWarning("[Inventory] WARNING: dragSlot inventory exists but has NO slots!");
            }
        }

        Debug.Log("[Inventory] SyncInventoryStructureFromScene DONE");
    }

    private void SyncInventorySlots(InventorySc inv, string invName)
    {
        if (inv == null)
        {
            Debug.LogWarning($"[Inventory] SyncInventorySlots NULL inventory: {invName}");
            return;
        }

        if (!InventoryData.ContainsKey(invName))
        {
            Debug.LogWarning($"[Inventory] InventoryData missing before slot sync: {invName}");
            InventoryData[invName] = new Dictionary<string, ItemData>();
        }

        foreach (var slotPair in inv.Slots)
        {
            string slotKey = slotPair.Key;

            if (string.IsNullOrEmpty(slotKey))
            {
                Debug.LogWarning($"[Inventory] INVALID slotKey in {invName}");
                continue;
            }

            if (invName == "")
            {
                Debug.Log($"[Inventory] Syncing dragSlot: {slotKey}");
            }

            if (!InventoryData[invName].ContainsKey(slotKey))
            {
                InventoryData[invName][slotKey] = new ItemData();

                Debug.Log($"[Inventory] Added missing slot: {invName}:{slotKey}");
            }
            else
            {
                Debug.Log($"[Inventory] Slot already exists: {invName}:{slotKey}");
            }
        }
    }
}