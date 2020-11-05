using TMPro;
using UnityEngine;
using DG.Tweening;

public class LevelCompletedWindow : WindowBehaviour<LevelCompletedWindow> {
    private const float PERCENTAGE_ANIM_TIME = 2f;

    [SerializeField] private TextMeshProUGUI _percentText;
    [SerializeField] private StarsControllerUI _starsControllerUI;

    public void ShowWindow(float stageFinishedAtPercentage) {
        _starsControllerUI.HideStars();
        var finalPercent = stageFinishedAtPercentage * 100;
        _starsControllerUI.ShowStars(finalPercent);
        _percentText.text = "0%";
        DOVirtual.Float(0, finalPercent, PERCENTAGE_ANIM_TIME, AnimatePercentage).SetEase(Ease.OutCubic);
        ShowWindow();
    }

    private void AnimatePercentage(float value) {
        _percentText.text = $"{(int) value}%";
    }
}
