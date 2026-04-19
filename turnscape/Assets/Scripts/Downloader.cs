using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Downloader : MonoBehaviour
{
    public IEnumerator DownloadInventoryJson(Action<string> onComplete)
    {
        string url = "https://turnscape-api.azurewebsites.net/Item/" + AuthManager.PlayerId;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                onComplete?.Invoke(request.downloadHandler.text);
            else
                onComplete?.Invoke(null);
        }
    }

    public IEnumerator SaveInventoryJson(string json, System.Action<bool> onComplete = null)
    {
        string url = "https://turnscape-api.azurewebsites.net/Item/" + AuthManager.PlayerId;

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;
            onComplete?.Invoke(success);
        }
    }
}