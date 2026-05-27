using System.Collections.Generic;
using UnityEngine;

public class DresserManSc : LoaderBehaviour<DresserManSc>
{
    private Dictionary<string, DresserSc> dressers = new();

    public override List<System.Type> Dependencies => new()
    {
        typeof(InventoryManSc)
    };

    protected override void Load()
    {
        ReloadDressed();
    }

    public void ReloadDressed()
    {

        Debug.Log("attempt");
        dressers.Clear();

        var found = FindObjectsByType<DresserSc>(FindObjectsSortMode.None);

        foreach (var x in found)
        {
            Debug.Log("found: " + x.uniqueName);

            if (x == null || string.IsNullOrEmpty(x.uniqueName))
                continue;

            dressers[x.uniqueName] = x;
        }

        DressPlayer();
        DressEnemy();
    }

    public void DressPlayer()
    {
        if (!dressers.TryGetValue("Player", out var player))
            return;

        player.UnequipAll();

        if (!InventoryManSc.Instance.InventoryData.ContainsKey("PlayerEquiped"))
            return;

        List<string> items = new();

        foreach (var x in InventoryManSc.Instance.InventoryData["PlayerEquiped"])
        {
            if (x.Value == null)
                continue;

            if (string.IsNullOrEmpty(x.Value.name))
                continue;

            items.Add(x.Value.name);
        }

        player.Equip(items);
    }

    public async void DressEnemy()
    {
        if (!dressers.TryGetValue("Enemy", out var enemy))
            return;

        enemy.UnequipAll();

        string enemyId =
            MatchSession.MyPlayerId == MatchSession.CurrentMatch.PlayerOneId
                ? MatchSession.CurrentMatch.PlayerTwoId.ToString()
                : MatchSession.CurrentMatch.PlayerOneId.ToString();

        string json = await GameManagerSc.Instance.downloader.DownloadInventoryJsonAsyncHelper(enemyId);

        if (string.IsNullOrEmpty(json))
            return;

        string wrapped = "{\"items\":" + json + "}";

        ItemDataList list = JsonUtility.FromJson<ItemDataList>(wrapped);

        if (list?.items == null)
            return;

        List<string> items = new();

        foreach (var item in list.items)
        {
            if (item == null)
                continue;

            if (item.inventoryType != "PlayerEquiped")
                continue;

            if (string.IsNullOrEmpty(item.name))
                continue;

            items.Add(item.name);
        }

        enemy.Equip(items);
    }

    public void Equip(string uniqueName, string itemName)
    {
        if (dressers.TryGetValue(uniqueName, out var dresser))
        {
            dresser.Equip(itemName);
        }
    }

    public void Equip(string uniqueName, IEnumerable<string> items)
    {
        if (dressers.TryGetValue(uniqueName, out var dresser))
        {
            dresser.Equip(items);
        }
    }

    public void Unequip(string uniqueName, string itemName)
    {
        if (dressers.TryGetValue(uniqueName, out var dresser))
        {
            dresser.Unequip(itemName);
        }
    }

    public void UnequipAll(string uniqueName)
    {
        if (dressers.TryGetValue(uniqueName, out var dresser))
        {
            dresser.UnequipAll();
        }
    }
}