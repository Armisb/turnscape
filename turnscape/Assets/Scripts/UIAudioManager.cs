using UnityEngine;
using UnityEngine.UI;

public class UIAudioManager : LoaderBehaviour<UIAudioManager>
{
    public AudioSource audioSource;

    [Header("UI")]
    public Slider volumeSlider;

    [Header("Sounds")]
    public AudioClip clickSound;
    public AudioClip buySound;

    private const string VolumeKey = "SFXVolume";

    private void Start()
    {
        LoadVolume();

        volumeSlider.onValueChanged.AddListener(SetVolume);
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

        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        audioSource.volume = savedVolume;

        if (volumeSlider != null)
            volumeSlider.value = savedVolume;
    }

    public void ResetVolume()
    {
        PlayerPrefs.DeleteKey(VolumeKey);

        audioSource.volume = 1f;

        if (volumeSlider != null)
            volumeSlider.value = 1f;
    }
}