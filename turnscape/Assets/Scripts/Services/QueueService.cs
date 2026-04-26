using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public static class QueueService
{
    private static string hubUrl = Networking.defaultBaseUrl + "matchhub";
    private static MatchData currentMatch;
    public static HubConnection Connection;
    public static event Action<MatchData> OnMatchUpdated;
    
    
    public async static void Connect()
    {
        string playerId = AuthManager.PlayerId;
        

        if (string.IsNullOrEmpty(playerId))
        {
            Debug.Log("Please enter a player ID");
            return;
        }

        var Connection = new HubConnectionBuilder()
            .WithUrl(hubUrl + "?userId=" + playerId)
            .WithAutomaticReconnect()
            .Build();

        Connection.On<MatchData>("MatchFound", mdata =>
        {
            Debug.Log("Match Found!");
            
            MatchSession.CurrentMatch = mdata;
            MatchSession.MyPlayerId = Guid.Parse(AuthManager.PlayerId);
            
            Debug.Log("I am player one: " + MatchSession.IsPlayerOne);
            Debug.Log("I am player two: " + MatchSession.IsPlayerTwo);
            Debug.Log("Is my turn: " + MatchSession.IsMyTurn);
            SceneManager.LoadScene("CombatScene");
        });

        Connection.On<MatchData>("MatchUpdated", match =>
        {
            MatchSession.CurrentMatch = match;
            OnMatchUpdated?.Invoke(match);
            Debug.Log("Match updated");
            Debug.Log("Current turn: " + match.CurrentTurnPlayerId);
        });

        Connection.On<string>("ForceDisconnect", async message =>
        {
            Debug.Log("Force disconnect: " + message);

            if (Connection != null)
                await Connection.StopAsync();
        });

        try
        {
            await Connection.StartAsync();
            Debug.Log("Connected as " + playerId);
            string lobbyUrl = "match/lobby/" + playerId;
            Debug.Log("LOBBY URL - " + lobbyUrl);
            await Networking.SendPostGeneric(lobbyUrl,
                "",
                x =>
                {
                    Debug.Log("Lobby response: " + x);
                    if (x == "true")
                    {
                        Debug.Log("Matched / waiting for MatchFound");
                    }
                    else
                    {
                        Debug.Log("In queue, waiting for another player");
                    }


                },
                x => Debug.Log("Lobby request failed: " + x)
                );

        }
        catch (Exception ex)
        {
            Debug.Log("Connection failed: " + ex.Message);
        }
    }
    public async static void Attack(int attackIndex)
    {
        if (!MatchSession.IsMyTurn)
        {
            Debug.Log("Not your turn!");
            return;
        }

        AttackRequest request = new AttackRequest
        {
            MatchId = MatchSession.CurrentMatch.Id,
            AttackerId = MatchSession.MyPlayerId,
            TargetId = MatchSession.EnemyPlayerId,
        };

        await QueueService.Connection.InvokeAsync("Attack", request);

        Debug.Log("Attack sent");
    }
}
[Serializable]
public class MatchData
{
    public Guid Id {get; set;} = new Guid();

    [Required]
    public string State {get;set;}
    public Guid CurrentTurnPlayerId {get;set;}
    public Guid PlayerOneId {get;set;}
    public List<int> PlayerOneStats {get;set;}
    public Guid PlayerTwoId {get;set;}
    public List<int> PlayerTwoStats {get;set;}
    public bool IsFinished {get;set;}
}

[Serializable]
public class AttackRequest
{
    public Guid MatchId;
    public Guid AttackerId;
    public Guid TargetId;

}