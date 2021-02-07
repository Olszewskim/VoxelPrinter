

using UnityEngine;

namespace PsychoVR.ExtensionsMethods
{
    public static class SoundExtensions {

        public static void PlayAudioUniquely(this AudioSource audioSource, AudioClip clip) {
            if (!audioSource.isPlaying) {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
