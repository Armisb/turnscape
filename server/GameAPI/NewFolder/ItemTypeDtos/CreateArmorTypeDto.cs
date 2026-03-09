using System;

namespace GameAPI.NewFolder.ItemTypeDtos;

public class CreateArmorTypeDto
{
      public required string Name { get; set; }
    public required string Category {get;set;}
    public required int Protection { get; set; }
}
