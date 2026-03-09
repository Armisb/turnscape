using GameAPI.Data;
using GameAPI.Models;
using GameAPI.NewFolder.ItemDtos;
using GameAPI.NewFolder.ItemTypeDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [Route("/Item")]
    [ApiController]
    public class ItemController(AppDbContext context) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<GetItemDto>>> GetUser()
        {

            List<GetItemDto> items = await context.Items.Include(x=>x.ItemType).Select(i=>new GetItemDto
            {
                Id = i.Id,
                InventoryType = i.InventoryType,
                Position = i.Position,
                Level = i.Level,
                Health = i.Health,
                ItemType = EF.Property<string>(i.ItemType, "Type"),
                Category = i.ItemType.Category,
                Damage = EF.Property<string>(i.ItemType, "Type") == "Weapon"? ((WeaponType)i.ItemType).Damage : null,
                Protection = EF.Property<string>(i.ItemType, "Type") == "Armor"? ((ArmorType)i.ItemType).Protection:null

            }).ToListAsync<GetItemDto>();
            return Ok(items);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Item>>> GetUser(Guid userId)
        {

            List<GetItemDto> items = await context.Items.Where(i=>i.GameUserId == userId).Include(x=>x.ItemType).Select(i=>new GetItemDto
            {
                Id = i.Id,
                InventoryType = i.InventoryType,
                Position = i.Position,
                Level = i.Level,
                Health = i.Health,
                ItemType = EF.Property<string>(i.ItemType, "Type"),
                Category = i.ItemType.Category,
                Damage = EF.Property<string>(i.ItemType, "Type") == "Weapon"? ((WeaponType)i.ItemType).Damage : null,
                Protection = EF.Property<string>(i.ItemType, "Type") == "Armor"? ((ArmorType)i.ItemType).Protection:null

            }).ToListAsync<GetItemDto>();
            return Ok(items);
        }
        
        [HttpPost("WeaponType")]
        public async Task<ActionResult<WeaponType>> CreateWeaponType(CreateWeaponTypeDto weaponType)
        {
            WeaponType created = new WeaponType(){Name = weaponType.Name, Category = weaponType.Category, Damage = weaponType.Damage};
            await context.ItemTypes.AddAsync(created);
            await context.SaveChangesAsync();
            return Ok(created);
        }

        [HttpPost("ArmorType")]
        public async Task<ActionResult<ArmorType>> CreateArmorType(CreateArmorTypeDto armorDto)
        {
            ArmorType created = new ArmorType(){Name = armorDto.Name, Category = armorDto.Category, Protection = armorDto.Protection};
            await context.ItemTypes.AddAsync(created);
            await context.SaveChangesAsync();
            return Ok(created);
        }

        [HttpPost]
        public async Task<ActionResult<Item>> CreateItem(CreateItemDto item)
        {
            Item created = new Item()
            {
                InventoryType = item.InventoryType,
                Position = item.Position,
                Level = item.Level,
                Health = item.Health,
                ItemTypeId = item.ItemTypeId,
                GameUserId = item.GameUserId
            };
            await context.Items.AddAsync(created);
            await context.SaveChangesAsync();
            return Ok(created);
        }
    }
}
