using System;
using GameAPI.Models;
using GameAPI.NewFolder.ItemDtos;

namespace GameAPI.Services.Store;

public interface IStoreServices
{
// GET all item s
// Put item 
// Remove item 
// Update item
// Buy Item  
  Task<List<GetInStoreDto>> GetItemsInStore();
  Task<InStoreItem> PutItemInStore(Guid sellerId, CreateInStoreDto inStoreDto);
  Task<InStoreItem> RemoveItemInStore(Guid sellerId, Guid itemId);
  Task<InStoreItem> UpdateItemInStore(Guid sellerId, CreateInStoreDto inStoreDto);
  Task<InStoreItem> BuyItemInStore(Guid buyerId, Guid itemId);
}
