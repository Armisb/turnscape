using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManSc : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Aspect Lock")]
    [SerializeField] private Camera targetCamera;

    private List<Resolution> resolutions = new();
    private bool initializing;

    private const float targetAspect = 16f / 9f;

    private void Start()
    {
        initializing = true;

        if (targetCamera == null)
            targetCamera = Camera.main;

        LoadResolutions();
        LoadSavedSettings();

        initializing = false;
    }

    private void Update()
    {
        UpdateAspectRatio();
    }

    private void UpdateAspectRatio()
    {
        if (targetCamera == null) return;

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = targetCamera.rect;

        if (scaleHeight < 1.0f)
        {
            // black bars top/bottom
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            // black bars left/right
            float scaleWidth = 1f / scaleHeight;

            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
        }

        targetCamera.rect = rect;
        targetCamera.backgroundColor = Color.black;
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