using System;
using GameAPI.Data;
using GameAPI.NewFolder.ItemDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GameAPI.Services.Item;

public class ItemService(AppDbContext context) : IItemService
{
    public async Task<Models.Item> CreateItem(CreateItemDto itemDto)
    {
        Models.Item created = new Models.Item()
            {
                InventoryType = itemDto.InventoryType,
                Position = itemDto.Position,
                Level = itemDto.Level,
                Health = itemDto.Health,
                ItemTypeId = itemDto.ItemTypeId,
                GameUserId = itemDto.GameUserId
            };
            await context.Items.AddAsync(created);
            await context.SaveChangesAsync();
            return created;
    }

    public async Task<List<UpdatePosDto>> UpdateItemsPositions(List<UpdatePosDto> updated, Guid id)
  {
    var items =  await context.Items.Where(x => x.GameUserId == id).ToListAsync();

    foreach(UpdatePosDto updatePos in updated)
        {
            var item_to_switch = items.FirstOrDefault(x=> x.Id == updatePos.Id);
            if (item_to_switch == null) continue; // or throw

            if(updatePos.InventoryType.ToLower() != "playerequiped")
            {
                var item_in_place = items.FirstOrDefault(x => x.Position == updatePos.Position && x.InventoryType == updatePos.InventoryType &&
                                    x.Id != updatePos.Id);
                if(item_in_place != null)
                {
                    item_in_place.InventoryType = item_to_switch.InventoryType;
                    item_in_place.Position = item_to_switch.Position;
                    
                    item_to_switch.InventoryType = updatePos.InventoryType;
                    item_to_switch.Position = updatePos.Position;
                }
                else
                {
                    item_to_switch.InventoryType = updatePos.InventoryType;
                    item_to_switch.Position = updatePos.Position;
                }
            }
            else
            {
                var item_in_place = items.FirstOrDefault(x => x.ItemType.Category == item_to_switch.ItemType.Category && x.Id != updatePos.Id );
                if(item_in_place != null)
                {
                    item_in_place.InventoryType = item_to_switch.InventoryType;
                    item_in_place.Position = item_to_switch.Position;
                    
                    item_to_switch.InventoryType = updatePos.InventoryType;
                    item_to_switch.Position = updatePos.Position;
                }
                else
                {
                    item_to_switch.InventoryType = updatePos.InventoryType;
                    item_to_switch.Position = updatePos.Position;
                }
            }
        }
        await context.SaveChangesAsync();
            return updated;
  }
}
