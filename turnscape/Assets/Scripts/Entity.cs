
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Entity : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float CurrentHealth = 100f;
    public float MaxHealth;
    public Image healthBarFill;
    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;
    public float protection = 1f;
    public float damage = 1f;
    public TextMeshProUGUI healthBarText;
    public bool alive = true;
    

    private void Awake()
    {
        MaxHealth =  CurrentHealth;
    }
    
    private void UpdateHealthBar()
    {
        healthBarFill.fillAmount = CurrentHealth / MaxHealth;
        healthBarText.text = CurrentHealth.ToString("0");
    }
    
    public void TakeDamage(float damage)
    {
        var damageTaken = Math.Max(damage - protection, 0);
        CurrentHealth -= damageTaken;
        ShowDamageText(damageTaken);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            alive = false;
        }
        UpdateHealthBar();
        
    }
    
    private void ShowDamageText(float damage)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        
        GameObject damageText = Instantiate(
            damageTextPrefab,
            damageTextSpawnPoint.position + randomOffset,
            Quaternion.identity,
            damageTextSpawnPoint.parent
        );
        damageText.GetComponent<DamageTextSc>().SetText(damage.ToString("0"));
    }
}
