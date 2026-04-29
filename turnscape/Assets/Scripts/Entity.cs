using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Entity : MonoBehaviour
{
    public float CurrentHealth = 100f;
    public float MaxHealth = 100f;

    public Image healthBarFill;
    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;

    public float protection = 1f;
    public float damage = 1f;

    public TextMeshProUGUI healthBarText;
    public bool alive = true;

    private void Awake()
    {
        if (MaxHealth <= 0)
            MaxHealth = CurrentHealth;

        UpdateHealthBar();
    }

    public void SetStats(float currentHealth, float protectionValue, float damageValue)
    {
        float previousHealth = CurrentHealth;

        CurrentHealth = currentHealth;
        protection = protectionValue;
        damage = damageValue;

        alive = CurrentHealth > 0;

        UpdateHealthBar();

        float damageTaken = previousHealth - CurrentHealth;

        if (damageTaken > 0)
        {
            ShowDamageText(damageTaken);
        }
    }

    public void TakeDamage(float incomingDamage)
    {
        float damageTaken = Mathf.Max(incomingDamage - protection, 0);

        CurrentHealth -= damageTaken;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            alive = false;
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {

        if (CurrentHealth < 0) CurrentHealth = 0;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = CurrentHealth / MaxHealth;

            healthBarText.text = CurrentHealth.ToString("0");
        }
    }

    private void ShowDamageText(float damageAmount)
    {
        if (damageTextPrefab == null || damageTextSpawnPoint == null)
            return;

        if (damageTextSpawnPoint.parent == null)
            return;


        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0f
        );

        GameObject damageText = Instantiate(
            damageTextPrefab,
            damageTextSpawnPoint.position + randomOffset,
            Quaternion.identity,
            damageTextSpawnPoint.parent
        );

        DamageTextSc damageTextSc = damageText.GetComponent<DamageTextSc>();
        if (damageAmount <= 0 && damageTextSc != null) 
        {
            damageTextSc.SetText("*miss*");
        }
        else if (damageTextSc != null)
        {
            damageTextSc.SetText(damageAmount.ToString("0"));
        }
    }
}