using System;

namespace GameAPI.Models;

public abstract class ItemType
{
    public Guid Id { get; set; } = new Guid();

    public string Name { get; set; }
    public string Category {get;set;}
}
