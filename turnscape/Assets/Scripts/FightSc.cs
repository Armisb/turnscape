using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
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
    [SerializeField] private float enemyAttackDelay = 1f;
    
    
    void Awake()
    {
        canvas.enabled = true;
        turn = true;
        Player.protection = StatisticsSc.Instance.protection;
        Player.damage = StatisticsSc.Instance.damage;
        Enemy.protection = 2;
        Enemy.damage = 10;
    }

    public void Update()
    {
        CheckTurn();
    }

    public void DamageEnemy(float damageMultiplier)
    {
        Enemy.TakeDamage(Player.damage * damageMultiplier);
        turn = false;
        bool ended = CheckEnd();
        if (ended) return;
        DoDelayEnemyAttack(Enemy.damage);
    }

    public bool CheckEnd()
    {
        if (!Player.alive)
        {
            endText.text = "You Died!";
            panel.SetActive(true);
            
        }
        if (!Enemy.alive)
        {
            endText.text = "You Won!";
            panel.SetActive(true);
            
        }
        
    }

    private void CheckTurn()
    {
        canvas.enabled = turn;
    }
    
    
    private void DoDelayEnemyAttack(float damage)
    {
        StartCoroutine(DelayEnemyAttack(enemyAttackDelay, damage));
    }
    IEnumerator DelayEnemyAttack(float delayTime, float damage)
    {
        yield return new WaitForSeconds(delayTime);
        
        Player.TakeDamage(damage);
        turn = true;
        CheckEnd();
        //Wait for the specified delay time before continuing.
        //Do the action after the delay time has finished.
    }

 
}
