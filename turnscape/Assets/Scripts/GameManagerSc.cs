using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerSc : MonoBehaviour
{
    public static GameManagerSc Instance;

    [Header("UI Elements")]
    public GameObject loadingPanel;
    public GameObject miscCanvas;
    public Image fillImage;
    public TMP_Text percentageText;

    public Downloader downloader;

    public Camera MainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        yield return null;

        LoaderBehaviour.LoadAllUnloaded();

        SetCanvasCamera();
    }

    public void SetCanvasCamera()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            cam = MainCamera;

            if (cam == null)
            {
                Debug.LogWarning("No camera found (Main or fallback).");
                return;
            }
        }

        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceCamera || canvas.worldCamera == null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = cam;
            }
        }
    }

    public static void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public static void LoadScene(string sceneName)
    {
        Instance.loadingPanel.SetActive(true);
        Instance.StartCoroutine(Instance.LoadAsync(sceneName));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            fillImage.fillAmount = progress;
            percentageText.text = (progress * 100f).ToString("F0") + "%";

            if (asyncLoad.progress >= 0.9f)
            {
                fillImage.fillAmount = 1f;
                percentageText.text = "100%";
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        SetCanvasCamera();

        var enumerator = LoaderBehaviour.SceneReloadAll();
        while (enumerator.MoveNext())
        {
            fillImage.fillAmount += 1f / LoaderBehaviour.Loaders.Count;
            percentageText.text = (fillImage.fillAmount * 100f).ToString("F0") + "%";

            yield return enumerator.Current;
        }

        yield return null;

        Instance.loadingPanel.SetActive(false);
    }
}