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
    public class JwtAuthenticationService(AppDbContext context, IConfiguration configuration) : IJwtAuthenticationService
    {

    public async Task<GameUser> SignupUserAsync(CreateUserDto user)
    {

        var usernameTaken = await context.GameUsers.AnyAsync(u => u.UserName == user.UserName);

        if (usernameTaken)
        {
            throw new InvalidOperationException("Username is taken");
        }
        GameUser createdUser = new GameUser();

        var hashedPassword = new PasswordHasher<GameUser>().HashPassword(createdUser, user.Password);
        createdUser.UserName = user.UserName;
        createdUser.PasswordHash = hashedPassword;

        context.GameUsers.Add(createdUser);
        await context.SaveChangesAsync();
        return createdUser;
    }

    public async Task<TokenResponseDto> LoginUserAsync(LoginUserDto reqUser)
    {
        GameUser user = await context.GameUsers.FirstOrDefaultAsync(u => u.UserName == reqUser.UserName);
        if (user == null)
        {
            throw new InvalidOperationException("User does not exist");
        }

        if (new PasswordHasher<GameUser>().VerifyHashedPassword(user, user.PasswordHash, reqUser.Password) == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException("Bad password");
        }

        var response = new TokenResponseDto
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        };
        return response;
    }

    private string CreateToken(GameUser user)
    {
        var claims = new List<Claim>
        {
          new Claim(ClaimTypes.Name, user.UserName),
          new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
          new Claim(ClaimTypes.Role, user.Role),

        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(GameUser user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();
        return refreshToken;
    }

    private async Task<GameUser?> ValidateRefreshTokenAsync(Guid Id, string refreshToken)
    {
        var user = await context.GameUsers.FindAsync(Id);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }
        return user;
    }

    public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
    {
        var user = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);
        if (user == null) {
            return null;
        }
        return new TokenResponseDto {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        };

    }
    }
}