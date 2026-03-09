using System;

namespace GameAPI.Models;

public class ArmorType : ItemType
{
    public required int Protection { get; set; }
}
