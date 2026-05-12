using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    private ShopItem item;
    private ShopManager shopManager;

    public void Setup(ShopItem newItem, ShopManager manager)
    {
        item = newItem;
        shopManager = manager;

        iconImage.sprite = FileReader.GetTextureSprite(item.category + ".png");
        nameText.text = item.name;
        priceText.text = item.price + " coins";
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener((() => shopManager.BuyItem(item)));
    }
}
