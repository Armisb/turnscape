using System;
using UnityEngine;

public class UIAudioManager : LoaderBehaviour<UIAudioManager>
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip buySound;

    public void ChangeVolume(int volume)
    {
        audioSource.volume = volume;
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void PlayBuySound()
    {
        audioSource.PlayOneShot(buySound);
    }

    public void PlayCustomSound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
