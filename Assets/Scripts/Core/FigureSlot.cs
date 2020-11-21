using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FigureSlot : MonoBehaviour {
    [SerializeField] private StarsControllerUI _starsControllerUI;
    [SerializeField] private Button _printButton;
    [SerializeField] private Image _lockPadImage;
    [SerializeField] private TextMeshProUGUI _voxelFigureNameText;

    private VoxelFigureData _currentVoxelFigureData;
    private VoxelFigureInfoData _currentVoxelFigureInfoData;
    private VoxelFigure _currentSpawnedVoxelFigure;

    private void Awake() {
        _printButton.onClick.AddListener(PrintFigure);
            //TODO: Shake lockpad on click
    }



    public void Init(VoxelFigureData voxelFigureData, VoxelFigureInfoData voxelFigureInfoData) {
        _currentVoxelFigureData = voxelFigureData;
        _currentSpawnedVoxelFigure = Instantiate(_currentVoxelFigureData.voxelFigure, transform);
        _currentVoxelFigureInfoData = voxelFigureInfoData;
        _currentSpawnedVoxelFigure.SetFigureState(voxelFigureInfoData);
        _printButton.gameObject.SetActive(_currentVoxelFigureInfoData.isUnlocked);
        _lockPadImage.gameObject.SetActive(!voxelFigureInfoData.isUnlocked);
        InitStars();
        SetFigureName();
    }



    private void InitStars() {
        var shouldShowStars = _currentVoxelFigureInfoData.isCompleted;
        _starsControllerUI.gameObject.SetActive(shouldShowStars);
        if (shouldShowStars) {
            _starsControllerUI.HideStars();
            _starsControllerUI.ShowStars(_currentVoxelFigureInfoData.completionPercent, false);
        }
    }


    private void SetFigureName() {
        var shouldShowVoxelFigureName = _currentSpawnedVoxelFigure.IsCompleted;
        _voxelFigureNameText.gameObject.SetActive(shouldShowVoxelFigureName);
        if (shouldShowVoxelFigureName) {
            _voxelFigureNameText.text = _currentVoxelFigureData.figureName;
        }
    }
    public void Clear() {
        if (_currentSpawnedVoxelFigure != null) {
            //TODO: Optimize spawning figures
            Destroy(_currentSpawnedVoxelFigure.gameObject);
            _currentSpawnedVoxelFigure = null;
            _currentVoxelFigureInfoData = null;
        }
    }

    private void PrintFigure() {
        GameManager.Instance.PrintNewFigure(_currentVoxelFigureData);
    }
}
