using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour {
    [SerializeField] private GameObject _lockedArea;
    [SerializeField] private Image _itemIcon;

    private bool _isItemUnlocked;

    public void InitShopItem(ShopItemData shopItemData) {
        _itemIcon.sprite = shopItemData.itemShopImage;
        _isItemUnlocked = GameManager.Instance.IsShopItemUnlocked(shopItemData.itemID);
        _lockedArea.SetActive(!_isItemUnlocked);
        gameObject.SetActive(true);
    }
}
