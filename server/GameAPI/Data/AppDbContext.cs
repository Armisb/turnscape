using System;
using GameAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<GameUser> GameUsers {get;set;}
    public DbSet<ItemType> ItemTypes {get;set;}
    public DbSet<Item> Items {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{

    modelBuilder.Entity<ItemType>()
        .HasDiscriminator<string>("Type")
        .HasValue<WeaponType>("Weapon")
        .HasValue<ArmorType>("Armor");
    
}
}
