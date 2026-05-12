using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public static class QueueService
{
    private static string hubUrl = Networking.defaultBaseUrl + "matchhub";
    private static MatchData currentMatch;
    public static HubConnection Connection;
    public static MatchData PendingMatchUpdate;
    public static event Action<MatchData> OnMatchUpdated;
    public static bool HasPendingMatchUpdate;
    
    public static async void LeaveQueue()
    {
        var playerId = AuthManager.PlayerId;
        string lobbyUrl = "match/lobby/" + playerId;

        await Networking.SendDeleteGeneric(lobbyUrl,
                "",
                x => Debug.Log("Lobby left"),
                x => Debug.Log("Lobby request failed: " + x)
                );
        await Connection.StopAsync();
    }

    public async static void Connect()
    {

        await GameManagerSc.Instance.SaveAllAsync();

        string playerId = AuthManager.PlayerId;
        

        if (string.IsNullOrEmpty(playerId))
        {
            Debug.Log("Please enter a player ID");
            return;
        }

        Connection = new HubConnectionBuilder()
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
            GameManagerSc.LoadScene("CombatScene");
        });

        Connection.On<MatchData>("MatchUpdated", match =>
        {
            //MatchSession.CurrentMatch = match;
            //OnMatchUpdated?.Invoke(match);
            PendingMatchUpdate = match;
            HasPendingMatchUpdate = true;
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