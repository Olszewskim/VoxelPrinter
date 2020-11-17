using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : WindowBehaviour<MainMenuWindow> {
    [SerializeField] private Button _startGameButton;

    protected override void Awake() {
        base.Awake();
        _startGameButton.onClick.AddListener(StartGame);
    }

    private void StartGame() {
        GameManager.Instance.ShowCollectionsView();
        CloseWindow();
    }
}
