using System;
using DG.Tweening;
using UnityEngine;
using static Enums;

public class CameraController : MonoBehaviour {
    private const float MOVE_ANIM_TIME = 2f;

    [SerializeField] private float _minCameraPosY;
    [SerializeField] private float _maxCameraPosY;
    [SerializeField] private Transform _gamePrinterView;
    [SerializeField] private Transform _collectionsBookcaseView;

    private int _minLayerID, _maxLayerID;

    private void Awake() {
        GameManager.OnGameViewChanged += OnGameViewChanged;
    }

    private void OnGameViewChanged(GameViewType gameViewType) {
        switch (gameViewType) {
            case GameViewType.MainMenu:
                MoveCameraToGamePrinterView();
                break;
            case GameViewType.CollectionView:
                MoveCameraToCollectionsBookcaseView();
                break;
            case GameViewType.GameView:
                MoveCameraToGamePrinterView();
                break;
        }
    }

    public void InitCamera(int minLayerID, int maxLayerID) {
        _minLayerID = minLayerID;
        _maxLayerID = maxLayerID;
    }

    public void MoveCameraToLayer(int layer) {
        var percentage = Mathf.InverseLerp(_minLayerID, _maxLayerID, layer);
        var destinationHeight = Mathf.Lerp(_minCameraPosY, _maxCameraPosY, percentage);
        transform.DOMoveY(destinationHeight, MOVE_ANIM_TIME).SetEase(Ease.OutQuint);
    }

    private void MoveCameraToGamePrinterView() {
        transform.DOMove(_gamePrinterView.position, MOVE_ANIM_TIME).SetEase(Ease.InOutCubic);
        transform.DORotateQuaternion(_gamePrinterView.rotation, MOVE_ANIM_TIME).SetEase(Ease.InOutCubic);
    }

    private void MoveCameraToCollectionsBookcaseView() {
        transform.DOMove(_collectionsBookcaseView.position, MOVE_ANIM_TIME).SetEase(Ease.InOutCubic);
        transform.DORotateQuaternion(_collectionsBookcaseView.rotation, MOVE_ANIM_TIME).SetEase(Ease.InOutCubic);
    }

    private void OnDestroy() {
        transform.DOKill();
        GameManager.OnGameViewChanged -= OnGameViewChanged;
    }
}
