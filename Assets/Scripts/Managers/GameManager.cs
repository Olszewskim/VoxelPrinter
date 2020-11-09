using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private Printer _printer;
    [SerializeField] private FiguresBookcase _figuresBookcase;

    private CollectionType _currentCollection = CollectionType.Animals;
    private List<VoxelFigureData> _currentCollectionFigures;
    private int _currentFigureID;
    private VoxelFigure _currentVoxelFigure;


    public void Start() {
        Input.multiTouchEnabled = false;
        Vibration.Init();
        LoadCurrentCollection();
        InitNewFigure();
    }



    private void LoadCurrentCollection() {
        _currentCollectionFigures = GameResourcesDatabase.GetVoxelFiguresCollection(_currentCollection);
        _figuresBookcase.InitFigureSlots(_currentCollectionFigures);
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

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
