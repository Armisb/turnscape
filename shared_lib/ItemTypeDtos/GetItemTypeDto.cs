using System;
 
namespace shared_lib.ItemTypeDtos
{
    public class GetItemTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int? Damage { get; set; }
        public int? Protection { get; set; }
    }
}