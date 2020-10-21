using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrinterButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private Transform _buttonTop;

    private const float BUTTON_Y_ANIM_SCALE = 0.5f;
    private const float BUTTON_ANIM_TIME = 0.5f;

    private MeshRenderer _meshRenderer;
    private bool _isClicked;

    private void Awake() {
        _meshRenderer = _buttonTop.GetComponent<MeshRenderer>();
        _buttonTop.DOScaleY(1, 0);
    }

    public void SetButtonColor(Color color) {
        _meshRenderer.material.color = color;
    }

    public void OnPointerDown(PointerEventData eventData) {
        _isClicked = true;
        _buttonTop.DOKill();
        _buttonTop.DOScaleY(BUTTON_Y_ANIM_SCALE, BUTTON_ANIM_TIME);
    }

    public void OnPointerUp(PointerEventData eventData) {
        _isClicked = false;
        _buttonTop.DOKill();
        _buttonTop.DOScaleY(1, BUTTON_ANIM_TIME);
    }
}
