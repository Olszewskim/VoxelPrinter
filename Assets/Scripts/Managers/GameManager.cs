using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private Printer _printer;
    [SerializeField] private FiguresBookcase _figuresBookcase;

    private Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>> _voxelFiguresInfoData =
        new Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>>();

    private CollectionType _currentCollection = CollectionType.People;
    private List<VoxelFigureData> _currentCollectionFigures;
    private int _currentFigureID;
    private VoxelFigure _currentVoxelFigure;

    public void Start() {
        Input.multiTouchEnabled = false;
        Vibration.Init();
        LoadVoxelFiguresInfoData();
        LoadCurrentCollection();
        InitNewFigure();
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
        _currentCollectionFigures = GameResourcesDatabase.GetVoxelFiguresCollection(_currentCollection);
        var currentCollectionData = GetCurrentCollectionVoxelFigureInfoData();
        _figuresBookcase.InitFigureSlots(_currentCollectionFigures, currentCollectionData);
    }

    private List<VoxelFigureInfoData> GetCurrentCollectionVoxelFigureInfoData() {
        var currentCollectionData = new List<VoxelFigureInfoData>();
        var shouldSaveData = false;
        if (!_voxelFiguresInfoData.ContainsKey(_currentCollection)) {
            _voxelFiguresInfoData.Add(_currentCollection, new Dictionary<string, VoxelFigureInfoData>());
            shouldSaveData = true;
        }

        foreach (var currentCollectionFigure in _currentCollectionFigures) {
            if (!_voxelFiguresInfoData[_currentCollection].ContainsKey(currentCollectionFigure.figureID)) {
                _voxelFiguresInfoData[_currentCollection].Add(currentCollectionFigure.figureID, new VoxelFigureInfoData(currentCollectionFigure.figureID));
                shouldSaveData = true;
            }

            currentCollectionData.Add(_voxelFiguresInfoData[_currentCollection][currentCollectionFigure.figureID]);
        }

        if (shouldSaveData) {
            SaveVoxelsData();
        }

        return currentCollectionData;
    }

    private void InitNewFigure() {
        if (_currentVoxelFigure != null) {
            Destroy(_currentVoxelFigure.gameObject);
        }

        _currentVoxelFigure = Instantiate(_currentCollectionFigures[_currentFigureID].voxelFigure);
        _printer.SetupPrintModel(_currentVoxelFigure);
    }

    public void LoadNextFigure() {
        _currentFigureID = Mathf.Min(_currentFigureID + 1, _currentCollectionFigures.Count - 1);
        InitNewFigure();
    }

    public string GetCurrentVoxelFigureName() {
        return _currentCollectionFigures[_currentFigureID].figureName;
    }

    public void SaveVoxelsData() {
        var playerData = new PlayerSaveJSON(_voxelFiguresInfoData);
        string json = JsonConvert.SerializeObject(playerData,
            new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
        PlayerPrefs.SetString(SaveKey.PLAYER_SAVE, json);
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
