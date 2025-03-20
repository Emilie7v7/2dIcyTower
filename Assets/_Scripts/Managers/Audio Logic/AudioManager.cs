using System;
using System.Collections.Generic;
using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.Audio_Logic
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        public enum AudioTypes { Music, SFX, UI }

        private readonly Dictionary<AudioTypes, List<AudioSource>> _audioSources = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            DontDestroyOnLoad(gameObject);
        
            foreach (AudioTypes type in Enum.GetValues(typeof(AudioTypes)))
            {
                _audioSources[type] = new List<AudioSource>();
            }
        }

        private void Start()
        {
            RegisterAllAudioSources();
            ApplySavedAudioSettings();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Prevent memory leaks
        }

        public void RegisterAudioSource(AudioSource audioSource, AudioTypes type)
        {
            if (audioSource == null) return;

            foreach (var kvp in _audioSources)
            {
                if (kvp.Value.Contains(audioSource))
                {
                    return;
                }
            }
            if (!_audioSources[type].Contains(audioSource))
            {
                _audioSources[type].Add(audioSource);
                audioSource.volume = GetVolumeForType(type);
            }
        }

        public void SetVolume(float volume, AudioTypes type)
        {
            if (!_audioSources.TryGetValue(type, out var audioSource))
            {
                Debug.LogWarning($"AudioType {type} not found in AudioManager.");
                return;
            }
            
            foreach (var source in audioSource)
            {
                if (source != null)
                {
                    source.volume = volume;
                }
                else
                {
                    Debug.LogWarning("Found a null AudioSource in AudioManager.");
                }
            }
        }

        private void RegisterAllAudioSources()
        {
            foreach (var source in FindObjectsOfType<AudioSource>())
            {
                if (source.CompareTag("Music")) 
                {
                    Debug.Log($"✅ Registering {source.gameObject.name} as Music");
                    RegisterAudioSource(source, AudioTypes.Music);
                }
                else if (source.CompareTag("SFX"))
                {
                    Debug.Log($"✅ Registering {source.gameObject.name} as SFX");
                    RegisterAudioSource(source, AudioTypes.SFX);
                }
                else if (source.CompareTag("UI"))
                {
                    Debug.Log($"✅ Registering {source.gameObject.name} as UI");
                    RegisterAudioSource(source, AudioTypes.UI);
                }
                else
                {
                    Debug.LogWarning($"⚠ AudioSource {source.gameObject.name} has no valid tag. Skipping.");
                }
            }
        }
        
        public void UnregisterAudioSource(AudioSource audioSource, AudioTypes type)
        {
            if (audioSource == null || !_audioSources.ContainsKey(type)) return;

            if (_audioSources[type].Contains(audioSource))
            {
                _audioSources[type].Remove(audioSource);
            }
        }
        
        public void RegisterSceneAudio(AudioSource source, AudioTypes type)
        {
            if (source == null) return;
            if (!_audioSources.ContainsKey(type))
            {
                _audioSources[type] = new List<AudioSource>();
            }

            if (!_audioSources[type].Contains(source))
            {
                _audioSources[type].Add(source);
                source.volume = GetVolumeForType(type); // Apply correct volume immediately
            }
        }
        
        private static float GetVolumeForType(AudioTypes type)
        {
            return type switch
            {
                AudioTypes.Music => SettingsManager.GetMusicVolume(),
                AudioTypes.SFX => SettingsManager.GetSfxVolume(),
                AudioTypes.UI => SettingsManager.GetUiVolume(),
                _ => 1f
            };
        }
        
        private void ApplySavedAudioSettings()
        {
            Debug.Log($"Applying saved audio settings: Music={SettingsManager.GetMusicVolume()}, SFX={SettingsManager.GetSfxVolume()}, UI={SettingsManager.GetUiVolume()}");
            
            SetVolume(SettingsManager.GetMusicVolume(), AudioTypes.Music);
            SetVolume(SettingsManager.GetSfxVolume(), AudioTypes.SFX);
            SetVolume(SettingsManager.GetUiVolume(), AudioTypes.UI);
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplySavedAudioSettings(); // Apply volume settings again in the new scene
        }
    }
}