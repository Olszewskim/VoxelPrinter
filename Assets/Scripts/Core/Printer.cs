using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Printer : MonoBehaviour {
    [SerializeField] private Transform _nozzle;
    [SerializeField] private Transform _fillament;
    [SerializeField] private PrinterButton _buttonsPrefab;
    [SerializeField] private ParticleSystem _laserBeam;

    private Material _fillamentMaterial;
    private Vector3 _fillamentStartScale;
    private Vector3 _fillamentRunOutScale = new Vector3(0.8f, 0, 0.8f);

    private List<PrinterButton> _buttons = new List<PrinterButton>();

    private const float PRINT_HEIGHT = 1.5f;
    private const float MOVE_TIME = 5f;

    private float _buttonAreaDistance = 8f;

    private void Awake() {
        _laserBeam.Stop();
        _fillamentStartScale = _fillament.localScale;
        _fillamentRunOutScale = Vector3.Scale(_fillamentRunOutScale, _fillamentStartScale);
        _fillamentMaterial = _fillament.GetComponent<MeshRenderer>().sharedMaterial;
        _buttonsPrefab.gameObject.SetActive(false);
    }

    public void SetButtonsColors(List<Color> colors) {
        TurnOffAllButtons();
        for (int i = 0; i < colors.Count; i++) {
            if (i >= _buttons.Count) {
                _buttons.Add(Instantiate(_buttonsPrefab, _buttonsPrefab.transform.parent));
            }

            _buttons[i].gameObject.SetActive(true);
            _buttons[i].SetButtonColor(colors[i]);
        }

        SetButtonsPositions(colors.Count);
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

    private void TurnOffAllButtons() {
        foreach (var button in _buttons) {
            button.gameObject.SetActive(false);
        }
    }

    private void SetButtonsPositions(int numberOfButtons) {
        var areaPerButton = _buttonAreaDistance / numberOfButtons;
        for (int i = 0; i < numberOfButtons; i++) {
            var pos = _buttons[i].transform.localPosition;
            pos.x = _buttonsPrefab.transform.localPosition.x + areaPerButton * i;
            _buttons[i].transform.localPosition = pos;
        }
    }
}
