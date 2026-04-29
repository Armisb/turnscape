using GameAPI.Data;
using GameAPI.Models;
using GameAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GameAPI.Hubs
{
    public class MatchHub(AppDbContext context, IUserService userService) : Hub
    {

        public async Task Attack(Guid MatchId, string attack)
        {
            string activeId = Context.UserIdentifier;
            
            Match match = await context.Matches.FirstOrDefaultAsync<Match>(x=>x.Id == MatchId);

            if(match.CurrentTurnPlayerId.ToString() != activeId)
            {
                throw new InvalidOperationException("Not Your Turn");
            }

            if(match.IsFinished)
            {
                throw new InvalidOperationException("ended");
            }

            if(activeId == match.PlayerOneId.ToString())
            {
                int damage = 0;
                if(attack.ToLower() == "lite")
                {
                damage = Helpers.AttackHelpers.CalcDamageLiteAttack(match.PlayerOneStats,match.PlayerTwoStats);
                }else if(attack.ToLower() == "heavy"){
                    damage = Helpers.AttackHelpers.CalcDamageHeavyAttack(match.PlayerOneStats,match.PlayerTwoStats);
                }
                match.PlayerTwoStats[2] -= damage;
            }
            else
            {
                int damage = 0;
                if(attack.ToLower() == "lite")
                {
                damage = Helpers.AttackHelpers.CalcDamageLiteAttack(match.PlayerTwoStats,match.PlayerOneStats);
                }else if(attack.ToLower() == "heavy"){
                    damage = Helpers.AttackHelpers.CalcDamageHeavyAttack(match.PlayerTwoStats,match.PlayerOneStats);
                }
                match.PlayerOneStats[2] -= damage;
            }
            
            match.CurrentTurnPlayerId = activeId == match.PlayerOneId.ToString()
                ? match.PlayerTwoId
                : match.PlayerOneId;


            if( match.PlayerTwoStats[2]<0 || match.PlayerOneStats[2] < 0)
            {   
                match.IsFinished=true;
            }
            await context.SaveChangesAsync();

            await Clients.Users(
                match.PlayerOneId.ToString(),
                match.PlayerTwoId.ToString()
            ).SendAsync("MatchUpdated", match);
            }
   
    }
}