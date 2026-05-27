using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManSc : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private List<Resolution> resolutions = new();
    private bool initializing;

    private void Start()
    {
        initializing = true;
        LoadResolutions();
        LoadSavedSettings();
        initializing = false;
    }

    private void LoadResolutions()
    {
        resolutionDropdown.ClearOptions();
        resolutions.Clear();

        Resolution[] all = Screen.resolutions;
        List<string> options = new();

        for (int i = 0; i < all.Length; i++)
        {
            Resolution r = all[i];
            resolutions.Add(r);
            options.Add($"{r.width} x {r.height} {(int)r.refreshRateRatio.value} Hz");
        }

        resolutionDropdown.AddOptions(options);
    }

    public void SetResolution(int index)
    {
        if (initializing) return;
        if (index < 0 || index >= resolutions.Count) return;

        Resolution r = resolutions[index];

        Screen.SetResolution(
            r.width,
            r.height,
            Screen.fullScreenMode,
            Mathf.RoundToInt((float)r.refreshRateRatio.value)
        );

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool fullscreen)
    {
        if (initializing) return;

        Screen.fullScreenMode = fullscreen
            ? FullScreenMode.ExclusiveFullScreen
            : FullScreenMode.Windowed;

        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSavedSettings()
    {
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        fullscreenToggle.isOn = fullscreen;
        SetFullscreen(fullscreen);

        savedIndex = Mathf.Clamp(savedIndex, 0, resolutions.Count - 1);

        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        SetResolution(savedIndex);
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey("ResolutionIndex");
        PlayerPrefs.DeleteKey("Fullscreen");
        PlayerPrefs.Save();
        Start();
    }
}