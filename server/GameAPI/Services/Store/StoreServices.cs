using System;
using GameAPI.Data;
using GameAPI.Models;
using GameAPI.NewFolder.ItemDtos;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Services.Store;

public class StoreServices(AppDbContext context) : IStoreServices
{
  public async Task<InStoreItem> BuyItemInStore(Guid buyerId, Guid itemId)
  {
   var itemInStore = await context.InStoreItems.Include(x => x.Item).FirstOrDefaultAsync(x => x.Item.Id == itemId);

   if(itemInStore == null)
    {
       throw new InvalidOperationException("Item you're trying to buy from store doesn't exists");
    }

   var buyer = await context.GameUsers.FirstOrDefaultAsync(x => x.Id == buyerId);

   if (buyer == null)
    {
        throw new KeyNotFoundException("Buyer not found.");
    }

   var seller = await context.GameUsers.FirstOrDefaultAsync(x => x.Id == itemInStore.Item.GameUserId);

   if (seller == null)
    {
        throw new KeyNotFoundException("Seller not found.");
    }

   

    if(itemInStore.GameUserId == buyerId)
    {
      throw new InvalidOperationException("You Cannot buy this item, youre the owner.");
    }
    
    if(buyer.Money < itemInStore.Price)
    {
      throw new InvalidOperationException("You don't have enough funds.");
    }

    seller.Money += itemInStore.Price;
    buyer.Money -= itemInStore.Price;
    itemInStore.Item.GameUserId = buyerId;
    itemInStore.Item.InventoryType = "PlayerInventory";

    context.InStoreItems.Remove(itemInStore);
    await context.SaveChangesAsync();
    return itemInStore;

  }
 // DONE 
  public async Task<List<GetInStoreDto>> GetItemsInStore()
  {
     List<GetInStoreDto> itemsInStore = await context.InStoreItems.Include(x=>x.Item).Select(i=>new GetInStoreDto
            {
                Id = i.Id,
                Price = i.Price,
                Level = i.Item.Level,
                Health = i.Item.Health,
                ItemType = EF.Property<string>(i.Item.ItemType, "Type"),
                Category = i.Item.ItemType.Category,
                Name = i.Item.ItemType.Name,
                Damage = EF.Property<string>(i.Item.ItemType, "Type") == "Weapon"? ((WeaponType)i.Item.ItemType).Damage : 0,
                Protection = EF.Property<string>(i.Item.ItemType, "Type") == "Armor"? ((ArmorType)i.Item.ItemType).Protection:0

            }).ToListAsync<GetInStoreDto>();

            return itemsInStore;
  }

// DONE
  public async Task<InStoreItem> PutItemInStore(Guid sellerId, CreateInStoreDto inStoreDto)
  {
    var itemToSell = await context.Items.FirstOrDefaultAsync(x=>x.Id == inStoreDto.ItemID);

    if(itemToSell == null)
    {
       throw new InvalidOperationException("Item you're trying to sell doesn't exists");
    }

    // Check the owner of item
    if(itemToSell.GameUserId != sellerId)
    {
      throw new InvalidOperationException("You are not the owner.");
    }

    // Check if it isnt already in store
    if (itemToSell.InventoryType.ToLower().Equals("shopinventory"))
    {
      throw new InvalidOperationException("Item is already in store.");
    }

    if(inStoreDto.Price < 0)
    {
      throw new InvalidOperationException("Price should be more than 0.00");
    }

bool alreadyListed = await context.InStoreItems
    .AnyAsync(x => x.ItemId == itemToSell.Id);

if (alreadyListed)
{
    throw new InvalidOperationException("Item is already listed.");
}

    InStoreItem itemInstore = new InStoreItem()
    {
      Price = inStoreDto.Price,
      ItemId = inStoreDto.ItemID,
      GameUserId = sellerId
    };

    itemToSell.InventoryType = "shopinventory";

    await context.InStoreItems.AddAsync(itemInstore);
    await context.SaveChangesAsync();
    return itemInstore;
  }

 // DONE
  public async Task<InStoreItem> RemoveItemInStore(Guid sellerId, Guid itemId)
  {
    var itemInStore = await context.InStoreItems.Include(x => x.Item).FirstOrDefaultAsync(x => x.Item.Id == itemId);

    if(itemInStore == null)
    {
       throw new InvalidOperationException("Item you're trying to remove from store doesn't exists");
    }
    // Check the owner of item
    if(itemInStore.GameUserId != sellerId)
    {
      throw new InvalidOperationException("You are not the owner.");
    }
    // Check if it isnt already in store
    if (!itemInStore.Item.InventoryType.ToLower().Equals("shopinventory"))
    {
      throw new InvalidOperationException("Item is not in in store.");
    }

    itemInStore.Item.InventoryType = "PlayerInventory";
    context.InStoreItems.Remove(itemInStore);
    await context.SaveChangesAsync();
    return itemInStore;
  }

  public async Task<InStoreItem> UpdateItemInStore(Guid sellerId, CreateInStoreDto inStoreDto)
  {
    var itemInStore = await context.InStoreItems
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.ItemId == inStoreDto.ItemID);

    if (itemInStore == null)
    {
        throw new KeyNotFoundException("Store item not found.");
    }

    if (itemInStore.GameUserId != sellerId)
    {
        throw new UnauthorizedAccessException(
            "You are not the owner of this item.");
    }

    if (inStoreDto.Price <= 0)
    {
        throw new InvalidOperationException(
            "Price must be greater than 0.");
    }

    itemInStore.Price = inStoreDto.Price;

    await context.SaveChangesAsync();

    return itemInStore;
  }
}
