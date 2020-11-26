using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrinterButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Action<Color> OnButtonPressed;
    public Action OnButtonReleased;

    [SerializeField] private Transform _buttonTop;

    private const float BUTTON_Y_ANIM_SCALE = 0.5f;
    private const float BUTTON_ANIM_TIME = 0.5f;


    private float _startScaleY;
    private MeshRenderer _meshRenderer;
    private bool _isClicked;
    private Color _myColor;

    private void Awake() {
        _meshRenderer = _buttonTop.GetComponent<MeshRenderer>();
        _startScaleY = transform.localScale.y;
        _buttonTop.DOScaleY(_startScaleY, 0);
    }

    public void SetButtonColor(Color color) {
        _meshRenderer.material.color = color;
        _myColor = color;
    }

    public void OnPointerDown(PointerEventData eventData) {
        _isClicked = true;
        _buttonTop.DOKill();
        _buttonTop.DOScaleY(BUTTON_Y_ANIM_SCALE, BUTTON_ANIM_TIME);
        OnButtonPressed?.Invoke(_myColor);
    }

    public void OnPointerUp(PointerEventData eventData) {
        _isClicked = false;
        _buttonTop.DOKill();
        _buttonTop.DOScaleY(_startScaleY, BUTTON_ANIM_TIME);
        OnButtonReleased?.Invoke();
    }
}
