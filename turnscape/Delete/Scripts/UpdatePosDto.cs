using System;
using UnityEngine;


[System.Serializable]
public class UpdatePosDtoList
{
    public UpdatePosDto[] updated;
}

[Serializable]
public class UpdatePosDto
{
    public string Id { get; set; }
    public string InventoryType { get; set; }
    public int Position { get; set; }
}