using System;
using GameAPI.Data;
using GameAPI.Models;
using GameAPI.NewFolder.ItemTypeDtos;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Services.ItemType;

public class ItemTypeService(AppDbContext context) : IItemTypeService
{
    public async Task<ArmorType> CreateArmorType(CreateArmorTypeDto armorTypeDto)
    {
        var typeExists = await context.ItemTypes.AnyAsync(u => u.Name == armorTypeDto.Name);

        if (typeExists)
        {
            throw new InvalidOperationException("That Item type already exists!");
        }

        ArmorType created = new ArmorType(){Name = armorTypeDto.Name, Category = armorTypeDto.Category, Protection = armorTypeDto.Protection};

        context.ItemTypes.Add(created);
        await context.SaveChangesAsync();
        return created;
    }

    public async Task<WeaponType> CreateWeaponType(CreateWeaponTypeDto weaponTypeDto)
    {
        var typeExists = await context.ItemTypes.AnyAsync(u => u.Name == weaponTypeDto.Name);

        if (typeExists)
        {
            throw new InvalidOperationException("That Item type already exists!");
        }

        WeaponType created = new WeaponType(){Name = weaponTypeDto.Name, Category = weaponTypeDto.Category, Damage = weaponTypeDto.Damage};
        await context.ItemTypes.AddAsync(created);
        await context.SaveChangesAsync();
        return created;

    }

    public async Task<Models.ItemType> DeleteItemType(Guid Id)
    {
        var data = await context.ItemTypes.FirstOrDefaultAsync(x => x.Id == Id);
        if(data != null)
        {
            context.ItemTypes.Remove(data);
            
            await context.Items
            .Where(i => i.ItemTypeId == data.Id)
            .ExecuteDeleteAsync();
            
            await context.SaveChangesAsync();
            return data;
        }

        throw new InvalidOperationException("That Item doesnt exist!"); 
        
    }

    public async Task<List<GetItemTypeDto>> GetItemTypeAll()
    {
        var itemTypes = await context.ItemTypes.Select(i => new GetItemTypeDto
        {
            Id = i.Id,
            Name = i.Name,
            Category = i.Category,
            Damage = EF.Property<string>(i, "Type") == "Weapon"? ((WeaponType)i).Damage : null,
            Protection = EF.Property<string>(i, "Type") == "Armor"? ((ArmorType)i).Protection:null
        }).ToListAsync<GetItemTypeDto>();

        return itemTypes;
    }
}
