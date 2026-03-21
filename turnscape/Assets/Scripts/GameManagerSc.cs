using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.LightingExplorerTableColumn;

public class GameManagerSc : MonoBehaviour
{
    public static GameManagerSc Instance;

    [Header("UI Elements")]
    public GameObject loadingPanel;
    public GameObject miscCanvas;
    public Image fillImage;
    public TMP_Text percentageText;

    public Downloader downloader;

    public static event System.Action OnStage_AfterSceneLoad;
    public static event System.Action OnStage_Locate;
    public static event System.Func<IEnumerator> OnStage_Load;
    public static event System.Action OnStage_Bind;
    public static event System.Action OnStage_Finalize;

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
    }

    public void SetCanvasCamera()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogWarning("Main Camera not found in scene.");
            return;
        }

        Canvas canvas = miscCanvas.GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
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

    private System.Collections.IEnumerator LoadAsync(string sceneName)
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

        /*if (sceneName == "BaseScene")
        {
            Debug.Log("loaders");
            InventoryManSc.Instance.RebuildSceneInventories();
            StatisticsSc.Instance.LocateStatisticsUI();
        }*/

        LoaderBehaviour.LoadAllUnloaded();

        SetCanvasCamera();

        loadingPanel.SetActive(false);
    }
}