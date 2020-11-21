using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class MainMenuWindow : WindowBehaviour<MainMenuWindow> {
    [SerializeField] private Button _startGameButton;

    protected override void Awake() {
        base.Awake();
        _startGameButton.onClick.AddListener(StartGame);
        GameManager.OnGameViewChanged += OnGameViewChanged;
    }

    private void OnGameViewChanged(GameViewType gameViewType) {
        if (gameViewType == GameViewType.MainMenu) {
            ShowWindow();
        }
    }

    private void StartGame() {
        GameManager.Instance.ShowCollectionsView();
        CloseWindow();
    }

    protected override void OnDestroy() {
        GameManager.OnGameViewChanged -= OnGameViewChanged;
        base.OnDestroy();
    }
}
