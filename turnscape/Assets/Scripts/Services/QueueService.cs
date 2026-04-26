using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.InputSystem;

public static class QueueService
{
    private static string hubUrl = Networking.defaultBaseUrl + "matchhub";
    private static MatchData currentMatch;
    
    
    
    public async static void Connect()
    {
        string playerId = AuthManager.PlayerId;
        

        if (string.IsNullOrEmpty(playerId))
        {
            Debug.Log("Please enter a player ID");
            return;
        }

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl + "?userId=" + playerId)
            .WithAutomaticReconnect()
            .Build();

        connection.On<MatchData>("MatchFound", mdata =>
        {
            Debug.Log("Match Found!");
            currentMatch = mdata;
            GameManagerSc.LoadScene("CombatScene");
        });

        connection.On("MatchUpdated", (string match) =>
        {
            Debug.Log("Match Updated" + match);
        });

        connection.On<string>("ForceDisconnect", async message =>
        {
            Debug.Log("Force disconnect: " + message);

            if (connection != null)
                await connection.StopAsync();
        });

        try
        {
            await connection.StartAsync();
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
