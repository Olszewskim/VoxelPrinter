using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VoxelElement : MonoBehaviour {
    private static readonly Vector3 _hideScale = new Vector3(0, 0, 0);
    private static readonly Vector3 _beforePrintScale = new Vector3(1, 0.01f, 1);
    private static readonly Vector3 _showScale = new Vector3(1, 1, 1);

    [SerializeField] private Image _frame;

    private Canvas _canvas;

    private void Awake() {
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.worldCamera = Camera.main;
    }

    public void Hide() {
        transform.localScale = _hideScale;
        _frame.gameObject.SetActive(false);
    }

    public void Print(float time) {
        _frame.color = Color.white;
        _frame.DOColor(Color.clear, .4f).SetLoops(-1);
        transform.DOScale(_showScale, time).SetEase(Ease.Linear)
            .OnComplete(OnElementPrinted);
    }

    private void OnElementPrinted() {
        _frame.gameObject.SetActive(false);
    }

    public void PrepareToPrint() {
        transform.localScale = _beforePrintScale;
        _frame.gameObject.SetActive(true);
    }
}
