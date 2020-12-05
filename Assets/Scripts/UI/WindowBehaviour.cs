using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class WindowBehaviour<T> : Screen<T> where T : Screen<T> {
    public event Action OnWindowOpened;

    public event Action OnWindowClosed;

    [SerializeField] protected Button _closeButton;
    [SerializeField] protected Button _closeWindowClickableArea;

    public bool IsOpen { get; protected set; }
    protected float animTime = 0.5f;
    protected CanvasGroup canvasGroup;

    protected override void Awake() {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.LockGroup();
        _closeWindowClickableArea?.onClick.AddListener(ForceCloseWindow);
        if (_closeButton != null) {
            _closeButton.onClick.AddListener(CloseWindow);
        }
    }

    protected virtual void Start() {
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        CloseWindow();
    }

    public virtual void ShowWindow() {
        if (IsOpen) {
            return;
        }

        IsOpen = true;
        TurnOnScreen();
        canvasGroup?.UnlockGroup();
        canvasGroup?.DOKill();
        canvasGroup?.DOFade(1, animTime).SetDelay(Time.deltaTime).SetUpdate(true);
        OnWindowOpened?.Invoke();
    }

    public virtual void CloseWindow() {
        CloseWindow(null);
    }

    protected virtual void ForceCloseWindow() {
        CloseWindow();
    }

    public void CloseWindow(Action closeWindowAction) {
        if (!IsOpen) {
            return;
        }

        canvasGroup?.DOKill();
        IsOpen = false;
        canvasGroup?.LockGroup();
        Action afterAnimationAction = TurnOffScreen;
        if (closeWindowAction != null) {
            afterAnimationAction += closeWindowAction;
        }

        canvasGroup?.DOFade(0, animTime).OnComplete(afterAnimationAction.Invoke);
        OnWindowClosed?.Invoke();
    }

    public bool IsVisible() {
        return IsOpen && canvasGroup.alpha == 1;
    }

    protected override void OnDestroy() {
        canvasGroup?.DOKill();
        base.OnDestroy();
    }
}
