using System;
using GameAPI.Data;
using GameAPI.Hubs;
using GameAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Services.Lobby;

public class LobbyService(AppDbContext context, IHubContext<MatchHub> matchHub, IUserService userService) : ILobbyService
{
    public async Task FindMatch()
    {
        var players = await context.Lobby.Take(2).ToListAsync();

        if(players.Count != 2)
        {
            return;
        }

        context.Lobby.Remove(players[0]);
        context.Lobby.Remove(players[1]);

        List<int> stats1 = await userService.CalcStatistics(players[0].GameUserId);
        stats1.Add(100);
        List<int> stats2 = await userService.CalcStatistics(players[1].GameUserId);
        stats2.Add(100);


        Match newMatch = new Match()
        {
            State = "In Progress",
            CurrentTurnPlayerId = players[0].GameUserId,
            PlayerOneId = players[0].GameUserId,
            PlayerOneStats = stats1,
            PlayerTwoId = players[1].GameUserId,
            PlayerTwoStats = stats2,
            IsFinished = false
        };
        await context.Matches.AddAsync(newMatch);
        await context.SaveChangesAsync();

        await matchHub.Clients.User(players[0].GameUserId.ToString())
            .SendAsync("MatchFound", newMatch);
        await matchHub.Clients.User(players[1].GameUserId.ToString())
            .SendAsync("MatchFound", newMatch);
    }

    public async Task<List<Match>> GetMatchAll()
    {
        return await context.Matches.ToListAsync<Match>();
    }

    public async Task<bool> JoinLobby(Guid PlayerId)
    {
        var exists = await context.Lobby.AnyAsync(x => x.GameUserId == PlayerId);
            if (exists) return false;

            var player = new InLobby()
            {
                GameUserId = PlayerId
            };

            await context.Lobby.AddAsync(player);
            await context.SaveChangesAsync();
            return true;
    }

    public async Task<bool> LeaveLobby(Guid PlayerId)
    {
        var exists = await context.Lobby.FirstOrDefaultAsync(x => x.GameUserId == PlayerId);
        if (exists == null) return false;

        context.Lobby.Remove(exists);
        await context.SaveChangesAsync();
        
        return true;
    }

    public async Task<Match> RemoveMatch(Guid MatchId)
    {
        var toRemove = await context.Matches.FirstOrDefaultAsync<Match>(x => x.Id == MatchId);

        if(toRemove != null)
        {
            context.Matches.Remove(toRemove);
            await context.SaveChangesAsync();
            return toRemove;
        }

        throw new InvalidOperationException("That Item doesnt exist!"); 
    }
}
