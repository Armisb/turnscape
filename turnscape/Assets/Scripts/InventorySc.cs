using System.Collections.Generic;
using UnityEngine;

public class InventorySc : MonoBehaviour
{
    public string uniqueName;
    //public InventoryManSc invMan;

    public Dictionary<string, SlotSc> Slots = new Dictionary<string, SlotSc>();

    /*void Awake()
    {
        if (string.IsNullOrEmpty(uniqueName))
        {
            Debug.LogWarning($"InventorySc on GameObject '{gameObject.name}' does not have a uniqueName assigned.");
        }

        if (Slots.Count > 0)
        {
            Debug.LogWarning(
                $"Inventory '{uniqueName}' slots dictionary was not empty before Clear(). " +
                $"It contained {Slots.Count} elements."
            );
        }

        Slots.Clear();

        SlotSc[] foundSlots = GetComponentsInChildren<SlotSc>(true);

        for (int i = 0; i < foundSlots.Length; i++)
        {
            SlotSc slot = foundSlots[i];

            string newId = i.ToString();

            if (!string.IsNullOrEmpty(slot.uniqueName))
            {
                Debug.LogWarning(
                    $"Inventory '{uniqueName}': SlotSc '{slot.gameObject.name}' uniqueName " +
                    $"is being overwritten from '{slot.uniqueName}' to '{newId}'."
                );
            }

            slot.uniqueName = newId;
            slot.SetInventory(this);

            Slots[newId] = slot;
        }

        invMan.RegisterInventory(this);
    }*/
}