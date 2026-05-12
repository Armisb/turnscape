using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button btn;
    
    private ShopManager shopManager;

    public void Setup(ShopManager manager, ShopItemUIMode mode, string itemName, float itemPrice, string itemCategory, string itemId)
    {
        shopManager = manager;

        iconImage.sprite = FileReader.GetTextureSprite(itemCategory + ".png");
        nameText.text = itemName;
        priceText.text = itemPrice + " coins";
        btn.onClick.RemoveAllListeners();
        if (mode == ShopItemUIMode.Buy)
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            btn.onClick.AddListener(() => shopManager.BuyItem(itemId));
        }
        else
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Sell";
            btn.onClick.AddListener(() => shopManager.SellItem(itemId));
        }

    }
}

public enum ShopItemUIMode
{
    Buy,
    Sell
}