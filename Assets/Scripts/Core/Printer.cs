using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Printer : MonoBehaviour {
    [SerializeField] private Transform _nozzle;
    [SerializeField] private Transform _fillament;
    [SerializeField] private PrinterButton[] _buttons;
    [SerializeField] private ParticleSystem _laserBeam;

    private Material _fillamentMaterial;
    private Vector3 _fillamentStartScale;
    private Vector3 _fillamentRunOutScale = new Vector3(0.8f, 0, 0.8f);

    private const float PRINT_HEIGHT = 1.5f;
    private const float MOVE_TIME = 5f;

    private void Awake() {
        _laserBeam.Stop();
        _fillamentStartScale = _fillament.localScale;
        _fillamentRunOutScale = Vector3.Scale(_fillamentRunOutScale, _fillamentStartScale);
        _fillamentMaterial = _fillament.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void SetButtonsColors(List<Color> colors) {
        for (int i = 0; i < _buttons.Length; i++) {
            _buttons[i].SetButtonColor(colors[i]);
        }
    }

    public Tween MoveNoozle(Vector3 position, Color printColor) {
        _fillamentMaterial.color = printColor;
        _fillament.localScale = _fillamentStartScale;
        var main = _laserBeam.main;
        main.startColor = printColor;
        position.y += PRINT_HEIGHT;
        var anim = _nozzle.DOMove(position, MOVE_TIME).SetEase(Ease.Linear).SetSpeedBased();
        return anim;
    }

    public void Print(float printTime) {
        _laserBeam.Play();
        _fillament.DOScale(_fillamentRunOutScale, printTime).OnComplete(() => _laserBeam.Stop());
    }
}
