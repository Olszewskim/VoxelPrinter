using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VoxelElement : MonoBehaviour {
    private const float FRAME_BLINK_TIME = 0.5f;

    private static readonly Vector3 _hideScale = new Vector3(0, 0, 0);
    private static readonly Vector3 _beforePrintScale = new Vector3(1, 0.01f, 1);
    private static readonly Vector3 _showScale = new Vector3(1, 1, 1);

    [SerializeField] private Image _frame;

    private Canvas _canvas;
    private MeshRenderer _meshRenderer;
    private Sequence _blinkSequence;

    private void Awake() {
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.worldCamera = Camera.main;
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Hide() {
        transform.localScale = _hideScale;
        _frame.gameObject.SetActive(false);
    }

    public void Print(float time, Material material, Action onFinish) {
        _meshRenderer.sharedMaterial = material;
        transform.DOScale(_showScale, time).SetEase(Ease.Linear)
            .OnComplete(() => {
                OnElementPrinted();
                onFinish?.Invoke();
            });
    }

    private void OnElementPrinted() {
        _frame.gameObject.SetActive(false);
        _blinkSequence.Kill();
    }

    public void PrepareToPrint() {
        transform.localScale = _beforePrintScale;
        _frame.gameObject.SetActive(true);
    }

    public Material GetMaterial() {
        return _meshRenderer.sharedMaterial;
    }

    public void ShowCurrentElement() {
        _frame.color = Color.white;
        _blinkSequence = DOTween.Sequence();
        _blinkSequence.Append(_frame.DOColor(Color.clear, FRAME_BLINK_TIME))
            .Append(_frame.DOColor(Color.white, FRAME_BLINK_TIME))
            .SetLoops(-1).SetEase(Ease.Linear);
    }
}
