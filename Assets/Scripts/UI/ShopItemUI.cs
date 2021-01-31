using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour {
    [SerializeField] private GameObject _lockedArea;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _frame;

    private bool _isItemUnlocked;
    private ShopItemData _currentShopItemData;

    public void InitShopItem(ShopItemData shopItemData) {
        _currentShopItemData = shopItemData;
        _itemIcon.sprite = shopItemData.itemShopImage;
        _isItemUnlocked = GameManager.Instance.IsShopItemUnlocked(shopItemData.itemID);
        _lockedArea.SetActive(!_isItemUnlocked);
        gameObject.SetActive(true);
        Unmark();
    }

    public void Mark() {
        _frame.gameObject.SetActive(true);
    }

    public void Unmark() {
        _frame.gameObject.SetActive(false);
    }

    public bool IsShopItemTile(string itemID) {
        return _currentShopItemData?.itemID == itemID;
    }
}
