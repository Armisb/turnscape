using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> items;
    public Transform contentParent;
    public ShopItemUI itemUIPrefab;

    private async void GetItems()
    {
        string json = "";
        
        await Networking.SendGetGeneric(
            "store",
            "",
            response => json = response,
            error => Debug.LogError(error)
        );

        
        items = JsonConvert.DeserializeObject<List<ShopItem>>(json);

        foreach (ShopItem item in items)
        {
            Debug.Log(item.name);
        }
        
        PopulateShop();
    }

    public void RefreshShop()
    {
        items =  new List<ShopItem>();
        GetItems();
    }
    
    void PopulateShop()
    {
        foreach (var x in items)
        {
            ShopItemUI ui = Instantiate(itemUIPrefab, contentParent);
            ui.Setup(x, this);
        }
      
    }

    public void BuyItem(ShopItem item)
    {
        // Check player coins
        
        
        // Subtract price
        // Add item to inventory
        Debug.Log("Bought: " + item.name);
        RefreshShop();
    }

    public void CloseShop()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenShop()
    {
        RefreshShop();
        this.gameObject.SetActive(true);
        //PopulateShop();
    }
}

