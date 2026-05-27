using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicManager : LoaderBehaviour<MusicManager>
{
    public AudioSource audioSource;

    [Header("UI")]
    public Slider volumeSlider;

    public string currentMusic = "";

    private const string VolumeKey = "MusicVolume";

    protected override void Load()
    {
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;

        LoadVolume();

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

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

        PlayerPrefs.SetFloat(VolumeKey, vol);
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