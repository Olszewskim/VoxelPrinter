using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FigureSlot : MonoBehaviour {
    private const float POP_ANIM_TIME = 0.5f;
    private const float POP_STRENGTH = 0.3f;

    [SerializeField] private StarsControllerUI _starsControllerUI;
    [SerializeField] private Button _printButton;
    [SerializeField] private Button _lockPadImage;
    [SerializeField] private TextMeshProUGUI _voxelFigureNameText;

    private VoxelFigureData _currentVoxelFigureData;
    private VoxelFigureInfoData _currentVoxelFigureInfoData;
    private VoxelFigure _currentSpawnedVoxelFigure;

    private void Awake() {
        _printButton.onClick.AddListener(PrintFigure);
        _lockPadImage.onClick.AddListener(ShakeLockPad);
    }

    public void Init(VoxelFigureData voxelFigureData, VoxelFigureInfoData voxelFigureInfoData) {
        gameObject.SetActive(true);
        _currentVoxelFigureData = voxelFigureData;
        _currentVoxelFigureInfoData = voxelFigureInfoData;
        _printButton.gameObject.SetActive(_currentVoxelFigureInfoData.isUnlocked);
        _lockPadImage.gameObject.SetActive(!voxelFigureInfoData.isUnlocked);
        LoadFigure();
        InitStars();
        SetFigureName();
    }

    private void LoadFigure() {
        _currentSpawnedVoxelFigure = GameResourcesDatabase.GetBookcaseFigure(_currentVoxelFigureData);
        _currentSpawnedVoxelFigure.gameObject.SetActive(true);
        _currentSpawnedVoxelFigure.transform.position = transform.position;
        _currentSpawnedVoxelFigure.transform.localScale = transform.lossyScale;
        _currentSpawnedVoxelFigure.SetFigureState(_currentVoxelFigureInfoData);
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
        var shouldShowVoxelFigureName = _currentVoxelFigureInfoData.isCompleted;
        _voxelFigureNameText.gameObject.SetActive(shouldShowVoxelFigureName);
        if (shouldShowVoxelFigureName) {
            _voxelFigureNameText.text = _currentVoxelFigureData.figureName;
        }
    }

    public void Clear() {
        if (_currentSpawnedVoxelFigure != null) {
            _currentSpawnedVoxelFigure.gameObject.SetActive(false);
            _currentSpawnedVoxelFigure = null;
            _currentVoxelFigureInfoData = null;
        }

        gameObject.SetActive(false);
    }

    private void PrintFigure() {
        GameManager.Instance.PrintNewFigure(_currentVoxelFigureData);
    }

    private void ShakeLockPad() {
        _lockPadImage.transform.DOShakeScale(POP_ANIM_TIME, POP_STRENGTH);
    }
}
