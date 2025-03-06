using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // List of audio clips for background music
    [SerializeField] private List<AudioClip> musicTracks;

    // Audio source to play music
    private AudioSource audioSource;

    // Controls for playback
    private bool isRandom = false; // Set true if you want random play

    // To track the current index
    private int currentTrackIndex = 0;

    private void Awake()
    {
        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = false; // No looping, we handle track transitions
    }

    private void Start()
    {
        if (musicTracks.Count > 0)
        {
            // Start playing the music queue
            StartCoroutine(PlayMusicQueue());
        }
    }

    private IEnumerator PlayMusicQueue()
    {
        while (true) // Loop indefinitely over music tracks
        {
            if (!audioSource.isPlaying) // Wait until the current track finishes
            {
                // Select the next track
                PlayNextTrack();
            }

            yield return null; // Allow the coroutine to wait for a frame
        }
    }

    private void PlayNextTrack()
    {
        if (musicTracks.Count == 0) return; // No tracks available, return

        // Determine the next track
        if (isRandom)
        {
            // Play a random track
            currentTrackIndex = Random.Range(0, musicTracks.Count);
        }
        else
        {
            // Play the next track in sequence
            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count;
        }

        // Play the selected track
        AudioClip nextClip = musicTracks[currentTrackIndex];
        audioSource.clip = nextClip;
        audioSource.Play();
    }
}