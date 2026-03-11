using System;
namespace shared_lib.ItemDtos
{
    public class CreateItemDto
    {
        public string? InventoryType { get; set; }
        public int? Position { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public Guid ItemTypeId { get; set; }
        public Guid? GameUserId { get; set; }
    }
}