using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManSc : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();

    private bool initializing;

    private void Start()
    {
        LoadResolutions();
        LoadSavedSettings();
    }

    private void LoadResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        List<string> options = new List<string>();
        int currentIndex = 0;

        float currentWidth = Screen.currentResolution.width;
        float currentHeight = Screen.currentResolution.height;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];

            filteredResolutions.Add(res);
            options.Add($"{res.width} x {res.height}");

            if (res.width == currentWidth &&
                res.height == currentHeight)
            {
                currentIndex = filteredResolutions.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);

        currentIndex = Mathf.Clamp(currentIndex, 0, options.Count - 1);

        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        if (initializing) return;
        if (index < 0 || index >= filteredResolutions.Count) return;

        Resolution res = filteredResolutions[index];

        Screen.SetResolution(
            res.width,
            res.height,
            Screen.fullScreenMode);

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();

        StartCoroutine(RefreshUI());
    }

    public void SetFullscreen(bool fullscreen)
    {
        if (initializing) return;

        Screen.fullScreenMode = fullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();

        StartCoroutine(RefreshUI());
    }

    private void LoadSavedSettings()
    {
        initializing = true;

        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);

        fullscreenToggle.isOn = fullscreen;
        SetFullscreen(fullscreen);

        if (resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Count)
        {
            resolutionDropdown.value = resolutionIndex;
            SetResolution(resolutionIndex);
        }

        initializing = false;
    }

    private IEnumerator RefreshUI()
    {
        yield return null;

        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                canvas.GetComponent<RectTransform>()
            );
        }
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey("Fullscreen");
        PlayerPrefs.DeleteKey("ResolutionIndex");
        PlayerPrefs.DeleteKey("Volume");
        PlayerPrefs.Save();

        LoadSavedSettings();
    }
}