using System;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models;

public class GameUser
{
    public int Id { get; set; }

    [Required]
    public string? UserName { get; set; }    

    [Required]
    public string? Password { get; set; }

}
