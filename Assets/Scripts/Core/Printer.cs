using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Printer : MonoBehaviour {
    private const float PRINT_HEIGHT = 1.5f;
    private const float MOVE_TIME = 5f;
    private const float PRINT_TIME = 0.3f;

    [SerializeField] private Transform _nozzle;
    [SerializeField] private Transform _fillament;
    [SerializeField] private PrinterButton _buttonsPrefab;
    [SerializeField] private ParticleSystem _laserBeam;

    private Material _fillamentMaterial;
    private Vector3 _fillamentStartScale;
    private Vector3 _fillamentRunOutScale = new Vector3(0.8f, 0, 0.8f);

    private List<PrinterButton> _buttons = new List<PrinterButton>();
    private VoxelFigure _currentPrintedModel;
    private float _buttonAreaDistance = 8f;

    private bool _isPrinting;
    private bool _continuePrinting;
    private bool _isNoozleMoved;
    private Color _currentColor;

    private void Awake() {
        _laserBeam.Stop();
        _fillamentStartScale = _fillament.localScale;
        _fillamentRunOutScale = Vector3.Scale(_fillamentRunOutScale, _fillamentStartScale);
        _fillamentMaterial = _fillament.GetComponent<MeshRenderer>().sharedMaterial;
        _buttonsPrefab.gameObject.SetActive(false);
    }

    public void SetupPrintModel(VoxelFigure voxelFigure) {
        _currentPrintedModel = voxelFigure;
        SetButtonsColors(voxelFigure.GetFigureColors());
        voxelFigure.TurnOffAllVoxels();
        _currentPrintedModel.ShowCurrentElementAndLayer();
    }

    private void SetButtonsColors(List<Color> colors) {
        TurnOffAllButtons();
        for (int i = 0; i < colors.Count; i++) {
            if (i >= _buttons.Count) {
                var button = Instantiate(_buttonsPrefab, _buttonsPrefab.transform.parent);
                button.OnButtonPressed += OnPrintButtonPressed;
                button.OnButtonReleased += OnPrintButtonReleased;
                _buttons.Add(button);
            }

            _buttons[i].gameObject.SetActive(true);
            _buttons[i].SetButtonColor(colors[i]);
        }

        SetButtonsPositions(colors.Count);
    }

    private void StartPrinting(Color printColor) {
        var currentElement = _currentPrintedModel.GetCurrentElement();
        if (currentElement == null) {
            return;
        }

        SetFillamentColor();
        if (_isNoozleMoved) {
            ContinuePrinting(printColor);
            return;
        }

        _isPrinting = true;
        _isNoozleMoved = false;
        MoveNoozle(printColor, currentElement);
    }

    private void MoveNoozle(Color printColor, VoxelData voxelData) {
        _fillament.localScale = _fillamentStartScale;
        var position = voxelData.voxelPosition;
        position.y += PRINT_HEIGHT;
        _nozzle.DOMove(position, MOVE_TIME).SetEase(Ease.Linear).SetSpeedBased()
            .OnComplete(FinishNoozleMove);
    }

    private void FinishNoozleMove() {
        _isNoozleMoved = true;
        if (_continuePrinting) {
            SetFillamentColor();
            ContinuePrinting(_currentColor);
        } else {
            _isPrinting = false;
        }
    }

    private void ContinuePrinting(Color printColor) {
        _laserBeam.Play();
        _fillament.DOScale(_fillamentRunOutScale, PRINT_TIME).OnComplete(() => _laserBeam.Stop());
        _currentPrintedModel.Print(PRINT_TIME, printColor, FinishPrinting);
    }

    private void FinishPrinting() {
        _currentPrintedModel.ShowCurrentElementAndLayer();
        _isPrinting = false;
        _isNoozleMoved = false;
        if (_continuePrinting) {
            StartPrinting(_currentColor);
        }
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

    private void OnPrintButtonPressed(Color printColor) {
        _currentColor = printColor;
        _continuePrinting = true;
        if (!_isPrinting) {
            StartPrinting(printColor);
        }
    }

    private void SetFillamentColor() {
        _fillamentMaterial.color = _currentColor;
        var main = _laserBeam.main;
        main.startColor = _currentColor;
    }

    private void OnPrintButtonReleased() {
        _continuePrinting = false;
    }
}
