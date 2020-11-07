using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : PoppingOutWindowBehaviour<SettingsWindow> {
    [SerializeField] private Button _closeButton;

    protected override void Awake() {
        base.Awake();
        _closeButton.onClick.AddListener(CloseWindow);
    }
}
