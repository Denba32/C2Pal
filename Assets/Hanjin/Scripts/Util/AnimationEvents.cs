using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOnePlay(AudioClip clip)
    {
        if(audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.PlayOneShot(clip, 0.8f);
        }
    }

    public void PlayOnePlay(AudioClip clip, float volume)
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.PlayOneShot(clip, volume);
        }
    }
}
