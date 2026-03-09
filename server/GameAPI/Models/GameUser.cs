using System;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models;

public class GameUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? UserName { get; set; }    
    public string? PasswordHash { get; set; }
    public string Role { get; set; } = "GameUser";
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

}
