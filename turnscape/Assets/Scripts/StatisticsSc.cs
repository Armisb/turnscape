using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StatisticsSc : MonoBehaviour
{
    public static StatisticsSc Instance { get; private set; }

    public int damage;
    public int protection;

    private List<StatisticsUI> statisticsUIs = new List<StatisticsUI>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LocateStatisticsUI()
    {
        statisticsUIs = FindObjectsByType<StatisticsUI>(FindObjectsSortMode.None).ToList();
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        damage = 1;
        protection = 0;

        if (!InventoryManSc.Instance.InventoryData.ContainsKey("PlayerEquipped"))
            return;

        ItemData[] equippedItems = InventoryManSc.Instance.InventoryData["PlayerEquipped"].Values.ToArray();

        foreach (var item in equippedItems)
        {
            if (item != null)
            {
                damage += item.damage;
                protection += item.protection;
            }
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        foreach (var ui in statisticsUIs)
        {
            if (ui.damageText != null)
                ui.damageText.text = "Damage: " + damage;

            if (ui.protectionText != null)
                ui.protectionText.text = "Protection: " + protection;
        }
    }
}