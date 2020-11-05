using System;
using UnityEngine;

public abstract class Screen<T> : Singleton<T> where T : Singleton<T> {

    public event Action OnScreenOpened;

    public event Action OnScreenClosed;

    public virtual void TurnOnScreen() {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            OnScreenOpened?.Invoke();
        }
    }

    public virtual void TurnOffScreen() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
            OnScreenClosed?.Invoke();
        }
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            OnBack();
        }
    }

    protected virtual void OnBack() {
    }
}
