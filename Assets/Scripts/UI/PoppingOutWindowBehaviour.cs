using DG.Tweening;
using UnityEngine;

public abstract class PoppingOutWindowBehaviour<T> : WindowBehaviour<T> where T : PoppingOutWindowBehaviour<T> {
    [SerializeField] private Transform _poppingOutElement;

    private const float POP_ANIM_TIME = 0.35f;
    private const float POP_STRENGTH = 0.1f;

    public override void ShowWindow() {
        if (IsOpen) {
            return;
        }

        PopOutAnim();
        base.ShowWindow();
    }

    private void PopOutAnim() {
        _poppingOutElement?.DOShakeScale(POP_ANIM_TIME, POP_STRENGTH);
    }
}
