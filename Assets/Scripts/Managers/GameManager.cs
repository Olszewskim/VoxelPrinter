using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private List<VoxelFigureData> _voxelFiguresData;
    [SerializeField] private Printer _printer;

    private Dictionary<CollectionType, List<VoxelFigureData>> _voxelFiguresDataDictionary;
    private CollectionType _currentCollection = CollectionType.Animals;
    private int _currentFigureID;
    private VoxelFigure _currentVoxelFigure;

    public void Start() {
        Input.multiTouchEnabled = false;
        Vibration.Init();
        InitVoxelFiguresDictionary();
        InitNewFigure();
    }

    private void InitVoxelFiguresDictionary() {
        _voxelFiguresDataDictionary = new Dictionary<CollectionType, List<VoxelFigureData>>();
        foreach (var voxelData in _voxelFiguresData) {
            if (!_voxelFiguresDataDictionary.ContainsKey(voxelData.collectionType)) {
                _voxelFiguresDataDictionary[voxelData.collectionType] = new List<VoxelFigureData>();
            }

            _voxelFiguresDataDictionary[voxelData.collectionType].Add(voxelData);
        }
    }

    private void InitNewFigure() {
        if (_currentVoxelFigure != null) {
            Destroy(_currentVoxelFigure.gameObject);
        }

        _currentVoxelFigure = Instantiate(_voxelFiguresData[_currentFigureID].voxelFigure);
        _printer.SetupPrintModel(_currentVoxelFigure);
    }

    public void LoadNextFigure() {
        _currentFigureID = Mathf.Min(_currentFigureID + 1, _voxelFiguresDataDictionary[_currentCollection].Count - 1);
        InitNewFigure();
    }

    public string GetCurrentVoxelFigureName() {
        return _voxelFiguresDataDictionary[_currentCollection][_currentFigureID].figureName;
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
