using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour {
    [SerializeField] private GameObject _lockedArea;
    [SerializeField] private Image _itemIcon;

    public void InitShopItem(ShopItemData shopItemData) {
        _itemIcon.sprite = shopItemData.itemShopImage;
        _lockedArea.SetActive(false);
        gameObject.SetActive(true);
    }
}
