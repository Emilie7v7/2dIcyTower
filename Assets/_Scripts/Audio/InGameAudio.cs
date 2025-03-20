using System;
using _Scripts.Managers.Audio_Logic;
using UnityEngine;

namespace _Scripts.Audio
{
    public class InGameAudio : MonoBehaviour
    {
        public static InGameAudio Instance { get; private set; }
        
        [SerializeField] private AudioSource gameAudio;
        
        private void Awake()
        {
            if(Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
            
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            AudioManager.Instance?.RegisterAudioSource(gameAudio, AudioManager.AudioTypes.Music);
        }

        private void OnDisable()
        {
            AudioManager.Instance?.UnregisterAudioSource(gameAudio, AudioManager.AudioTypes.Music);
        }

        public void PlayGameAudio()
        {
            if(gameAudio)
                gameAudio.Play();
        }

        public void StopGameAudio()
        {
            if(gameAudio && gameAudio.isPlaying)
                gameAudio.Stop();
        }
    }
}
