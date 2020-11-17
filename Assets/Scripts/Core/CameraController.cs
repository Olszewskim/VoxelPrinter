using DG.Tweening;
using UnityEngine;

public class CameraController : Singleton<CameraController> {
    private const float MOVE_ANIM_TIME = 2f;

    [SerializeField] private float _minCameraPosY;
    [SerializeField] private float _maxCameraPosY;
    [SerializeField] private Transform _gamePrinterView;
    [SerializeField] private Transform _collectionsBookcaseView;

    private int _minLayerID, _maxLayerID;

    public void InitCamera(int minLayerID, int maxLayerID) {
        _minLayerID = minLayerID;
        _maxLayerID = maxLayerID;
    }

    public void MoveCameraToLayer(int layer) {
        var percentage = Mathf.InverseLerp(_minLayerID, _maxLayerID, layer);
        var destinationHeight = Mathf.Lerp(_minCameraPosY, _maxCameraPosY, percentage);
        transform.DOMoveY(destinationHeight, MOVE_ANIM_TIME).SetEase(Ease.OutQuint);
    }

    public void MoveCameraToGamePrinterView() {
        transform.DOMove(_gamePrinterView.position, MOVE_ANIM_TIME).SetEase(Ease.InOutCubic);
    }

    public void MoveCameraToCollectionsBookcaseView() {
        transform.DOMove(_collectionsBookcaseView.position, MOVE_ANIM_TIME).SetEase(Ease.InOutCubic);
    }

    private void OnDestroy() {
        transform.DOKill();
    }
}
