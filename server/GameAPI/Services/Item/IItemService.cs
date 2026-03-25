using System;
using shared_lib.ItemDtos;

namespace GameAPI.Services.Item;

public interface IItemService
{
    Task<Models.Item> CreateItem(CreateItemDto itemDto);
    Task<List<UpdatePosDto>> UpdateItemsPositions(List<UpdatePosDto> updated, Guid id);
}
