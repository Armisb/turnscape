using System;

namespace GameAPI.Models;

public class InStoreItem
{
  public Guid Id { get; set; } = new Guid();
  public DateTime DatePosted {get;set;} = DateTime.Now;
  public decimal Price {get;set;}
  public Guid ItemId { get; set; }
  public Item Item {get;set;}
  public Guid? GameUserId { get; set; }
  public GameUser? GameUser { get; set; }

}
