using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_InputField priceField;
    public Button btn;

    private ShopManager shopManager;

    public void Setup(ShopManager manager, ShopItemUIMode mode, string itemName, float itemPrice, string itemCategory, string itemId, string itemLevel)
    {
        shopManager = manager;

        iconImage.sprite = FileReader.GetTextureSprite(itemName + ".png");
        nameText.text = itemName + " lvl " + itemLevel;
        priceText.text = itemPrice + " coins";
        btn.onClick.RemoveAllListeners();
        if (mode == ShopItemUIMode.Buy)
        {
            priceField.text = itemPrice.ToString("0.00");
            priceField.readOnly = true;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            btn.onClick.AddListener(() => shopManager.BuyItem(itemId));
            //shopManager.RefreshBuyShop();
        }
        else
        {
            priceField.readOnly = false;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Sell";
            btn.onClick.AddListener(() => shopManager.SellItem(itemId, decimal.Parse(priceField.text)));
            //shopManager.RefreshSellShop();
        }
    }
}

public enum ShopItemUIMode
{
    Buy,
    Sell
}