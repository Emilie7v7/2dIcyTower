using System;
using _Scripts.Managers.Audio_Logic;
using UnityEngine;

namespace _Scripts.Audio
{
    public class ExplosionAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource playerExplosionSound;
        [SerializeField] private AudioSource skeletonExplosionSound;

        private void OnEnable()
        {
            AudioManager.Instance?.RegisterAudioSource(playerExplosionSound, AudioManager.AudioTypes.SFX);
            AudioManager.Instance?.RegisterAudioSource(skeletonExplosionSound, AudioManager.AudioTypes.SFX);
        }

        private void OnDisable()
        {
            AudioManager.Instance?.UnregisterAudioSource(playerExplosionSound, AudioManager.AudioTypes.SFX);
            AudioManager.Instance?.UnregisterAudioSource(skeletonExplosionSound, AudioManager.AudioTypes.SFX);
        }
        
        public void PlayPlayerExplosion()
        {
            if(playerExplosionSound)
                playerExplosionSound.Play();
        }

        public void PlaySkeletonExplosion()
        {
            if(skeletonExplosionSound)
                skeletonExplosionSound.Play();
        }
        
        
    }
}
