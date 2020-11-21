using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private Printer _printer;
    [SerializeField] private FiguresBookcase _figuresBookcase;
    [SerializeField] private CameraController _cameraController;

    private Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>> _voxelFiguresInfoData =
        new Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>>();

    private CollectionType _currentCollection = CollectionType.People;
    private VoxelFigure _currentVoxelFigure;
    private VoxelFigureInfoData _currentVoxelFigureInfoData;

    public void Start() {
        Input.multiTouchEnabled = false;
        Vibration.Init();
        LoadVoxelFiguresInfoData();
        LoadCurrentCollection();
        _cameraController.MoveCameraToGamePrinterView();
        MainMenuWindow.Instance.ShowWindow();
    }

    private void LoadVoxelFiguresInfoData() {
        if (PlayerPrefs.HasKey(SaveKey.PLAYER_SAVE)) {
            string json = PlayerPrefs.GetString(SaveKey.PLAYER_SAVE);
            var playerSaveData = JsonConvert.DeserializeObject<PlayerSaveJSON>(json,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

            try {
                foreach (var saveData in playerSaveData.voxelsData) {
                    _voxelFiguresInfoData.Add(saveData.Key, new Dictionary<string, VoxelFigureInfoData>());
                    foreach (var data in saveData.Value) {
                        _voxelFiguresInfoData[saveData.Key].Add(data.figureID, data);
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
        _cameraController.MoveCameraToCollectionsBookcaseView();
    }

    public void PrintNewFigure(VoxelFigureData voxelFigureData) {
        if (_currentVoxelFigure != null) {
            Destroy(_currentVoxelFigure.gameObject);
        }

        _currentVoxelFigure = Instantiate(voxelFigureData.voxelFigure);
        _currentVoxelFigureInfoData = _voxelFiguresInfoData[_currentCollection][voxelFigureData.figureID];
        _printer.SetupPrintModel(_currentVoxelFigure);
        _cameraController.MoveCameraToGamePrinterView();
    }

    public void LoadNextFigure() {
        // InitNewFigure();
    }

    public string GetCurrentVoxelFigureName() {
        return GameResourcesDatabase.GetVoxelFigureName(_currentVoxelFigureInfoData.figureID);
    }

    private void SaveVoxelsData() {
        var playerData = new PlayerSaveJSON(_voxelFiguresInfoData);
        string json = JsonConvert.SerializeObject(playerData,
            new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
        PlayerPrefs.SetString(SaveKey.PLAYER_SAVE, json);
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DeleteSave() {
        PlayerPrefs.DeleteAll();
        ResetGame();
    }
}
