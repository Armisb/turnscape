using System;

namespace GameAPI.Models;

public class InLobby
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GameUserId {get ; set; }
}
