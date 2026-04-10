using UnityEngine;
using UnityEngine.UI;

public class FightSc : MonoBehaviour
{
    public Image healthBarFill;
    public float MaxHealth = 100f;
    public float CurrentHealth;

    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;
    
    void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        UpdateHealthBar();
        Debug.Log("Took some damage! Current health: " + CurrentHealth);
        
        ShowDamageText(damage);
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
    
    private void UpdateHealthBar()
    {
        healthBarFill.fillAmount = CurrentHealth / MaxHealth;
    }

 
}
