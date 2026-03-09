using System;

namespace GameAPI.NewFolder.ItemTypeDtos;

public class CreateWeaponTypeDto
{
    
    public required string Name { get; set; }
    public required string Category {get;set;}
    public required int Damage { get; set; }
}
