using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    [SerializeField] private Button _openSettingsButton;

    private void Awake() {
        _openSettingsButton.onClick.AddListener(OpenSetttingsWindow);
    }

    private void OpenSetttingsWindow() {
        SettingsWindow.Instance.ShowWindow();
    }
}
