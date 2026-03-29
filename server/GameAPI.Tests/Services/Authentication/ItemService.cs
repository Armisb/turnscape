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

namespace GameAPI.Tests;

public class ItemServiceTests : IDisposable
{
    AppDbContext dbContext;
    IConfiguration configuration;
    CreateUserDto createUserDto;
    JwtAuthenticationService authService;
    ItemService itemService;
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


    public async Task<List<Guid>> InitializeInventoryWithTwoItems(Guid id)
    {
        output.WriteLine("Initialize inventory");
        var createItemDto = new CreateItemDto
        {
            InventoryType = "InvType",
            Position = 0,
            Level = 1,
            Health = 10,
            ItemTypeId = Guid.NewGuid(),
            GameUserId = id
        };

        List<Guid> guids = new List<Guid>();
        guids.Add((await itemService.CreateItem(createItemDto)).Id);

        createItemDto.ItemTypeId = Guid.NewGuid();
        createItemDto.Position = 1;
        createItemDto.Level = 2;

        await itemService.CreateItem(createItemDto);
        guids.Add((await itemService.CreateItem(createItemDto)).Id);
        return guids;
    }

    [Fact]
    public async Task MoveOneItemToAFreeLocation()
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
        output.WriteLine(itemGuids[0].ToString());
        output.WriteLine(dbContext.Items.ToArray()[0].Id.ToString());
        output.WriteLine(dbContext.Items.ToArray()[1].Id.ToString());
        output.WriteLine(dbContext.Items.Count().ToString());
        Assert.NotNull(item);
        Assert.Equal(1, item.Level);
        Assert.Equal(2, item.Position);
        Assert.Equal("NewInv", item.InventoryType);
    }
}