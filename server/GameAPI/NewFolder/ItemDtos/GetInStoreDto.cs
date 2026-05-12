using System;

namespace GameAPI.NewFolder.ItemDtos;

public class GetInStoreDto
{
  //ORDER SPECIFIC
  public Guid Id { get; set; } = new Guid();
  public DateTime DatePosted {get;set;} = DateTime.Now;
  public decimal Price {get;set;}

  // Item Specific
  public int Level { get; set; }
  public int Health { get; set; }
  public string ItemType { get; set; }
  public string Name { get; set; }
  public string Category { get; set; }
  public int? Damage { get; set; }
  public int? Protection { get; set; }
}
