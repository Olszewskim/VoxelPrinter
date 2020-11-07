using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SwitchUI : MonoBehaviour {
    public event Action<bool> OnSwitched;

    [SerializeField] private Image _onGraphic;
    [SerializeField] private Image _offGraphic;

    private bool _isOn = true;
    private Button _button;

    protected void Awake() {
        InitSwitch(_isOn);
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        _isOn = !_isOn;
        SetupSwitch();
        OnSwitched?.Invoke(_isOn);
    }

    public void InitSwitch(bool isOn) {
        _isOn = isOn;
        SetupSwitch();
    }

    private void SetupSwitch() {
        _onGraphic?.gameObject.SetActive(_isOn);
        _offGraphic?.gameObject.SetActive(!_isOn);
    }

    public void Untoggle() {
        InitSwitch(false);
    }
}
