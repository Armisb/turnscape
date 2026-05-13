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
        
        PopulateShop();
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
        var items = InventoryManSc.Instance.InventoryData;
        foreach (var item in items)
        {
            
        }
    }
    
    void PopulateShop()
    {
        foreach (var x in items)
        {
            ShopItemUI ui = Instantiate(itemUIPrefab, contentParent);
            ui.Setup(x, this, ShopItemUIMode.Buy);
        }
      
    }

    public async void BuyItem(ShopItem item)
    {
        // Check player coins
  
        // Subtract price
        
        // Add item to inventory
        await Networking.SendPostGeneric(
            $"store/buy/{item.id}",
            "",
            x => Debug.Log("Buy item: " + x),
            x => Debug.LogError($" buy Error: {x}")
        );

        RefreshBuyShop();
    }

    public async void SellItem(ShopItem item)
    {
        // Check player coins
  
        // Subtract price
        
        // Add item to inventory
        await Networking.SendPostGeneric(
            "store",
            JsonConvert.SerializeObject(item),
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
        GameManagerSc.Instance.LoadAllAsyncWithoutSaving();

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

