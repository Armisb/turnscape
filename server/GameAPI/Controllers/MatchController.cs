using GameAPI.Models;
using GameAPI.Services.Lobby;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameAPI.Controllers
{
    [Route("match")]
    [ApiController]
    public class MatchController(ILobbyService lobbyService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<List<Match>>> GetMatchAll()
        {
            var res = await lobbyService.GetMatchAll();
            return Ok(res);
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<Match>> RemoveMatch(Guid Id)
        {
            try
            {
                var res = await lobbyService.RemoveMatch(Id);
                return Ok(res);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("lobby/{Id}")]
        [Authorize(Roles = "Admin,GameUser")]
        public async Task<ActionResult<bool>> GetUserStats( Guid Id)
        {
            var res = await lobbyService.JoinLobby(Id);
            return Ok(res);
        }
    }
}
