using System;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;
    
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip buySound;
    

    private void Awake()
    {
        Instance = this;
    }

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
}
