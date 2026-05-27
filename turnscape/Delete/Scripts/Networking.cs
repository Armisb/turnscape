using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;
using shared_lib;
using Newtonsoft.Json;


public static class Networking
{
    public static string defaultBaseUrl = "https://turnscape-api.azurewebsites.net/";

        public static async Task SendDeleteGeneric(string url, object jsonData, Action<string> onSuccess, Action<string> onError)
        {
            await SendRequestGen("DELETE", url, jsonData, onSuccess, onError);
        }

    public static async Task SendPostGeneric(string url, object jsonData, Action<string> onSuccess, Action<string> onError)
        {
            await SendRequestGen("POST", url, jsonData, onSuccess, onError);
        }

        public static async Task SendGetGeneric(string url, object jsonData, Action<string> onSuccess, Action<string> onError)
        {
            await SendRequestGen("GET", url, jsonData, onSuccess, onError);
        }

        private static async Task SendRequestGen(string type, string url, object jsonData, Action<string> onSuccess, Action<string> onError)
        {
            
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonData) ?? "");

            using UnityWebRequest request = new UnityWebRequest(defaultBaseUrl + url, type);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.certificateHandler = new AcceptAllCertificates();
            request.SetRequestHeader("Content-Type", "application/json");

            if (AuthManager.AccessToken != null)
                request.SetRequestHeader("Authorization", $"Bearer {AuthManager.AccessToken}");

            // Await the web request
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                onError?.Invoke(request.downloadHandler.text);
            }
        }
    }


public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // always accept
    }
}
