using System;

namespace GameAPI.NewFolder.ItemDtos;

public class CreateItemDto
{
    public string? InventoryType { get; set; }
    public int? Position { get; set; }
    public required int Level { get; set; }
    public required int Health { get; set; }
    public required Guid ItemTypeId { get; set; }
    public Guid? GameUserId { get; set; }
}
