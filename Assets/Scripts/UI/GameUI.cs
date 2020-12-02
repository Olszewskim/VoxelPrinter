using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class GameUI : MonoBehaviour {
    [SerializeField] private Button _openSettingsButton;
    [SerializeField] private Button _onBackButton;

    private void Awake() {
        _openSettingsButton.onClick.AddListener(OpenSetttingsWindow);
        _onBackButton.onClick.AddListener(OnBack);
        GameManager.OnGameViewChanged += RefreshButtonsVisibility;
    }

    private void RefreshButtonsVisibility(GameViewType gameViewType) {
        var shouldOnBackButtonBeVisible =
            gameViewType == GameViewType.CollectionView || gameViewType == GameViewType.GameView;
        _onBackButton.gameObject.SetActive(shouldOnBackButtonBeVisible);
    }

    private void OpenSetttingsWindow() {
        SettingsWindow.Instance.ShowWindow();
    }

    private void OnBack() {
        if (GameManager.Instance.IsInCollectionsView()) {
            GameManager.Instance.GoBackToMainMenu();
        } else if (GameManager.Instance.IsInGameView()) {
            GameManager.Instance.GoBackToCollectionView();
        }
    }

    private void OnDestroy() {
        GameManager.OnGameViewChanged -= RefreshButtonsVisibility;
    }
}
