using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Downloader : MonoBehaviour
{
    public void GetInventoriesFromServer(System.Action<string> onComplete)
    {
        StartCoroutine(DownloadInventoryJson(onComplete));
    }

    private IEnumerator DownloadInventoryJson(System.Action<string> onComplete)
    {
        string url = "https://turnscape-api.azurewebsites.net/Item";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;

                Debug.Log("Downloaded JSON: " + json);

                onComplete?.Invoke(json);
            }
            else
            {
                Debug.LogError("Failed: " + request.error);
                onComplete?.Invoke(null);
            }
        }
    }
}