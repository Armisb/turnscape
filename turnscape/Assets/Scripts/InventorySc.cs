using System.Collections.Generic;
using UnityEngine;

public class InventorySc : MonoBehaviour
{
    public string uniqueName;

    public Dictionary<string, SlotSc> Slots = new Dictionary<string, SlotSc>();
}