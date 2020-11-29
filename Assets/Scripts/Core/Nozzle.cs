using System;
using DG.Tweening;
using UnityEngine;

public class Nozzle : MonoBehaviour {
    private const float PRINT_HEIGHT = 1.5f;

    [SerializeField] private Transform _fillament;
    [SerializeField] private ParticleSystem _laserBeam;
    [SerializeField] private Vector3 _fillamentRunOutScale = new Vector3(0.8f, 0, 0.8f);

    private Material _fillamentMaterial;
    private Vector3 _fillamentStartScale;

    private void Awake() {
        _laserBeam.Stop();
        _fillamentStartScale = _fillament.localScale;
        _fillamentRunOutScale = Vector3.Scale(_fillamentRunOutScale, _fillamentStartScale);
        _fillamentMaterial = _fillament.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void MoveNoozle(Vector3 voxelPosition, float nozzleMoveSpeed, Action onFinishMoveAction) {
        _fillament.localScale = _fillamentStartScale;
        var position = voxelPosition;
        position.y += PRINT_HEIGHT;
        transform.DOMove(position, nozzleMoveSpeed).SetEase(Ease.Linear).SetSpeedBased().OnComplete(onFinishMoveAction.Invoke);
    }

    public void Print(float printTime) {
        _laserBeam.Play();
        _fillament.DOScale(_fillamentRunOutScale, printTime).OnComplete(() => _laserBeam.Stop());
    }

    public void SetFillamentColor(Color color) {
        _fillamentMaterial.color = color;
        var main = _laserBeam.main;
        main.startColor = color;
    }
}
