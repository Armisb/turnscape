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
        dressers.Clear();

        var found = FindObjectsByType<DresserSc>(FindObjectsSortMode.None);

        foreach (var x in found)
        {
            if (x == null || string.IsNullOrEmpty(x.uniqueName))
                continue;

            dressers[x.uniqueName] = x;
        }
    }

    protected override void Apply()
    {
        ReloadDressed();
    }

    public void ReloadDressed()
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