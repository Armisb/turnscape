using GameAPI.Models;
using GameAPI.Services;
using GameAPI.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using shared_lib;

namespace GameAPI.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController(IUserService service, IJwtAuthenticationService authenticationService) : ControllerBase
    {
        
        [Authorize(Roles = "GameUser")]
        [HttpGet]
        public async Task<ActionResult<List<GameUser>>> GetUser()
        {

            List<GameUser> user = await service.GetAllUsersAsync();
            return Ok(user);
        }
    
        [HttpGet("{Id}")]
        public async Task<ActionResult<GameUser>> GetUserById( int Id)
        {

            GameUser? user = await service.GetUserByIdAsync(Id);
            return user is null ? NotFound("User with specified id is not found") : Ok(user);
        }

        [HttpPost("signup")]
        public async Task<ActionResult<GameUser>> Singup(CreateUserDto newUser)
        {
            try
            {
            var response = await authenticationService.SignupUserAsync(newUser);
            return Ok(response);
                
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginUserDto request)
        {
            try
            {
            var result = await authenticationService.LoginUserAsync(request);
            return result == null ? BadRequest("Invalid username or password."):Ok(result);
                
            }
            catch(Exception e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("Refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authenticationService.RefreshTokensAsync(request);
            if(result is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }
            return Ok(result);
        }
    }
}
