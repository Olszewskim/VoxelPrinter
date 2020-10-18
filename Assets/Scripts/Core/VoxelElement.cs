using DG.Tweening;
using UnityEngine;

public class VoxelElement : MonoBehaviour {
    private static readonly Vector3 _hideScale = new Vector3(0, 0, 0);
    private static readonly Vector3 _beforePrintScale = new Vector3(1, 0, 1);
    private static readonly Vector3 _showScale = new Vector3(1, 1, 1);

    public void Hide() {
        transform.localScale = _hideScale;
    }

    public void Print(float time) {
        transform.localScale = _beforePrintScale;
        transform.DOScale(_showScale, time).SetEase(Ease.Linear);
    }
}
