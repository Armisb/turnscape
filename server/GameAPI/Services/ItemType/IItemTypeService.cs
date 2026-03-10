using System;
using GameAPI.Models;
using GameAPI.NewFolder.ItemTypeDtos;

namespace GameAPI.Services.ItemType;

public interface IItemTypeService
{
    //Create-Update-delete WeaponType
    //Create-Update-delete Armortype
    Task<WeaponType> CreateWeaponType(CreateWeaponTypeDto weaponDto);
    Task<ArmorType> CreateArmorType(CreateArmorTypeDto armorTypeDto);
    Task<List<GetItemTypeDto>> GetItemTypeAll();
    Task<Models.ItemType> DeleteItemType(Guid Id);
    

}
