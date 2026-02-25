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
using GameAPI.NewFolder;

namespace GameAPI.Services.User;

public class UserService(AppDbContext context, IConfiguration configuration) : IUserService
{
    public async Task<List<GameUser>> GetAllUsersAsync()
    {
        List<GameUser> users = await context.GameUsers.ToListAsync<GameUser>();
        return users;
    }

    public async Task<GameUser?> GetUserByIdAsync(int Id)
    {
        GameUser? user = await context.GameUsers.FindAsync(Id);
        return user;
    }

   
}
