using System.Collections.Generic;
using UnityEngine;

public class DresserSc : MonoBehaviour
{
    [System.Serializable]
    public class DressObject
    {
        public string name;
        public GameObject obj;
    }

    public string uniqueName;

    public List<DressObject> objects = new();

    private Dictionary<string, GameObject> map;

    private void Awake()
    {
        map = new Dictionary<string, GameObject>();

        foreach (var x in objects)
        {
            if (x == null || x.obj == null)
                continue;

            map[x.name] = x.obj;
        }
    }

    public void Equip(string itemName)
    {
        itemName = itemName.ToLower();

        if (string.IsNullOrEmpty(itemName))
            return;

        if (map.TryGetValue(itemName, out var obj))
        {
            obj.SetActive(true);
        }
    }

    public void Equip(IEnumerable<string> itemNames)
    {
        foreach (var x in itemNames)
        {
            Equip(x);
        }
    }

    public void Unequip(string itemName)
    {
        itemName = itemName.ToLower();

        if (string.IsNullOrEmpty(itemName))
            return;

        if (map.TryGetValue(itemName, out var obj))
        {
            obj.SetActive(false);
        }
    }

    public void Unequip(IEnumerable<string> itemNames)
    {
        foreach (var x in itemNames)
        {
            Unequip(x);
        }
    }

    public void UnequipAll()
    {
        foreach (var obj in map.Values)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}