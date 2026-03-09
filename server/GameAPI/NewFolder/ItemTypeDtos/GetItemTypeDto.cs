using System;

namespace GameAPI.NewFolder.ItemTypeDtos;

public class GetItemTypeDto
{

    public required string Name { get; set; }
    public required string Category { get; set; }

    public int? Damage { get; set; }
}
