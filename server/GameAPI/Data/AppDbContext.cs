using System;
using GameAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<GameUser> GameUsers {get;set;}
}
