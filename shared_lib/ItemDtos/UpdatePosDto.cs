using System;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.NewFolder.ItemDtos
{

	public class UpdatePosDto
	{
		public Guid Id { get; set; }
		[Required]
		public string InventoryType { get; set; }
		[Required]
		public int Position { get; set; }
	}
}
