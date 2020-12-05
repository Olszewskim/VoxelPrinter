using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LevelCompletedWindow : WindowBehaviour<LevelCompletedWindow> {
    private const float PERCENTAGE_ANIM_TIME = 2f;

    [SerializeField] private TextMeshProUGUI _percentText;
    [SerializeField] private TextMeshProUGUI _completedText;
    [SerializeField] private StarsControllerUI _starsControllerUI;
    [SerializeField] private Button _nextFigureButton;

    protected override void Start() {
        base.Start();
        _nextFigureButton.onClick.AddListener(PrintNextFigure);
    }

    public void ShowWindow(float stageFinishedAtPercentage) {
        _starsControllerUI.HideStars();
        var finalPercent = stageFinishedAtPercentage * 100;
        _starsControllerUI.ShowStars(stageFinishedAtPercentage, true);
        _percentText.text = "0%";
        _completedText.text = $"{GameManager.Instance.GetCurrentVoxelFigureName()} Completed";
        DOVirtual.Float(0, finalPercent, PERCENTAGE_ANIM_TIME, AnimatePercentage).SetEase(Ease.OutCubic);
        ShowWindow();
    }

    private void AnimatePercentage(float value) {
        _percentText.text = $"{(int) value}%";
    }

    private void PrintNextFigure() {
        CloseWindow();
        GameManager.Instance.ShowSelectFigureView();
    }
}
