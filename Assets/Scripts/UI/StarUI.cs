using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour {
    private const float SHOW_STAR_ANIM_TIME = 1f;
    private const int STAR_SPINS = 3;

    [SerializeField] private Image _starFullImage;

    public void Show(float delay, bool withAnimation) {
        _starFullImage.gameObject.SetActive(true);
        if (withAnimation) {
            _starFullImage.DOFade(0, 0);
            var animation = DOTween.Sequence();
            animation.AppendInterval(delay)
                .Append(_starFullImage.DOFade(1, SHOW_STAR_ANIM_TIME))
                .Join(_starFullImage.transform.DOScale(1, SHOW_STAR_ANIM_TIME))
                .Join(_starFullImage.transform.DORotate(new Vector3(0, 0, -360 * STAR_SPINS), SHOW_STAR_ANIM_TIME,
                    RotateMode.FastBeyond360))
                .SetEase(Ease.OutCubic);
        } else {
            _starFullImage.DOFade(1, 0);
        }
    }

    public void Hide() {
        _starFullImage.transform.DOScale(0, 0);
        _starFullImage.gameObject.SetActive(false);
    }
}
