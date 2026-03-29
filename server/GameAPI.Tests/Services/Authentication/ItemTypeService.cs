using GameAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using shared_lib;
using GameAPI.Services.Authentication;
using GameAPI.Services.ItemType;
using GameAPI.Models;
using shared_lib.ItemTypeDtos;
using GameAPI.Services.Item;
using shared_lib.ItemDtos;

namespace GameAPI.Tests;

public class ItemTypeServiceTests : IDisposable
{
    AppDbContext dbContext;
    IConfiguration configuration;
    CreateUserDto createUserDto;
    JwtAuthenticationService authService;
    ItemTypeService itemTypeService;
    private readonly ITestOutputHelper output;

    public ItemTypeServiceTests(ITestOutputHelper output)
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
        itemTypeService = new ItemTypeService(dbContext);

        this.output = output;
    }

    public void Dispose()
    {
        dbContext.Dispose();
    } 
    
    [Fact]
    public async Task CreateValidArmorType()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var createItemDto = new CreateArmorTypeDto
        {
            Name = "steelheart",
            Category = "Chestplate",
            Protection = 10
        };
        ArmorType armorType = await itemTypeService.CreateArmorType(createItemDto);
        Assert.Equal(createItemDto.Name, armorType.Name);
        Assert.Equal(createItemDto.Category, armorType.Category);
        Assert.Equal(createItemDto.Protection, armorType.Protection);

        var dbArmor = dbContext.ItemTypes.FirstOrDefault(x => x.Id == armorType.Id);
        Assert.NotNull(dbArmor);
        Assert.Equal(createItemDto.Name, dbArmor.Name);
        Assert.Equal(createItemDto.Category, dbArmor.Category);
    }

    [Fact]
    public async Task CreateDuplicateArmorType()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var createItemDto = new CreateArmorTypeDto
        {
            Name = "steelheart",
            Category = "Chestplate",
            Protection = 10
        };
        await itemTypeService.CreateArmorType(createItemDto);
        var exception = await Record.ExceptionAsync(async () => {
            await itemTypeService.CreateArmorType(createItemDto);
        });
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("That Item type already exists!", exception.Message);
    }

    [Fact]
    public async Task CreateValidWeaponType()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var createItemDto = new CreateWeaponTypeDto
        {
            Name = "steelpiercer",
            Category = "sword",
            Damage = 10
        };
        WeaponType weaponType = await itemTypeService.CreateWeaponType(createItemDto);
        Assert.Equal(createItemDto.Name, weaponType.Name);
        Assert.Equal(createItemDto.Category, weaponType.Category);
        Assert.Equal(createItemDto.Damage, weaponType.Damage);

        var dbWeapon = dbContext.ItemTypes.FirstOrDefault(x => x.Id == weaponType.Id);
        Assert.NotNull(dbWeapon);
        Assert.Equal(createItemDto.Name, dbWeapon.Name);
        Assert.Equal(createItemDto.Category, dbWeapon.Category);
    }

    [Fact]
    public async Task CreateDuplicateWeaponType()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var createItemDto = new CreateWeaponTypeDto
        {
            Name = "steelpiercer",
            Category = "sword",
            Damage = 10
        };
        await itemTypeService.CreateWeaponType(createItemDto);
        var exception = await Record.ExceptionAsync(async () => {
            await itemTypeService.CreateWeaponType(createItemDto);
        });
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("That Item type already exists!", exception.Message);
    }

    [Fact]
    public async Task DeleteValidItemTypeWithoutItems()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var createItemDto = new CreateArmorTypeDto
        {
            Name = "steelheart",
            Category = "Chestplate",
            Protection = 10
        };
        ArmorType armorType = await itemTypeService.CreateArmorType(createItemDto);

        Models.ItemType itemType = await itemTypeService.DeleteItemType(armorType.Id);
        Assert.Equal(0, dbContext.ItemTypes.Count());
        Assert.Equal(createItemDto.Name, itemType.Name);
        Assert.Equal(createItemDto.Category, itemType.Category);
    }

    [Fact]
    public async Task DeleteValidItemTypeWithItems()
    {
        GameUser gameUser = await authService.SignupUserAsync(createUserDto);

        var createItemTypeDto = new CreateArmorTypeDto
        {
            Name = "steelheart",
            Category = "Chestplate",
            Protection = 10
        };
        ArmorType armorType = await itemTypeService.CreateArmorType(createItemTypeDto);

        ItemService itemService = new ItemService(dbContext);

        CreateItemDto createItemDto = new CreateItemDto
        {
            GameUserId = gameUser.Id,
            Health = 10,
            InventoryType = "random",
            Level = 1,
            Position = 0,
            ItemTypeId = armorType.Id
        };
        await itemService.CreateItem(createItemDto);

        Models.ItemType itemType = await itemTypeService.DeleteItemType(armorType.Id);
        Assert.Equal(0, dbContext.ItemTypes.Count());
        Assert.Equal(createItemTypeDto.Name, itemType.Name);
        Assert.Equal(createItemTypeDto.Category, itemType.Category);
        Assert.Equal(0, dbContext.Items.Count());
    }

    [Fact]
    public async Task DeleteNonExistantItem()
    {
        var exception = await Record.ExceptionAsync(async () => {
            await itemTypeService.DeleteItemType(Guid.NewGuid());
        });
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("That Item doesnt exist!", exception.Message);
    }

    [Fact]
    public async Task GetAllItemTypesWhenThereAreNone()
    {
        List<GetItemTypeDto> itemTypes = await itemTypeService.GetItemTypeAll();
        Assert.Empty(itemTypes);
    }

    [Fact]
    public async Task GetAllItemTypesWhenThereAreTwo()
    {
        var createWeaponDto = new CreateWeaponTypeDto
        {
            Name = "steelpiercer",
            Category = "sword",
            Damage = 10
        };
        WeaponType weaponType = await itemTypeService.CreateWeaponType(createWeaponDto);
        var createArmorTypeDto = new CreateArmorTypeDto
        {
            Name = "steelheart",
            Category = "Chestplate",
            Protection = 10
        };
        ArmorType armorType = await itemTypeService.CreateArmorType(createArmorTypeDto);

        List<GetItemTypeDto> itemTypes = await itemTypeService.GetItemTypeAll();
        Assert.Equal(2, itemTypes.Count);
        GetItemTypeDto? weapon = itemTypes.Find(x => x.Id == weaponType.Id);
        Assert.NotNull(weapon);
        Assert.Equal(createWeaponDto.Name, weapon.Name);
        GetItemTypeDto? armor = itemTypes.Find(x => x.Id == armorType.Id);
        Assert.NotNull(armor);
        Assert.Equal(createArmorTypeDto.Name, armor.Name);
    }
}