using System.Collections.Generic;
using UnityEngine;

public enum SoundType {
    ShopDraw,
    UnlockItem
}

public class SoundsManager : Singleton<SoundsManager> {
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] Dictionary<SoundType, AudioClip> _soundsDatabase = new Dictionary<SoundType, AudioClip>();

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

    public static void PlaySound(SoundType soundType) {
        Instance._sfxAudioSource.PlayOneShot(Instance._soundsDatabase[soundType]);
    }

    public static AudioClip GetSound(SoundType soundType) {
        if (Instance._soundsDatabase.ContainsKey(soundType)) {
            return Instance._soundsDatabase[soundType];
        }

        return null;
    }
}
