using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;

    private void Start() {
        SetMusicSettings(SettingsWindow.Instance.IsMusicOn);
        SetSFXSettings(SettingsWindow.Instance.IsSoundOn);
    }

    public void SetMusicSettings(bool isOn) {
        _musicAudioSource.mute = !isOn;
    }

    public void SetSFXSettings(bool isOn) {
        _sfxAudioSource.mute = !isOn;
    }
}
