using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class FightSc : MonoBehaviour
{

    public Entity Player;
    public Entity Enemy;
    public Canvas canvas;
    public bool turn;
    public GameObject panel;
    [SerializeField] private TextMeshProUGUI endText;
    public TextMeshProUGUI TurnText;
    public TextMeshProUGUI MyHp;
    public TextMeshProUGUI EnemyHp;
    

    private void Start()
    {
        LoadMatch();
    }

    private void Update()
    {
        if (QueueService.HasPendingMatchUpdate)
        {
            QueueService.HasPendingMatchUpdate = false;
            HandleMatchUpdated(QueueService.PendingMatchUpdate);
        }
        RefreshTurn();
        //CheckEnd();
    }

    private void OnEnable()
    {
        QueueService.OnMatchUpdated += HandleMatchUpdated;
    }

    private void OnDisable()
    {
        QueueService.OnMatchUpdated -= HandleMatchUpdated;
    }

    private void LoadMatch()
    {
        MatchData match = MatchSession.CurrentMatch;

        if (match == null)
        {
            Debug.LogError("No match data found!");
            return;
        }

        Debug.Log("Match ID: " + match.Id);
        Debug.Log("My ID: " + MatchSession.MyPlayerId);
        Debug.Log("I am player one: " + MatchSession.IsPlayerOne);
        Debug.Log("My turn: " + MatchSession.IsMyTurn);

        ApplyStatsFromMatch(match);
        HandleMatchUpdated(match);
    }

    public void HandleMatchUpdated(MatchData match)
    {
        MatchSession.CurrentMatch = match;

        ApplyStatsFromMatch(match);
        RefreshTurn();
        CheckEnd();
    }

    private void ApplyStatsFromMatch(MatchData match)
    {
        List<int> myStats = MatchSession.MyStats;
        List<int> enemyStats = MatchSession.EnemyStats;

        // Example stats:
        // [0] = damage
        // [1] = protection
        // [2] = HP

        Player.SetStats(myStats[2], myStats[1], myStats[0]);
        Enemy.SetStats(enemyStats[2], enemyStats[1], enemyStats[0]);

    }

    private void RefreshTurn()
    {
        canvas.enabled = MatchSession.IsMyTurn;
        if (MatchSession.IsMyTurn)
        {
            TurnText.text = "My turn!";
        }
        else
        {
            TurnText.text = "Enemy turn!";
        }
        CheckEnd();
    }

    public async void DamageEnemy(float damageMultiplier)
    {
        if (!MatchSession.IsMyTurn)
        {
            Debug.Log("Not your turn!");
            return;
        }
        await QueueService.Connection.InvokeAsync("Attack", MatchSession.CurrentMatch.Id);
        canvas.enabled = false;
    }

    public void CheckEnd()
    {
        int myHp = MatchSession.MyStats[2];
        int enemyHp = MatchSession.EnemyStats[2];

        if (myHp <= 0)
        {
            endText.text = "You Lost!";
            panel.SetActive(true);
            return;
        }
        if (enemyHp <= 0) {
            endText.text = "You Won!";
            panel.SetActive(true);
            return;
        }
    }
}

