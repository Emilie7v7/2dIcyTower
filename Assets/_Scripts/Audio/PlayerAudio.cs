using System;
using _Scripts.Managers.Audio_Logic;
using UnityEngine;

namespace _Scripts.Audio
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource coinPickupSound;
        [SerializeField] private AudioSource gotHitSound;

        private void OnEnable()
        {
            AudioManager.Instance?.RegisterAudioSource(coinPickupSound, AudioManager.AudioTypes.SFX);
            AudioManager.Instance?.RegisterAudioSource(gotHitSound, AudioManager.AudioTypes.SFX);
        }

        private void OnDisable()
        {
            AudioManager.Instance?.UnregisterAudioSource(coinPickupSound, AudioManager.AudioTypes.SFX);
            AudioManager.Instance?.UnregisterAudioSource(gotHitSound, AudioManager.AudioTypes.SFX);
        }

        public void PlayCoinPickupSound()
        {
            if (coinPickupSound)
                coinPickupSound.Play();
        }
        
        public void PlayHitSound()
        {
            if (gotHitSound)
                gotHitSound.Play();
        }
        
    }
}
