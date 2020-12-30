using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class ShopWindow : WindowBehaviour<ShopWindow> {
    [SerializeField]
    private Dictionary<ShopItemType, Button> _switchShopTypeButtons = new Dictionary<ShopItemType, Button>();
    [SerializeField] private ShopItemUI _shopItemUIPrefab;
    [SerializeField] private TextMeshProUGUI _shopTypeTitle;

    private readonly List<ShopItemUI> _shopItemUIs = new List<ShopItemUI>();
    private ShopItemType _currentShopItemType;

    protected override void Awake() {
        base.Awake();
        foreach (var button in _switchShopTypeButtons) {
            var shopType = button.Key;
            button.Value.onClick.AddListener(() => SwitchShopType(shopType));
        }

        _shopItemUIPrefab.gameObject.SetActive(false);
    }

    protected override void Start() {
        base.Start();
        SwitchShopType(_currentShopItemType);
    }

    private void SwitchShopType(ShopItemType shopType) {
        _currentShopItemType = shopType;
        _shopTypeTitle.text = GameTexts.GetShopName(shopType);
        TurnOffShopItemUIs();
        var shopItems = GameResourcesDatabase.GetShopItemData(_currentShopItemType);
        for (int i = 0; i < shopItems.Count; i++) {
            if (i >= _shopItemUIs.Count) {
                _shopItemUIs.Add(Instantiate(_shopItemUIPrefab, _shopItemUIPrefab.transform.parent));
            }

            _shopItemUIs[i].InitShopItem(shopItems[i]);
        }
    }

    private void TurnOffShopItemUIs() {
        foreach (var shopItemUI in _shopItemUIs) {
            shopItemUI.gameObject.SetActive(false);
        }
    }
}
