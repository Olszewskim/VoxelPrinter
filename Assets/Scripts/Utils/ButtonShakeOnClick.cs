using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonShakeOnClick : MonoBehaviour {
    private const float SHAKE_ANIM_TIME = 0.5f;
    private const float SHAKE_STRENGTH = 0.15f;

    private Button _button;

    private void Awake() {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ShakeButton);
    }

    private void ShakeButton() {
        _button.transform.DOShakeScale(SHAKE_ANIM_TIME, SHAKE_STRENGTH);
    }
}
