using GameAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using shared_lib;
using GameAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using GameAPI.Services.Authentication;
using GameAPI.Services.Item;
using shared_lib.ItemDtos;
using GameAPI.NewFolder.ItemDtos;
using GameAPI.Services.ItemType;
using shared_lib.ItemTypeDtos;

namespace GameAPI.Tests;

public class ItemServiceTests : IDisposable
{
    AppDbContext dbContext;
    IConfiguration configuration;
    CreateUserDto createUserDto;
    JwtAuthenticationService authService;
    ItemService itemService;
    ItemTypeService itemTypeService;
    private readonly ITestOutputHelper output;

    public ItemServiceTests(ITestOutputHelper output)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        dbContext = new AppDbContext(optionsBuilder.Options);

        configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

        createUserDto = new CreateUserDto
        {
            UserName = "Test",
            Password = "Test"
        };

        authService = new JwtAuthenticationService(dbContext, configuration);
        itemService = new ItemService(dbContext);
        itemTypeService = new ItemTypeService(dbContext);

        this.output = output;
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }

    [Fact]
    public async Task CreateValidItem()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var createItemDto = new CreateItemDto
        {
            InventoryType = "InvType",
            Position = 0,
            Level = 1,
            Health = 10,
            ItemTypeId = new Guid(),
            GameUserId = gameUser.Id
        };

        Models.Item item = await itemService.CreateItem(createItemDto);

        Assert.Equal(createItemDto.InventoryType, item.InventoryType);
        Assert.Equal(createItemDto.Position, item.Position);
        Assert.Equal(createItemDto.Level, item.Level);
        Assert.Equal(createItemDto.Health, item.Health);
        Assert.Equal(createItemDto.ItemTypeId, item.ItemTypeId);
        Assert.Equal(createItemDto.GameUserId, item.GameUserId);
        Assert.Equal(gameUser, item.GameUser);
        Assert.Equal(1, dbContext.Items.Count());
    }

    [Fact]
    public async Task UpdateItemsPositionsWithEmptyInventroyAndEmptyList()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);
        var updatePosDtos = new List<UpdatePosDto>();
        List<UpdatePosDto> updatedPos = await itemService.UpdateItemsPositions(updatePosDtos, gameUser.Id);
        Assert.Empty(updatedPos);
    }

    [Fact]
    public async Task UpdateItemsPositionsWithEmptyInventroyAndNonEmptyList()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);
        var updatePosDtos = new List<UpdatePosDto>();
        updatePosDtos.Add(new UpdatePosDto
        {
           Id = Guid.NewGuid(),
           Position = 2,
           InventoryType = "NewInv"
        });

        List<UpdatePosDto> updatedPos = await itemService.UpdateItemsPositions(updatePosDtos, gameUser.Id);
        Assert.Equal(2, updatedPos[0].Position);
        Assert.Equal("NewInv", updatedPos[0].InventoryType);
        Assert.Equal(0, dbContext.Items.Count());
    }


    public async Task<List<Guid>> InitializeInventoryWithTwoItems(Guid id)
    {
        var createArmorTypeDto = new CreateArmorTypeDto
        {
            Category = "Chestplate",
            Name = "Iron",
            Protection = 10
        };

        var itemTypeGuid = (await itemTypeService.CreateArmorType(createArmorTypeDto)).Id;

        var createItemDto = new CreateItemDto
        {
            InventoryType = "InvType",
            Position = 0,
            Level = 1,
            Health = 10,
            ItemTypeId = itemTypeGuid,
            GameUserId = id
        };

        List<Guid> guids = new List<Guid>();
        guids.Add((await itemService.CreateItem(createItemDto)).Id);

        createItemDto.Position = 1;
        createItemDto.Level = 2;

        guids.Add((await itemService.CreateItem(createItemDto)).Id);
        return guids;
    }

    [Fact]
    public async Task MoveOneItemToAFreeLocationInNewInventory()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);
        List<Guid> itemGuids = await InitializeInventoryWithTwoItems(gameUser.Id);
        var updatePosDtos = new List<UpdatePosDto>();
        updatePosDtos.Add(new UpdatePosDto
        {
           Id = itemGuids[0],
           Position = 2,
           InventoryType = "NewInv"
        });

        List<UpdatePosDto> itemUpdates = await itemService.UpdateItemsPositions(updatePosDtos, gameUser.Id);
        Assert.Equal(2, itemUpdates[0].Position);
        Assert.Equal("NewInv", itemUpdates[0].InventoryType);
        Assert.Equal(updatePosDtos[0].Id, itemUpdates[0].Id);

        var item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[0]);
        Assert.NotNull(item);
        Assert.Equal(1, item.Level);
        Assert.Equal(2, item.Position);
        Assert.Equal("NewInv", item.InventoryType);

        item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[1]);
        Assert.NotNull(item);
        Assert.Equal(2, item.Level);
        Assert.Equal(1, item.Position);
        Assert.Equal("InvType", item.InventoryType);
    }

    [Fact]
    public async Task MoveItemToTakenLocation()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);
        List<Guid> itemGuids = await InitializeInventoryWithTwoItems(gameUser.Id);
        var updatePosDtos = new List<UpdatePosDto>();
        updatePosDtos.Add(new UpdatePosDto
        {
           Id = itemGuids[0],
           Position = 1,
           InventoryType = "InvType"
        });

        List<UpdatePosDto> itemUpdates = await itemService.UpdateItemsPositions(updatePosDtos, gameUser.Id);
        Assert.Equal(1, itemUpdates[0].Position);
        Assert.Equal("InvType", itemUpdates[0].InventoryType);
        Assert.Equal(updatePosDtos[0].Id, itemUpdates[0].Id);

        var item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[0]);
        Assert.NotNull(item);
        Assert.Equal(1, item.Level);
        Assert.Equal(1, item.Position);
        Assert.Equal("InvType", item.InventoryType);

        item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[1]);
        Assert.NotNull(item);
        Assert.Equal(2, item.Level);
        Assert.Equal(0, item.Position);
        Assert.Equal("InvType", item.InventoryType);
    }

    [Fact]
    public async Task MoveOneItemToAFreePlayerEquipedLocation()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);
        List<Guid> itemGuids = await InitializeInventoryWithTwoItems(gameUser.Id);
        var updatePosDtos = new List<UpdatePosDto>();
        updatePosDtos.Add(new UpdatePosDto
        {
           Id = itemGuids[0],
           Position = 2,
           InventoryType = "PlayerEquiped"
        });

        List<UpdatePosDto> itemUpdates = await itemService.UpdateItemsPositions(updatePosDtos, gameUser.Id);
        Assert.Equal(2, itemUpdates[0].Position);
        Assert.Equal("PlayerEquiped", itemUpdates[0].InventoryType);
        Assert.Equal(updatePosDtos[0].Id, itemUpdates[0].Id);

        var item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[0]);
        Assert.NotNull(item);
        Assert.Equal(1, item.Level);
        Assert.Equal(2, item.Position);
        Assert.Equal("PlayerEquiped", item.InventoryType);

        item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[1]);
        Assert.NotNull(item);
        Assert.Equal(2, item.Level);
        Assert.Equal(1, item.Position);
        Assert.Equal("InvType", item.InventoryType);
    }

    [Fact]
    public async Task MoveOneItemToATakenPlayerEquipedLocation()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);
        List<Guid> itemGuids = await InitializeInventoryWithTwoItems(gameUser.Id);
        var updatePosDtos = new List<UpdatePosDto>();
        updatePosDtos.Add(new UpdatePosDto
        {
           Id = itemGuids[0],
           Position = 2,
           InventoryType = "PlayerEquiped"
        });
        updatePosDtos.Add(new UpdatePosDto
        {
           Id = itemGuids[1],
           Position = 0,
           InventoryType = "PlayerEquiped"
        });

        List<UpdatePosDto> itemUpdates = await itemService.UpdateItemsPositions(updatePosDtos, gameUser.Id);
        Assert.Equal(2, itemUpdates[0].Position);
        Assert.Equal("PlayerEquiped", itemUpdates[0].InventoryType);
        Assert.Equal(updatePosDtos[0].Id, itemUpdates[0].Id);
        Assert.Equal(0, itemUpdates[1].Position);
        Assert.Equal("PlayerEquiped", itemUpdates[1].InventoryType);
        Assert.Equal(updatePosDtos[1].Id, itemUpdates[1].Id);

        var item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[0]);
        Assert.NotNull(item);
        Assert.Equal(1, item.Level);
        Assert.Equal(1, item.Position);
        Assert.Equal("InvType", item.InventoryType);

        item = dbContext.Items.FirstOrDefault(x => x.Id == itemGuids[1]);
        Assert.NotNull(item);
        Assert.Equal(2, item.Level);
        Assert.Equal(0, item.Position);
        Assert.Equal("PlayerEquiped", item.InventoryType);
    }
}