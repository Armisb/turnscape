using System;
using Microsoft.VisualBasic;

namespace GameAPI.Helpers;

public class AttackHelpers
{


    public static int CalcDamageLiteAttack(List<int> playerOneStat, List<int> playerTwoStat)
    {
        
        if(playerOneStat[0] == 0)
        {
            return 3;
        }
        double chance = Random.Shared.Next(1,100);
        double positive_outcome_chance = 85;
        
        double damage_percent = Math.Max(0.3,(Random.Shared.Next(80,80)-Math.Max(0,(playerTwoStat[1]-playerOneStat[0])))/100);

        int damage = (int)(playerOneStat[0]*damage_percent);


        return chance <= positive_outcome_chance ? damage : 0;
    }

    public static int CalcDamageHeavyAttack(List<int> playerOneStat, List<int> playerTwoStat)
    {
        if(playerOneStat[0] == 0)
        {
            return 6;
        }
        double chance = Random.Shared.Next(1,100);
        double positive_outcome_chance = 40;
        
        double damage_percent = Math.Max(0.8,(Random.Shared.Next(150,200)-Math.Max(0,(playerTwoStat[1]-playerOneStat[0])))/100);

        int damage = (int)(playerOneStat[0]*damage_percent);

        return chance <= positive_outcome_chance ? damage : 0;
    }
}
