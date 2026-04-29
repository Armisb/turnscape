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
    public TMP_Text loadBatchText;

    public Downloader downloader;
    public Camera MainCamera;

    private int batchIndex;
    private int batchTotal;
    private string batchLabel;

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

        loadingPanel.SetActive(true);
    }

    private IEnumerator Start()
    {
        SetCanvasCamera();

        yield return RunBatch(1, 1, "Loading game data", () =>
        {
            return RunLoadingBar(LoaderBehaviour.LoadAll());
        });

        yield return new WaitForEndOfFrame();
        loadingPanel.SetActive(false);
    }

    public static void LoadScene(string sceneName)
    {
        Instance.loadingPanel.SetActive(true);
        Instance.StartCoroutine(Instance.LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return RunBatch(1, 2, "Loading scene assets", () =>
        {
            return RunLoadingBar(LoadSceneAsync(sceneName));
        });

        SetCanvasCamera();

        yield return RunBatch(2, 2, "Loading game data", () =>
        {
            return RunLoadingBar(LoaderBehaviour.LoadAll());
        });

        yield return new WaitForEndOfFrame();
        loadingPanel.SetActive(false);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float p = Mathf.Clamp01(op.progress / 0.9f);
            UpdateProgress(p);

            if (op.progress >= 0.9f)
            {
                UpdateProgress(1f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator RunBatch(int index, int total, string label, System.Func<IEnumerator> routine)
    {
        batchIndex = index;
        batchTotal = total;
        batchLabel = label;

        UpdateBatchUI();

        yield return routine();

        UpdateBatchUI(1f);
    }

    private IEnumerator RunLoadingBar(IEnumerator routine)
    {
        while (routine.MoveNext())
        {
            if (routine.Current is IEnumerator nested)
                yield return nested;

            yield return null;

            UpdateProgress(LoaderBehaviour.Progress);
        }

        UpdateProgress(1f);
    }

    private void UpdateProgress(float progress)
    {
        fillImage.fillAmount = progress;
        percentageText.text = $"{(progress * 100f):F0}%";
    }

    private void UpdateBatchUI(float? forcedProgress = null)
    {
        if (forcedProgress.HasValue)
        {
            fillImage.fillAmount = forcedProgress.Value;
            percentageText.text = "100%";
        }

        loadBatchText.text = $"{batchLabel} {batchIndex}/{batchTotal}";
    }

    public void SetCanvasCamera()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            cam = MainCamera;
            if (cam == null) return;
        }

        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas canvas in canvases)
        {
            if (canvas.worldCamera == null && canvas != miscCanvas.GetComponent<Canvas>())
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = cam;
            }
        }
    }

    public static void QuitGame()
    {
        Instance.StartCoroutine(Instance.QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        loadingPanel.SetActive(true);

        yield return RunBatch(1, 1, "Saving game data", () =>
        {
            return RunLoadingBar(LoaderBehaviour.LoadAll(true));
        });

        yield return new WaitForEndOfFrame();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}