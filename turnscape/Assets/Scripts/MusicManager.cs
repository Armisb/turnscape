using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MusicManager : LoaderBehaviour<MusicManager>
{
    public AudioSource audioSource;
    public string currentMusic = "";

    private void Start()
    {
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == currentMusic) return;

        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM/" + sceneName + "BGM");

        if (clip == null)
        {
            Debug.LogWarning("No Bgm found in the scene.");
            return;
        }
        audioSource.clip = clip;
        audioSource.Play();

        currentMusic = sceneName;
    }

    public void SetVolume(float vol)
    {
        audioSource.volume = vol;
        
        Debug.Log($"Volume: " + audioSource.volume);
    }
}
