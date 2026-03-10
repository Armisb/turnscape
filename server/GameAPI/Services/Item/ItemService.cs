using System;
using GameAPI.Data;
using GameAPI.NewFolder.ItemDtos;
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
}
