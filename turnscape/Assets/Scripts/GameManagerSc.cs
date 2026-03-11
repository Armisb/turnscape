using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManagerSc : MonoBehaviour
{
    public static GameManagerSc Instance;

    [Header("UI Elements")]
    public GameObject loadingPanel;
    public Image fillImage;
    public TMP_Text percentageText;

    public Downloader downloader;

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

        if (sceneName == "BaseScene")
        {
            InventoryManSc.Instance.RebuildSceneInventories();
            StatisticsSc.Instance.LocateStatisticsUI();
        }

        loadingPanel.SetActive(false);
    }
}