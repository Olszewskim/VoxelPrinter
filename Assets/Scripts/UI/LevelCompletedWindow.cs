using TMPro;
using UnityEngine;

public class LevelCompletedWindow : WindowBehaviour<LevelCompletedWindow> {
    [SerializeField] private TextMeshProUGUI _percentText;

    public void ShowWindow(float stageFinishedAtPercentage) {
        _percentText.text = $"{(int)(stageFinishedAtPercentage * 100)}%";
        ShowWindow();
    }
}
