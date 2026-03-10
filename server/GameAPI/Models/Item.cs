using System;

namespace GameAPI.Models;

public class Item
{
    public Guid Id { get; set; } = new Guid();
    public string? InventoryType { get; set; }
    public int? Position { get; set; }
    public int Level { get; set; }
    public int Health { get; set; }
    public Guid ItemTypeId { get; set; }
    public ItemType ItemType {get;set;}
    public Guid? GameUserId { get; set; }
    public GameUser? GameUser { get; set; }

}
