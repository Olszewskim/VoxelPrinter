using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Printer : MonoBehaviour {
    public event Action OnPrintingStarted;
    public event Action<float> OnPrintingProgress;

    private const float PRINT_HEIGHT = 1.5f;
    private const float MOVE_TIME = 7f;
    private const float PRINT_TIME = 0.3f;
    private const float MAX_SPEED_MULTIPLIER = 6f;
    private const float SPEED_MULTIPLIER_INCREASE_RATE = 1.1f;

    [SerializeField] private Transform _nozzle;
    [SerializeField] private Transform _fillament;
    [SerializeField] private PrinterButton _buttonsPrefab;
    [SerializeField] private ParticleSystem _laserBeam;
    [SerializeField] private CameraController _cameraController;

    private Material _fillamentMaterial;
    private Vector3 _fillamentStartScale;
    private Vector3 _fillamentRunOutScale = new Vector3(0.8f, 0, 0.8f);

    private List<PrinterButton> _buttons = new List<PrinterButton>();
    private VoxelFigure _currentPrintedModel;
    private float _buttonAreaDistance = 16f;

    private bool _isPrinting;
    private bool _continuePrinting;
    private bool _isNoozleMoved;
    private Color _currentColor;
    private float _currentSpeedMultiplier = 1f;

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

        var layers = _currentPrintedModel.GetLayersIDs();
        _cameraController.InitCamera(layers.min, layers.max);
        _currentPrintedModel.OnLayerChanged += _cameraController.MoveCameraToLayer;

        _currentPrintedModel.ShowCurrentElementAndLayer();
        OnPrintingStarted?.Invoke();
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
        var currentElement = _currentPrintedModel?.CurrentElement;
        if (currentElement == null) {
            return;
        }

        _isPrinting = true;
        SetFillamentColor();

        if (_isNoozleMoved) {
            ContinuePrinting(printColor);
            return;
        }

        _isNoozleMoved = false;
        MoveNoozle(currentElement.voxelPosition);
    }

    private void MoveNoozle(Vector3 voxelPosition) {
        _fillament.localScale = _fillamentStartScale;
        var position = voxelPosition;
        position.y += PRINT_HEIGHT;
        _nozzle.DOMove(position, GetNoozleMovementSpeed()).SetEase(Ease.Linear).SetSpeedBased()
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
        var printTime = GetPrintSpeed();
        _fillament.DOScale(_fillamentRunOutScale, printTime).OnComplete(() => _laserBeam.Stop());
        _currentPrintedModel.Print(printTime, printColor, FinishPrinting);
        Vibration.VibratePop();
    }

    private void FinishPrinting() {
        _currentPrintedModel.ShowCurrentElementAndLayer();
        _isPrinting = false;
        _isNoozleMoved = false;
        OnPrintingProgress?.Invoke(_currentPrintedModel.GetPrintProgress());

        if (_currentPrintedModel.IsCompleted) {
            FinishPrintingCurrentModel();
            return;
        }

        if (_continuePrinting) {
            IncreaseSpeedMultiplier();
            StartPrinting(_currentColor);
        }
    }

    private void FinishPrintingCurrentModel() {
        GameManager.Instance.SaveFigureData(_currentPrintedModel,out float stageFinishedAtPercentage);
        _currentPrintedModel = null;
        LevelCompletedWindow.Instance.ShowWindow(stageFinishedAtPercentage);
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
        _currentSpeedMultiplier = 1f;
    }

    private void IncreaseSpeedMultiplier() {
        _currentSpeedMultiplier *= SPEED_MULTIPLIER_INCREASE_RATE;
        _currentSpeedMultiplier = Mathf.Min(_currentSpeedMultiplier, MAX_SPEED_MULTIPLIER);
    }

    private float GetPrintSpeed() {
        return PRINT_TIME / _currentSpeedMultiplier;
    }

    private float GetNoozleMovementSpeed() {
        return MOVE_TIME * _currentSpeedMultiplier;
    }
}
