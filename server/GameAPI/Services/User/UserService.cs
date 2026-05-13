using System;
using System.IO.Pipelines;
using GameAPI.Data;
using GameAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using shared_lib;
using shared_lib.ItemDtos;
using GameAPI.Helpers;

namespace GameAPI.Services.User;

public class UserService(AppDbContext context, IConfiguration configuration) : IUserService
{
    public async Task<List<GameUser>> GetAllUsersAsync()
    {
        List<GameUser> users = await context.GameUsers.ToListAsync<GameUser>();
        return users;
    }

    // change that later id would be parsed from auth token
    public async Task<List<int>> CalcStatistics(Guid id)
    {
        List<GetItemDto> playeritems = await context.Items.Where(i=>i.GameUserId == id && i.InventoryType.ToLower() == "playerequiped").Include(x=>x.ItemType).Select(i=>new GetItemDto
            {
                Id = i.Id,
                InventoryType = i.InventoryType,
                Position = i.Position,
                Level = i.Level,
                Health = i.Health,
                ItemType = EF.Property<string>(i.ItemType, "Type"),
                Category = i.ItemType.Category,
                Name = i.ItemType.Name,
                Damage = EF.Property<string>(i.ItemType, "Type") == "Weapon"? ((WeaponType)i.ItemType).Damage : 0,
                Protection = EF.Property<string>(i.ItemType, "Type") == "Armor"? ((ArmorType)i.ItemType).Protection:0

            }).ToListAsync<GetItemDto>();

            return(UserStatistics.CalcPlayerStats(playeritems));

    }

  public async Task<decimal> GetMoney(Guid id)
  {
    GameUser user = await context.GameUsers.FirstOrDefaultAsync(x => x.Id == id);


    return user.Money;
  }
}
