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

    private ShopItem item;
    private ShopManager shopManager;

    public void Setup(ShopItem newItem, ShopManager manager, ShopItemUIMode mode)
    {
        item = newItem;
        shopManager = manager;

        iconImage.sprite = FileReader.GetTextureSprite(item.name + ".png");
        nameText.text = item.name;
        priceText.text = item.price + " coins";
        //btn.onClick.RemoveAllListeners();
        if (mode == ShopItemUIMode.Buy)
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            btn.onClick.AddListener(() => shopManager.BuyItem(item));
        }
        else
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Sell";
            btn.onClick.AddListener(() => shopManager.SellItem(item));
        }

    }
}

public enum ShopItemUIMode
{
    Buy,
    Sell
}