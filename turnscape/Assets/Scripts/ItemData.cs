using System;
using UnityEngine;

public class ItemDataList
{
    public ItemData[] items;
}

[System.Serializable]
public class ItemData
{
    public string id;
    public string inventoryType;
    public int position;
    public int level;
    public int health;
    public string itemType;
    public string category;
    public int damage;
    public int protection;
}