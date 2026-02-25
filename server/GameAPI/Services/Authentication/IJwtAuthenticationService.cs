using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO.Pipelines;
using GameAPI.Data;
using GameAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using GameAPI.NewFolder;

namespace GameAPI.Services.Authentication
{
    public interface IJwtAuthenticationService
    {
            Task<GameUser> SignupUserAsync(CreateUserDto user);

            Task<TokenResponseDto> LoginUserAsync(LoginUserDto reqUser);

            Task<TokenResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}