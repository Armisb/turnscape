using GameAPI.Models;
using GameAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameAPI.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        
        [HttpGet]
        public async Task<ActionResult<List<GameUser>>> GetUser()
        {

            List<GameUser> user = await service.GetAllUsersAsync();
            return Ok(user);
        }
    
        [HttpGet("{Id}")]
        public async Task<ActionResult<GameUser>> GetUserById( int Id)
        {

            GameUser user = await service.GetAllUserByIdAsync(Id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<GameUser>> CreateUser()
        {

            GameUser user = await service.CreateUserAsync();
            return Ok();
        }
    }
}
