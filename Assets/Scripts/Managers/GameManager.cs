using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class GameManager : Singleton<GameManager> {
    public static event Action<GameViewType> OnGameViewChanged;

    [SerializeField] private Printer _printer;
    [SerializeField] private FiguresBookcase _figuresBookcase;

    private Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>> _voxelFiguresInfoData =
        new Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>>();

    public HashSet<string> UnlockedShopItems { get; private set; } = new HashSet<string>();

    private CollectionType _currentCollection;
    private VoxelFigure _currentVoxelFigure;
    private VoxelFigureInfoData _currentVoxelFigureInfoData;
    private GameViewType _currentGameViewType = GameViewType.None;

    public IEnumerator Start() {
        Input.multiTouchEnabled = false;
        Vibration.Init();
        _currentCollection = (CollectionType) PlayerPrefs.GetInt(SaveKey.CURRENT_COLLECTION, 0);
        LoadVoxelFiguresInfoData();
        LoadUnlockedShopItemsData();
        yield return null;
        ChangeGameView(GameViewType.MainMenu);
    }

    private void LoadVoxelFiguresInfoData() {
        if (PlayerPrefs.HasKey(SaveKey.PLAYER_SAVE)) {
            string json = PlayerPrefs.GetString(SaveKey.PLAYER_SAVE);
            var playerSaveData = JsonConvert.DeserializeObject<PlayerSaveJSON>(json,
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            try {
                foreach (var saveData in playerSaveData.voxelsData) {
                    _voxelFiguresInfoData.Add(saveData.Key, new Dictionary<string, VoxelFigureInfoData>());
                    foreach (var data in saveData.Value) {
                        _voxelFiguresInfoData[saveData.Key].Add(data.Key, data.Value);
                    }
                }
            } catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        } else {
            _voxelFiguresInfoData.Add(Constants.FIRST_COLLECTION, new Dictionary<string, VoxelFigureInfoData>());
            var firstFigure = GameResourcesDatabase.GetVoxelFiguresCollection(Constants.FIRST_COLLECTION)[0];
            var voxelFigureInfoData = new VoxelFigureInfoData {
                figureID = firstFigure.figureID,
                isUnlocked = true
            };
            _voxelFiguresInfoData[Constants.FIRST_COLLECTION].Add(firstFigure.figureID, voxelFigureInfoData);
            SaveVoxelsData();
        }
    }

    private void LoadUnlockedShopItemsData() {
        if (PlayerPrefs.HasKey(SaveKey.UNLOCKED_SHOP_ITEMS_SAVE)) {
            var json = PlayerPrefs.GetString(SaveKey.UNLOCKED_SHOP_ITEMS_SAVE);
            var unlockedShopItemsData = JsonConvert.DeserializeObject<UnlockedShopItemsJSON>(json,
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            UnlockedShopItems = unlockedShopItemsData.unlockedShopItems;
        } else {
            UnlockedShopItems = GameResourcesDatabase.GetStartUlockedShopItems();
            SaveUnlockedItems();
        }
    }

    private void LoadCurrentCollection() {
        var currentCollectionFigures = GameResourcesDatabase.GetVoxelFiguresCollection(_currentCollection);
        var currentCollectionData = GetCurrentCollectionVoxelFigureInfoData(currentCollectionFigures);
        _figuresBookcase.InitFigureSlots(currentCollectionFigures, currentCollectionData);
    }

    private List<VoxelFigureInfoData> GetCurrentCollectionVoxelFigureInfoData(
        List<VoxelFigureData> currentCollectionFigures) {
        var currentCollectionData = new List<VoxelFigureInfoData>();
        var shouldSaveData = false;
        if (!_voxelFiguresInfoData.ContainsKey(_currentCollection)) {
            _voxelFiguresInfoData.Add(_currentCollection, new Dictionary<string, VoxelFigureInfoData>());
            shouldSaveData = true;
        }

        foreach (var currentCollectionFigure in currentCollectionFigures) {
            if (!_voxelFiguresInfoData[_currentCollection].ContainsKey(currentCollectionFigure.figureID)) {
                _voxelFiguresInfoData[_currentCollection].Add(currentCollectionFigure.figureID,
                    new VoxelFigureInfoData(currentCollectionFigure.figureID));

                if (currentCollectionFigure == currentCollectionFigures[0]) {
                    _voxelFiguresInfoData[_currentCollection][currentCollectionFigure.figureID].isUnlocked = true;
                }

                shouldSaveData = true;
            }

            currentCollectionData.Add(_voxelFiguresInfoData[_currentCollection][currentCollectionFigure.figureID]);
        }

        if (shouldSaveData) {
            SaveVoxelsData();
        }

        return currentCollectionData;
    }

    public void ShowCollectionsView() {
        ChangeGameView(GameViewType.CollectionsView);
    }

    public void ShowSelectFigureView(CollectionType collectionType) {
        _currentCollection = collectionType;
        ShowSelectFigureView();
    }

    public void ShowSelectFigureView() {
        LoadCurrentCollection();
        ChangeGameView(GameViewType.SelectFigureView);
    }

    public void PrintNewFigure(VoxelFigureData voxelFigureData) {
        if (_currentGameViewType != GameViewType.SelectFigureView) {
            return;
        }

        RemoveCurrentPrintedFigure();
        _currentVoxelFigure = Instantiate(voxelFigureData.voxelFigure);
        _currentVoxelFigureInfoData = _voxelFiguresInfoData[_currentCollection][voxelFigureData.figureID];
        _printer.SetupPrintModel(_currentVoxelFigure);
        ChangeGameView(GameViewType.GameView);
    }

    private void RemoveCurrentPrintedFigure() {
        if (_currentVoxelFigure != null) {
            Destroy(_currentVoxelFigure.gameObject);
            _printer.RemoveModel();
        }
    }

    public string GetCurrentVoxelFigureName() {
        return GameResourcesDatabase.GetVoxelFigureName(_currentVoxelFigureInfoData.figureID);
    }

    public void SaveFigureData(VoxelFigure finishedModel, out float completionPercent) {
        completionPercent = finishedModel.GetPercentageOfCorrectVoxels();
        var modelChanged = _currentVoxelFigureInfoData.FinishModel(finishedModel, completionPercent);
        var newFigureUnlocked = TryUnlockNextFigure();

        if (modelChanged || newFigureUnlocked) {
            SaveVoxelsData();
        }
    }

    private bool TryUnlockNextFigure() {
        var nextFigureID = "";
        var currentCollectionFigures = GameResourcesDatabase.GetVoxelFiguresCollection(_currentCollection);
        var indexOfCurrent =
            currentCollectionFigures.FindIndex(v => v.figureID == _currentVoxelFigureInfoData.figureID);

        if (indexOfCurrent > -1 && indexOfCurrent < currentCollectionFigures.Count - 1) {
            nextFigureID = currentCollectionFigures[indexOfCurrent + 1].figureID;
        }

        if (_voxelFiguresInfoData[_currentCollection].ContainsKey(nextFigureID)) {
            _voxelFiguresInfoData[_currentCollection][nextFigureID].isUnlocked = true;
            return true;
        }

        return false;
    }

    public int GetCurrentAmountOfStars() {
        var stars = 0;
        foreach (var collectionData in _voxelFiguresInfoData) {
            foreach (var data in collectionData.Value) {
                var completionPercent = data.Value.completionPercent;
                stars += StarsControllerUI.GetCountOfStarsToShow(completionPercent);
            }
        }

        return stars;
    }

    private void ChangeGameView(GameViewType gameViewType) {
        if (gameViewType != _currentGameViewType) {
            _currentGameViewType = gameViewType;
            OnGameViewChanged?.Invoke(_currentGameViewType);
        }
    }

    public void OnBack() {
        switch (_currentGameViewType) {
            case GameViewType.CollectionsView:
                GoBackToMainMenu();
                break;
            case GameViewType.SelectFigureView:
                GoBackToCollectionsView();
                break;
            case GameViewType.GameView:
                GoBackToSelectFigureView();
                break;
        }
    }

    private void GoBackToSelectFigureView() {
        ChangeGameView(GameViewType.SelectFigureView);
    }

    private void GoBackToCollectionsView() {
        ChangeGameView(GameViewType.CollectionsView);
    }

    private void GoBackToMainMenu() {
        RemoveCurrentPrintedFigure();
        ChangeGameView(GameViewType.MainMenu);
    }

    #region ItemShop

    public bool IsShopItemUnlocked(string shopItemID) {
        return UnlockedShopItems.Contains(shopItemID);
    }

    public void UnlockedShopItem(string shopItemID) {
        UnlockedShopItems.Add(shopItemID);
        SaveUnlockedItems();
    }

    #endregion

    #region SavingData

    private void SaveVoxelsData() {
        var playerData = new PlayerSaveJSON(_voxelFiguresInfoData);
        string json = JsonConvert.SerializeObject(playerData,
            new JsonSerializerSettings
                {NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        PlayerPrefs.SetString(SaveKey.PLAYER_SAVE, json);
    }

    private void SaveUnlockedItems() {
        var unlockedItemsData = new UnlockedShopItemsJSON(UnlockedShopItems);
        string json = JsonConvert.SerializeObject(unlockedItemsData, new JsonSerializerSettings
            {NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        PlayerPrefs.SetString(SaveKey.UNLOCKED_SHOP_ITEMS_SAVE, json);
    }

    private void SaveAllData() {
        SaveVoxelsData();
        SaveUnlockedItems();
    }

    #endregion

    #region Test Buttons

    public void UnlockAllFigures() {
        var collections = (CollectionType[]) Enum.GetValues(typeof(CollectionType));
        foreach (var collection in collections) {
            if (!_voxelFiguresInfoData.ContainsKey(collection)) {
                _voxelFiguresInfoData.Add(collection, new Dictionary<string, VoxelFigureInfoData>());
            }

            var figuresInCollection = GameResourcesDatabase.GetVoxelFiguresCollection(collection);
            foreach (var figure in figuresInCollection) {
                if (!_voxelFiguresInfoData[collection].ContainsKey(figure.figureID)) {
                    _voxelFiguresInfoData[collection].Add(figure.figureID, new VoxelFigureInfoData(figure.figureID));
                }

                _voxelFiguresInfoData[collection][figure.figureID].isUnlocked = true;
                _voxelFiguresInfoData[collection][figure.figureID].completionPercent = 1;
            }
        }

        SaveVoxelsData();
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DeleteSave() {
        PlayerPrefs.DeleteAll();
        ResetGame();
    }

    #endregion
}
