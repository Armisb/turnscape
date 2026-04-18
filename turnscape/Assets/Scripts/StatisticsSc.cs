using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatisticsSc : LoaderBehaviour<StatisticsSc>
{
    public int damage;
    public int protection;

    private List<StatisticsUI> statisticsUIs = new List<StatisticsUI>();

    public override List<Type> Dependencies => new()
    {
        typeof(InventoryManSc)
    };

    protected override IEnumerator Load()
    {
        LocateStatisticsUI();

        yield return null;
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

        if (!InventoryManSc.Instance.InventoryData.ContainsKey("PlayerEquiped"))
            return;

        ItemData[] equippedItems = InventoryManSc.Instance.InventoryData["PlayerEquiped"].Values.ToArray();

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