using System;
using shared_lib.ItemDtos;

namespace GameAPI.Helpers;

public class UserStatistics
{
    public static List<int> CalcPlayerStats(List<GetItemDto> items)
    {
        int damage = 0;
        int protection = 0;
        foreach(GetItemDto item in items)
        {
            if(item.InventoryType.ToLower() == "playerequiped")
            {
                damage += item.Damage != null ? (int)item.Damage : 0 ;
                if(item.ItemType == "Weapon") damage += item.Level;
                protection += item.Protection != null ? (int)item.Protection : 0;  
                if(item.ItemType == "Armor") protection += item.Level;   
            }
        }

        return new List<int>{damage, protection};
    } 
}
