using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> items;
    public TMP_Text errorText;
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

    private void ResetErrorText()
    {
        errorText.text = "";
    }

    private void SetErrorText(string error)
    {
        errorText.text = error;
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
            x => this.SetErrorText(x)
        );

        RefreshBuyShop();
    }

    public async void SellItem(string itemId, decimal price)
    {
        

        SellItemRequest request = new SellItemRequest
        {
            ItemId = itemId,
            Price = (float)price
        };

        // Add item to inventory
        await Networking.SendPostGeneric(
            "store",
            request,            
            x => Debug.Log($"Sell item: " + x),
            x => SetErrorText(x)
        );

        RefreshSellShop();
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
        GameManagerSc.Instance.LoadAllAsyncWithoutSaving();

        this.gameObject.SetActive(false);
    }

    public void OpenShop()
    {
        RefreshBuyShop();
        this.gameObject.SetActive(true);
        ResetErrorText();
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
        ResetErrorText();
    }
}

[System.Serializable]
public class SellItemRequest
{
    public float Price;
    public string ItemId;
}