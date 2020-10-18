using DG.Tweening;
using UnityEngine;

public class Printer : MonoBehaviour {
    [SerializeField] private Transform _nozzle;
    [SerializeField] private Transform _fillament;

    private Material _fillamentMaterial;
    private Vector3 _fillamentStartScale;
    private Vector3 _fillamentRunOutScale = new Vector3(0.8f, 0, 0.8f);

    private const float PRINT_HEIGHT = 1.5f;
    private const float MOVE_TIME = 5f;

    private void Awake() {
        _fillamentStartScale = _fillament.localScale;
        _fillamentRunOutScale = Vector3.Scale(_fillamentRunOutScale, _fillamentStartScale);
        _fillamentMaterial = _fillament.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public Tween MoveNoozle(Vector3 position, Color printColor) {
        _fillamentMaterial.color = printColor;
        _fillament.localScale = _fillamentStartScale;
        position.y += PRINT_HEIGHT;
        var anim = _nozzle.DOMove(position, MOVE_TIME).SetEase(Ease.Linear).SetSpeedBased();
        return anim;
    }

    public void Print(float printTime) {
        _fillament.DOScale(_fillamentRunOutScale, printTime);
    }
}
