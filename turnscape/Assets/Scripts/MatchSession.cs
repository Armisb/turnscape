using System;
using System.Collections.Generic;
using UnityEngine;

public static class MatchSession
{
    public static MatchData CurrentMatch;
    public static Guid MyPlayerId;

    public static bool IsPlayerOne =>
        CurrentMatch != null && CurrentMatch.PlayerOneId == MyPlayerId;
    public static Guid EnemyPlayerId =>
        IsPlayerOne ? CurrentMatch.PlayerTwoId : CurrentMatch.PlayerOneId;
    public static bool IsPlayerTwo =>
        CurrentMatch != null && CurrentMatch.PlayerTwoId == MyPlayerId;

    public static bool IsMyTurn =>
        CurrentMatch != null && CurrentMatch.CurrentTurnPlayerId == MyPlayerId;

    public static List<int> MyStats =>
        IsPlayerOne ? CurrentMatch.PlayerOneStats : CurrentMatch.PlayerTwoStats;

    public static List<int> EnemyStats =>
        IsPlayerOne ? CurrentMatch.PlayerTwoStats : CurrentMatch.PlayerOneStats;
}