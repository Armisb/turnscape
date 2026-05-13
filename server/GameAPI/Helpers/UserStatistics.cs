using System;
using GameAPI.Models;
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
                if(item.ItemType.ToLower() == "weapon") damage += item.Level;
                protection += item.Protection != null ? (int)item.Protection : 0;  
                if(item.ItemType.ToLower() == "armor") protection += item.Level;   
            }
        }

        return new List<int>{damage, protection};
    } 

    public static int GetEmptyInvPosition(List<Models.Item> items)
    {
        int pos = -1;
        int inv_size = 25;
        for(int i = 0; i < inv_size; i++)
        {
            var it = items.FirstOrDefault(x=> x.InventoryType.ToLower() == "playerinventory" && x.Position == i);
            if(it is null)
            {
                pos = i;
                return pos;
            }
            
        }
        
        return pos;
    } 
}
