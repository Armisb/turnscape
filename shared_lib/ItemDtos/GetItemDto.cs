using System;
namespace shared_lib.ItemDtos
{
    public class GetItemDto
    {
        public Guid Id { get; set; }
        public string? InventoryType { get; set; }
        public int? Position { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }

        public string ItemType { get; set; }
        //Add Name
        public string Name { get; set; }
        public string Category { get; set; }
        public int? Damage { get; set; }
        public int? Protection { get; set; }
    }
}