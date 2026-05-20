using System;
using GameAPI.Models;

namespace GameAPI.Services.Lobby;

public interface ILobbyService
{
    Task<List<Match>> GetMatchAll(); 
    Task<Match> RemoveMatch(Guid MatchId);
    Task<bool> JoinLobby(Guid PlayerId);
    Task<bool> LeaveLobby(Guid PlayerId);
    Task<bool> FindMatch();
}
