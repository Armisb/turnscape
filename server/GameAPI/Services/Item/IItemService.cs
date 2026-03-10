using System;
using GameAPI.NewFolder.ItemDtos;

namespace GameAPI.Services.Item;

public interface IItemService
{
    Task<Models.Item> CreateItem(CreateItemDto itemDto);
}
