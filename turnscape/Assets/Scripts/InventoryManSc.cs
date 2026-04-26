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

    public override List<System.Type> Dependencies => new();

    private string json = "";

    protected override IEnumerator Download(CoroutineScope scope)
    {
        scope.Run(GameManagerSc.Instance.downloader.DownloadInventoryJson(r => json = r));
        yield break;
    }

    protected override void Load()
    {
        CollectInvetoryUI();
        LoadInventoryDataFromJson(json);
        ApplyInventoryDataToScene();
        SyncInventoryStructureFromScene();
        Debug.Log(json);
    }

    protected override void Prepare()
    {
        Debug.Log(json);
        json = BuildInventoryJson();
        Debug.Log("Build: " + json);
    }

    protected override IEnumerator Upload(CoroutineScope scope)
    {
        Debug.Log("Saving: " + json);
        var UpdatePosList = BuildUpdatePosDtos();
        scope.Run(GameManagerSc.Instance.downloader.UpdateInventoryPositions(UpdatePosList));
        yield break;
    }

    private void CollectInvetoryUI()
    {
        InventoryObjects.Clear();

        InitMiscInventory();
        CollectSceneInventories();
        AssignLooseSlots();
    }

    private void InitMiscInventory()
    {
        if (miscInv == null)
        {
            miscInv = new InventorySc();
            miscInv.uniqueName = "";
            miscInv.Slots.Clear();
        }

        InventoryObjects[""] = miscInv;

        if (!InventoryData.ContainsKey(""))
            InventoryData[""] = new Dictionary<string, ItemData>();
    }

    private void CollectSceneInventories()
    {
        var inventories = FindObjectsByType<InventorySc>(FindObjectsSortMode.None);

        foreach (var inv in inventories)
        {
            string invName = inv.uniqueName ?? "";

            InventoryObjects[invName] = inv;
            inv.Slots.Clear();

            RegisterSlots(inv, invName);
        }
    }

    private void RegisterSlots(InventorySc inv, string invName)
    {
        var slots = inv.GetComponentsInChildren<SlotSc>();

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];

            if (string.IsNullOrEmpty(slot.uniqueName))
                slot.uniqueName = i.ToString();

            slot.inventory = inv;
            inv.Slots[slot.uniqueName] = slot;
        }
    }

    private void AssignLooseSlots()
    {
        var allSlots = FindObjectsByType<SlotSc>(FindObjectsSortMode.None);

        foreach (var slot in allSlots)
        {
            if (slot.inventory != null)
                continue;

            slot.inventory = miscInv;

            if (!miscInv.Slots.ContainsKey(slot.uniqueName))
                miscInv.Slots[slot.uniqueName] = slot;
        }
    }

    private void PutItemInternal(ItemData item)
    {
        if (item == null)
            return;

        string invName = ResolveInventory(item.inventoryType);

        if (!InventoryObjects.ContainsKey(invName))
            return;

        var inv = InventoryObjects[invName];

        int index = item.position;
        int invSize = inv.Slots.Count;

        if (invSize == 0)
            return;

        for (int i = 0; i < invSize; i++)
        {
            string slotKey = index.ToString();

            if (inv.Slots.TryGetValue(slotKey, out var slot) && !slot.hasItem)
            {
                InventoryData[invName][slotKey] = item;
                slot.UpdateUI(FileReader.GetTextureSprite(item.category + ".png"));
                return;
            }

            index = (index + 1) % invSize;
        }
    }

    private string ResolveInventory(string invName)
    {
        if (string.IsNullOrEmpty(invName))
            return "";

        if (!InventoryObjects.ContainsKey(invName))
            return "";

        return invName;
    }

    public bool SwitchSlots(string inv0, string slot0, string inv1, string slot1, string cat0 = "", string cat1 = "")
    {
        inv0 ??= "";
        inv1 ??= "";

        if (!InventoryData.TryGetValue(inv0, out var slots0))
            return false;

        if (!InventoryData.TryGetValue(inv1, out var slots1))
            return false;

        if (!slots0.ContainsKey(slot0) || !slots1.ContainsKey(slot1))
            return false;

        if ((cat0 != "" && slots0[slot0].category != cat0) || (cat1 != "" && slots1[slot1].category != cat1))
            return false;

        (slots0[slot0], slots1[slot1]) = (slots1[slot1], slots0[slot0]);

        var inventory0 = InventoryObjects[inv0];
        var inventory1 = InventoryObjects[inv1];

        UpdateItemMeta(slots0[slot0], inventory0, slot0);
        UpdateItemMeta(slots1[slot1], inventory1, slot1);

        UpdateSlotUI(inventory0.Slots[slot0], inventory1.Slots[slot1]);

        return true;
    }

    public bool SwitchSlots(SlotSc slot0, SlotSc slot1)
    {
        if (slot0 == null || slot1 == null)
            return false;

        return SwitchSlots(
            slot0.inventoryName,
            slot0.uniqueName,
            slot1.inventoryName,
            slot1.uniqueName,
            slot0.category,
            slot1.category
        );
    }

    public bool SwitchSlots(SlotSc slot, InventorySc inv)
    {
        if (slot == null || inv == null)
            return false;

        foreach (var slot1 in inv.Slots.Values)
        {
            if (!slot1.hasItem)
                return SwitchSlots(slot, slot1);
        }

        return false;
    }

    public bool SwitchSlots(SlotSc[] slots, InventorySc inv)
    {
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
        if (string.IsNullOrEmpty(json))
            return;

        string wrapped = "{\"items\":" + json + "}";

        ItemDataList list = JsonUtility.FromJson<ItemDataList>(wrapped);

        if (list?.items == null)
            return;

        InventoryData.Clear();

        foreach (var item in list.items)
        {
            string invName = ResolveInventory(item.inventoryType);
            string slotKey = item.position.ToString();

            if (!InventoryData.ContainsKey(invName))
                InventoryData[invName] = new Dictionary<string, ItemData>();

            InventoryData[invName][slotKey] = item;
        }
    }

    private void ApplyInventoryDataToScene()
    {
        foreach (var invPair in InventoryData)
        {
            string invName = invPair.Key;

            if (!InventoryObjects.TryGetValue(invName, out InventorySc inv))
                continue;

            foreach (var slotPair in invPair.Value)
            {
                string slotKey = slotPair.Key;
                ItemData item = slotPair.Value;

                if (item == null) continue;
                if (!inv.Slots.TryGetValue(slotKey, out SlotSc slot)) continue;
                if (slot.hasItem) continue;

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
        if (!InventoryObjects.ContainsKey(""))
            InventoryObjects[""] = miscInv;

        foreach (var invPair in InventoryObjects)
        {
            string invName = invPair.Key;
            InventorySc inv = invPair.Value;

            if (!InventoryData.ContainsKey(invName))
                InventoryData[invName] = new Dictionary<string, ItemData>();

            SyncInventorySlots(inv, invName);
        }
    }

    private void SyncInventorySlots(InventorySc inv, string invName)
    {
        if (inv == null)
            return;

        if (!InventoryData.ContainsKey(invName))
            InventoryData[invName] = new Dictionary<string, ItemData>();

        foreach (var slotPair in inv.Slots)
        {
            string slotKey = slotPair.Key;

            if (string.IsNullOrEmpty(slotKey))
                continue;

            if (!InventoryData[invName].ContainsKey(slotKey))
                InventoryData[invName][slotKey] = new ItemData();
        }
    }

    public string BuildInventoryJson()
    {
        var list = new List<ItemData>();

        foreach (var invPair in InventoryData)
        {
            string invName = invPair.Key;

            if (invName == "")
                continue;

            foreach (var slotPair in invPair.Value)
            {
                var item = slotPair.Value;

                if (item == null)
                    continue;

                if (string.IsNullOrEmpty(item.id) &&
                    string.IsNullOrEmpty(item.itemType) &&
                    string.IsNullOrEmpty(item.category))
                {
                    continue;
                }

                list.Add(item);
            }
        }

        return JsonUtility.ToJson(new ItemDataList { items = list.ToArray() });
    }

    public List<UpdatePosDto> BuildUpdatePosDtos()
    {
        var result = new List<UpdatePosDto>();

        foreach (var invPair in InventoryData)
        {
            string invName = invPair.Key;

            if (invName == "") continue;

            foreach (var slotPair in invPair.Value)
            {
                ItemData item = slotPair.Value;

                if (item == null)
                    continue;

                if (string.IsNullOrEmpty(item.id))
                    continue;

                result.Add(new UpdatePosDto
                {
                    Id = item.id,
                    InventoryType = invName,
                    Position = item.position
                });
            }
        }

        return result;
    }
}