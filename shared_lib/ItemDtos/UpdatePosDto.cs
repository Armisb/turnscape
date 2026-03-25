using System;

namespace shared_lib.ItemDtos;

public class UpdatePosDto
{
    public Guid Id { get; set; }
    public required string InventoryType { get; set; }
    public required int Position { get; set; }
}
