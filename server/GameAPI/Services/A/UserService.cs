using System;
using GameAPI.Data;
using GameAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Services.User;

public class UserService(AppDbContext context) : IUserService
{
    public async Task<List<GameUser>> GetAllUsersAsync()
    {
        List<GameUser> users = await context.GameUsers.ToListAsync<GameUser>();
        return users;
    }

    public async Task<GameUser> GetAllUserByIdAsync(int Id)
    {
        GameUser user = await context.GameUsers.FindAsync(Id);
        return user;
    }

    public async Task<GameUser> CreateUserAsync()
    {
        GameUser user = new GameUser{UserName = "petras", Password = "petras123"};
        context.GameUsers.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
