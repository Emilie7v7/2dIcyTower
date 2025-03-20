using System;
using _Scripts.Managers.Audio_Logic;
using UnityEngine;

namespace _Scripts.Audio
{
    public class SkeletonAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource deathSound; //Work in progress
        [SerializeField] private AudioSource walkingSound;

        private void OnEnable()
        {
            AudioManager.Instance?.RegisterAudioSource(walkingSound, AudioManager.AudioTypes.SFX);
        }

        private void OnDisable()
        {
            AudioManager.Instance?.UnregisterAudioSource(walkingSound, AudioManager.AudioTypes.SFX);
        }

        public void PlayWalkingSound()
        {
            if(walkingSound)
                walkingSound.Play();
        }
        public void StopWalkingSound()
        {
            if (walkingSound && walkingSound.isPlaying)
                walkingSound.Stop();
        }
    }
}
