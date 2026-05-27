using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    private void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM/" + sceneName + "BGM");

        if (clip == null)
        {
            Debug.LogWarning("No Bgm found in the scene.");
            return;
        }
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void SetVolume(float vol)
    {
        audioSource.volume = vol;
        
        Debug.Log($"Volume: " + audioSource.volume);
    }
}
