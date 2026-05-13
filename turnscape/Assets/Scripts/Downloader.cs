using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Downloader : MonoBehaviour
{
    private Task<string> DownloadInventoryJsonAsyncHelper()
    {
        var tcs = new TaskCompletionSource<string>();

        StartCoroutine(DownloadInventoryJson(result =>
        {
            tcs.SetResult(result);
        }));

        return tcs.Task;
    }

    public IEnumerator DownloadInventoryJson(Action<string> onComplete)
    {
        string url = "https://turnscape-api.azurewebsites.net/Item/" + AuthManager.PlayerId;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            if (AuthManager.AccessToken != null)
            {
                request.SetRequestHeader("Authorization", $"Bearer {AuthManager.AccessToken}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                onComplete?.Invoke(request.downloadHandler.text);
            else
                onComplete?.Invoke(null);
        }
    }

    public IEnumerator UpdateInventoryPositions(List<UpdatePosDto> items)
    {
        string url = $"https://turnscape-api.azurewebsites.net/Item/update-positions/{AuthManager.PlayerId}";

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(items);

        if (json == null || json == "" || json == "[]") yield break;

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            if (AuthManager.AccessToken != null)
                request.SetRequestHeader("Authorization", $"Bearer {AuthManager.AccessToken}");

            Debug.Log(json);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error + " | " + request.downloadHandler.text);
            }
        }
    }
}