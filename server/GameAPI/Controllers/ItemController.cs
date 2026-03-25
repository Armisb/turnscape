using GameAPI.Data;
using GameAPI.Models;
using shared_lib.ItemDtos;
using shared_lib.ItemTypeDtos;
using GameAPI.Services.Item;
using GameAPI.Services.ItemType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Controllers
{
    [Route("/Item")]
    [ApiController]
    public class ItemController(AppDbContext context, IItemTypeService itemTypeService, IItemService itemService) : ControllerBase
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
                Name = i.ItemType.Name,
                Damage = EF.Property<string>(i.ItemType, "Type") == "Weapon"? ((WeaponType)i.ItemType).Damage : 0,
                Protection = EF.Property<string>(i.ItemType, "Type") == "Armor"? ((ArmorType)i.ItemType).Protection:0

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
                Name = i.ItemType.Name,
                Damage = EF.Property<string>(i.ItemType, "Type") == "Weapon"? ((WeaponType)i.ItemType).Damage : 0,
                Protection = EF.Property<string>(i.ItemType, "Type") == "Armor"? ((ArmorType)i.ItemType).Protection:0

            }).ToListAsync<GetItemDto>();
            return Ok(items);
        }
        
        
        // With Service 
        [HttpPost("WeaponType")]
        public async Task<ActionResult<WeaponType>> CreateWeaponType(CreateWeaponTypeDto weaponTypeDto)
        {
            try
            {
            var response = await itemTypeService.CreateWeaponType(weaponTypeDto);
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // With Service
        [HttpPost("ArmorType")]
        public async Task<ActionResult<ArmorType>> CreateArmorType(CreateArmorTypeDto armorTypeDto)
        {
            try
            {
            var response = await itemTypeService.CreateArmorType(armorTypeDto);
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // With Service
        [HttpGet("ItemType")]
        public async Task<ActionResult<List<GetItemTypeDto>>> GetItemTypeAll()
        {
            List<GetItemTypeDto> itemTypes = await itemTypeService.GetItemTypeAll();
            return Ok(itemTypes);
        }

        // With Service
        [HttpDelete("{Id}")]
        public async Task<ActionResult<ItemType>> DeleteItemType(Guid Id)
        {
            try
            {
            ItemType response = await itemTypeService.DeleteItemType(Id);
            
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut("update-positions/{userId}")]
        public async Task<ActionResult<List<UpdatePosDto>>> UpdatePositions(
        Guid userId,
        List<UpdatePosDto> updated)
        {
            if (updated == null)
                return BadRequest("No items provided.");

            var result = await itemService.UpdateItemsPositions(updated, userId);

            return Ok(result);
        }
        
        // with service
        [HttpPost]
        public async Task<ActionResult<Item>> CreateItem(CreateItemDto itemDto)
        {
            Item created = await itemService.CreateItem(itemDto);
            return Ok(created);
        }

        //Create-Update-delete WeaponType : Create Delete
        //Create-Update-delete Armortype : Create Delete
        //Create-Update-delete Item
        //Get all items 
        //Get item for specific user
        //Update item position
        
    }
}
