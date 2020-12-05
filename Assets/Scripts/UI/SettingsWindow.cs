using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : PoppingOutWindowBehaviour<SettingsWindow> {
    [SerializeField] private SwitchUI _musicSettings;
    [SerializeField] private SwitchUI _soundsSettings;
    [SerializeField] private SwitchUI _hapticSettings;
    [SerializeField] private TextMeshProUGUI _versionText;

    public bool IsMusicOn { get; private set; }
    public bool IsSoundOn { get; private set; }
    public bool IsHapticOn { get; private set; }

    protected override void Awake() {
        base.Awake();
        InitSettings();
        SetVersionText();
    }

    private void InitSettings() {
        IsMusicOn = PlayerPrefs.GetInt(SaveKey.MUSIC_SETTINGS, 1) == 1;
        _musicSettings.InitSwitch(IsMusicOn);
        _musicSettings.OnSwitched += RefreshMusicSettings;

        IsSoundOn = PlayerPrefs.GetInt(SaveKey.SOUNDS_SETTINGS, 1) == 1;
        _soundsSettings.InitSwitch(IsSoundOn);
        _soundsSettings.OnSwitched += RefreshSoundsSettings;

        IsHapticOn = PlayerPrefs.GetInt(SaveKey.HAPTIC_SETTINGS, 1) == 1;
        _hapticSettings.InitSwitch(IsHapticOn);
        _hapticSettings.OnSwitched += RefreshHapticSettings;
    }

    private void RefreshMusicSettings(bool state) {
        IsMusicOn = state;
        PlayerPrefs.SetInt(SaveKey.MUSIC_SETTINGS, state ? 1 : 0);
        SoundManager.Instance.SetMusicSettings(IsMusicOn);
    }

    private void RefreshSoundsSettings(bool state) {
        IsSoundOn = state;
        PlayerPrefs.SetInt(SaveKey.SOUNDS_SETTINGS, state ? 1 : 0);
        SoundManager.Instance.SetSFXSettings(IsSoundOn);
    }

    private void RefreshHapticSettings(bool state) {
        IsHapticOn = state;
        PlayerPrefs.SetInt(SaveKey.HAPTIC_SETTINGS, state ? 1 : 0);
    }

    private void SetVersionText() {
        _versionText.text = $"Version {Application.version}";
    }
}
