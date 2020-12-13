using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class GameUI : MonoBehaviour {
    [SerializeField] private Button _openSettingsButton;
    [SerializeField] private Button _openShopButton;
    [SerializeField] private Button _onBackButton;

    private void Awake() {
        _openSettingsButton.onClick.AddListener(OpenSetttingsWindow);
        _onBackButton.onClick.AddListener(OnBack);
        _openShopButton.onClick.AddListener(OpenShopWindow);
        GameManager.OnGameViewChanged += RefreshButtonsVisibility;
    }


    private void RefreshButtonsVisibility(GameViewType gameViewType) {
        var shouldOnBackButtonBeVisible =
            gameViewType == GameViewType.SelectFigureView || gameViewType == GameViewType.GameView;
        _onBackButton.gameObject.SetActive(shouldOnBackButtonBeVisible);
    }

    private void OpenSetttingsWindow() {
        SettingsWindow.Instance.ShowWindow();
    }

    private void OpenShopWindow() {
        ShopWindow.Instance.ShowWindow();
    }


    private void OnBack() {
        GameManager.Instance.OnBack();
    }

    private void OnDestroy() {
        GameManager.OnGameViewChanged -= RefreshButtonsVisibility;
    }
}
