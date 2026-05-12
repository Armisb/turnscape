using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> items;
    public Transform contentParent;
    public ShopItemUI itemUIPrefab;
    private ShopItemUIMode mode = ShopItemUIMode.Buy;

    private async void GetShopItems()
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
        
        PopulateBuyShop(items);
    }

    public void RefreshBuyShop()
    {
        ClearShop();
        GetShopItems();
    }
    
    public void RefreshSellShop()
    {
        ClearShop();
        GetInventoryItems();
    }

    private void GetInventoryItems()
    {
        var items = InventoryManSc.Instance.InventoryData["PlayerInventory"];
        foreach (var item in items.Values)
        {
            if (item.id != null)
            {
                ShopItemUI ui = Instantiate(itemUIPrefab, contentParent);
                // reiiktu pakeisti, koki price uzdet, tyngiu dabar
                ui.Setup(this, ShopItemUIMode.Sell, item.category, 1337, item.category, item.id);
            }
        
        }
    }
    
    void PopulateBuyShop(List<ShopItem> items)
    {
        foreach (var x in items)
        {
            ShopItemUI ui = Instantiate(itemUIPrefab, contentParent);
            ui.Setup(this, ShopItemUIMode.Buy, x.name, x.price, x.category, x.id);
        }
    }

    public async void BuyItem(string itemId)
    {
        // Check player coins
  
        // Subtract price
        
        // Add item to inventory
        await Networking.SendPostGeneric(
            $"store/buy/{itemId}",
            "",
            x => Debug.Log("Buy item: " + x),
            x => Debug.LogError($" buy Error: {x}")
        );

        RefreshBuyShop();
    }

    public async void SellItem(string itemId)
    {
        // Check player coins
  
        // Subtract price
        
        // Add item to inventory
        await Networking.SendPostGeneric(
            $"store/sell/{itemId}",
            "",
            x => Debug.Log($"Sell item: " + x),
            x => Debug.LogError($" sell Error: {x}")
        );

        RefreshBuyShop();
    }
    
    private void ClearShop()
    {
        items.Clear();
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void CloseShop()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenShop()
    {
        RefreshBuyShop();
        this.gameObject.SetActive(true);
        //PopulateShop();
    }

    public void SwitchShop()
    {
        if (mode == ShopItemUIMode.Buy)
        {
            RefreshSellShop();
            mode = ShopItemUIMode.Sell;
        }
        else
        {
            RefreshBuyShop();
            mode = ShopItemUIMode.Buy;
        }
    }
}

