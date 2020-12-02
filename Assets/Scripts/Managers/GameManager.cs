using System;
using System.Collections;
using System.Collections.Generic;
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

    private CollectionType _currentCollection;
    private VoxelFigure _currentVoxelFigure;
    private VoxelFigureInfoData _currentVoxelFigureInfoData;
    private GameViewType _currentGameViewType = GameViewType.None;

    public IEnumerator Start() {
        Input.multiTouchEnabled = false;
        Vibration.Init();
        _currentCollection = (CollectionType) PlayerPrefs.GetInt(SaveKey.CURRENT_COLLECTION, 0);
        LoadVoxelFiguresInfoData();
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

                //TODO: Remove it - temp unlock 1st figure
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
        LoadCurrentCollection();
        ChangeGameView(GameViewType.CollectionView);
    }

    public void PrintNewFigure(VoxelFigureData voxelFigureData) {
        if (!IsInCollectionsView()) {
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

    private void SaveVoxelsData() {
        var playerData = new PlayerSaveJSON(_voxelFiguresInfoData);
        string json = JsonConvert.SerializeObject(playerData,
            new JsonSerializerSettings
                {NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        PlayerPrefs.SetString(SaveKey.PLAYER_SAVE, json);
    }

    private void ChangeGameView(GameViewType gameViewType) {
        if (gameViewType != _currentGameViewType) {
            _currentGameViewType = gameViewType;
            OnGameViewChanged?.Invoke(_currentGameViewType);
        }
    }

    public bool IsInCollectionsView() {
        return _currentGameViewType == GameViewType.CollectionView;
    }

    public bool IsInGameView() {
        return _currentGameViewType == GameViewType.GameView;
    }

    public void GoBackToCollectionView() {
        ChangeGameView(GameViewType.CollectionView);
    }

    public void GoBackToMainMenu() {
        RemoveCurrentPrintedFigure();
        ChangeGameView(GameViewType.MainMenu);
    }

    #region Test Buttons

    public void SwitchToAnimalsCollection() {
        _currentCollection = CollectionType.Animals;
        PlayerPrefs.SetInt(SaveKey.CURRENT_COLLECTION, (int) _currentCollection);
        ResetGame();
    }

    public void SwitchToPeopleCollection() {
        _currentCollection = CollectionType.People;
        PlayerPrefs.SetInt(SaveKey.CURRENT_COLLECTION, (int) _currentCollection);
        ResetGame();
    }

    public void SwitchToPlantsCollection() {
        _currentCollection = CollectionType.Plants;
        PlayerPrefs.SetInt(SaveKey.CURRENT_COLLECTION, (int) _currentCollection);
        ResetGame();
    }

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
