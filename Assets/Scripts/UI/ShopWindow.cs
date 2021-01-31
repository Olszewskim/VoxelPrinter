using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class ShopWindow : WindowBehaviour<ShopWindow> {
    [SerializeField]
    private Dictionary<ShopItemType, Button> _switchShopTypeButtons = new Dictionary<ShopItemType, Button>();
    [SerializeField] private ShopItemUI _shopItemUIPrefab;
    [SerializeField] private TextMeshProUGUI _shopTypeTitle;
    [SerializeField] private Button _unlockRandomItemButton;

    private readonly List<ShopItemUI> _shopItemUIs = new List<ShopItemUI>();
    private ShopItemType _currentShopItemType;

    private const int MIN_UNLOCK_STEPS = 5;
    private const int MAX_UNLOCK_STEPS = 11;
    private readonly WaitForSeconds _timeBetweenUnlockAnimations = new WaitForSeconds(0.15f);

    protected override void Awake() {
        base.Awake();
        foreach (var button in _switchShopTypeButtons) {
            var shopType = button.Key;
            button.Value.onClick.AddListener(() => SwitchShopType(shopType));
        }

        _shopItemUIPrefab.gameObject.SetActive(false);
        _unlockRandomItemButton.onClick.AddListener(UnlockRandomShopItem);
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

        RefreshUnlockRandomShopItemButton();
    }

    private void RefreshUnlockRandomShopItemButton() {
        var itemsToUnlock = GetItemsToUnlock();
        var shouldButttonBeEnabled = itemsToUnlock.Count > 0;
        _unlockRandomItemButton.gameObject.SetActive(shouldButttonBeEnabled);
    }

    private void TurnOffShopItemUIs() {
        foreach (var shopItemUI in _shopItemUIs) {
            shopItemUI.gameObject.SetActive(false);
        }
    }

    private void UnlockRandomShopItem() {
        var itemsToUnlock = GetItemsToUnlock();
        StartCoroutine(UnlockAnimation(itemsToUnlock));
    }

    private IEnumerator UnlockAnimation(List<ShopItemData> itemsToUnlock) {
        ChangeButtonsInteractivity(false);
        ShopItemData randomElement = null;
        ShopItemData lastRandomElement = null;

        var animationSteps =
            itemsToUnlock.Count > 1 ? Randomizer.GetRandomNumber(MIN_UNLOCK_STEPS, MAX_UNLOCK_STEPS) : 1;

        for (int i = 0; i < animationSteps; i++) {
            do {
                randomElement = itemsToUnlock.GetRandomElement();
            } while (randomElement == lastRandomElement);

            lastRandomElement = randomElement;
            var shopItemUI = _shopItemUIs.FirstOrDefault(ui => ui.IsShopItemTile(randomElement.itemID));
            shopItemUI?.Mark();
            Vibration.VibratePop();
            //SoundsManager.Instance.PlaySound(SoundType.ShopDraw);
            yield return _timeBetweenUnlockAnimations;
            shopItemUI?.Unmark();
        }

        // SoundsManager.Instance.PlaySound(SoundType.UnlockItem);
        GameManager.Instance.UnlockedShopItem(randomElement.itemID);
        SwitchShopType(_currentShopItemType);
        ChangeButtonsInteractivity(true);
    }

    private List<ShopItemData> GetItemsToUnlock() {
        var shopItems = new List<ShopItemData>(GameResourcesDatabase.GetShopItemData(_currentShopItemType));
        var unlockedItems = GameManager.Instance.UnlockedShopItems;
        foreach (var unlockedItem in unlockedItems) {
            shopItems.RemoveAll(i => i.itemID == unlockedItem);
        }

        return shopItems;
    }

    private void ChangeButtonsInteractivity(bool shouldBeActive) {
        foreach (var switchShopButton in _switchShopTypeButtons) {
            switchShopButton.Value.interactable = shouldBeActive;
        }

        _closeButton.interactable = shouldBeActive;
        _closeWindowClickableArea.interactable = shouldBeActive;
    }
}
