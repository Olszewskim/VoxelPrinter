using UnityEngine;

public static class CanvasGroupExtensions {

    public static void LockGroup(this CanvasGroup canvasGroup) {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public static void SetAlpha(this CanvasGroup canvasGroup, float alphaValue) {
        canvasGroup.alpha = alphaValue;
    }

    public static void UnlockGroup(this CanvasGroup canvasGroup) {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void ChangeGroupState(this CanvasGroup canvasGroup, bool state) {
        SetAlpha(canvasGroup, state ? 1f : 0f);
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }
}
