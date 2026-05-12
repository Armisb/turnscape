using System.Security.Claims;
using GameAPI.Models;
using GameAPI.NewFolder.ItemDtos;
using GameAPI.Services.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameAPI.Controllers
{
    [Route("store")]
    [ApiController]
    public class StoreController(IStoreServices storeServices) : ControllerBase
    {
        
        [HttpGet]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<List<GetInStoreDto>>> GetItemsInStore()
        {
            try
            {
            var response = await storeServices.GetItemsInStore();
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<InStoreItem>> PutItemInStore(CreateInStoreDto item)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
             return Unauthorized("Invalid token.");
            }

            Guid sellerId = Guid.Parse(userIdClaim.Value);

            var response = await storeServices.PutItemInStore(sellerId,item);
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    
        [HttpDelete("remove/{Id}")]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<InStoreItem>> RemoveItemInStore(Guid Id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
             return Unauthorized("Invalid token.");
            }

            Guid sellerId = Guid.Parse(userIdClaim.Value);

            var response = await storeServices.RemoveItemInStore(sellerId,Id);
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    
        [HttpPost("buy/{Id}")]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<InStoreItem>> BuyItemInStore(Guid Id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
             return Unauthorized("Invalid token.");
            }

            Guid buyersId = Guid.Parse(userIdClaim.Value);

            var response = await storeServices.BuyItemInStore(buyersId,Id);
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    
        [HttpPut]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<InStoreItem>> UpdateItemInStore(CreateInStoreDto item)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
             return Unauthorized("Invalid token.");
            }

            Guid sellerId = Guid.Parse(userIdClaim.Value);

            var response = await storeServices.UpdateItemInStore(sellerId,item);
            return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
