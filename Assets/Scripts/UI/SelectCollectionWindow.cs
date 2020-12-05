using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class SelectCollectionWindow : PoppingOutWindowBehaviour<SelectCollectionWindow> {
    [SerializeField] private CollectionTileUI _collectionTileUIPrefab;
    [SerializeField] private Transform _moreCollectionsSoonTile;

    private List<CollectionTileUI> _collectionTileUIList = new List<CollectionTileUI>();

    protected override void Awake() {
        base.Awake();
        _collectionTileUIPrefab.gameObject.SetActive(false);
        GameManager.OnGameViewChanged += OnGameViewChanged;
        _closeButton.onClick.AddListener(GoBackToMainMenu);
        _closeWindowClickableArea.onClick.AddListener(GoBackToMainMenu);
    }

    public override void ShowWindow() {
        base.ShowWindow();
        RefreshCollectionTiles();
    }

    private void RefreshCollectionTiles() {
        TurnOffAllTiles();
        var collectionsData = GameResourcesDatabase.GetCollectionsData();
        for (int i = 0; i < collectionsData.Count; i++) {
            if (_collectionTileUIList.Count <= i) {
                _collectionTileUIList.Add(
                    Instantiate(_collectionTileUIPrefab, _collectionTileUIPrefab.transform.parent));
            }

            _collectionTileUIList[i].Init(collectionsData[i]);
        }

        _moreCollectionsSoonTile.SetAsLastSibling();
    }

    private void TurnOffAllTiles() {
        foreach (var collectionTileUI in _collectionTileUIList) {
            collectionTileUI.gameObject.SetActive(false);
        }
    }

    private void OnGameViewChanged(GameViewType gameViewType) {
        if (gameViewType == GameViewType.CollectionsView) {
            ShowWindow();
        }
    }

    private void GoBackToMainMenu() {
        GameManager.Instance.OnBack();
    }

    protected override void OnDestroy() {
        GameManager.OnGameViewChanged -= OnGameViewChanged;
        base.OnDestroy();
    }
}
