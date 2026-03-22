using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class Networking : MonoBehaviour
{
    [SerializeField] private LoginScreen loginScreen;
    private static string defaultBaseUrl = "https://turnscape-api.azurewebsites.net/"; 
    private void Awake()
    {
        if (AuthManager.AccessToken != null)
        {
            var data = new LoginResponse
            {
                accessToken = AuthManager.AccessToken,
                refreshToken = AuthManager.RefreshToken
            };
            loginScreen.SucessfullLogin(JsonUtility.ToJson(data));
        }
    }

    public void SendPostGeneric(string url, string jsonData, Action<String> onSuccess, Action<String> onError)
    {
        StartCoroutine(SendRequestGen("POST", url, jsonData, onSuccess, onError));
    }
    
    public void SendGetGeneric(string url, string jsonData, Action<String> onSuccess, Action<String> onError)
    {
        StartCoroutine(SendRequestGen("GET", url, jsonData, onSuccess, onError));
    }
    
    public void Logout()
    {
        loginScreen.LogOut();
    }

    private IEnumerator SendRequestGen(string type, string url, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        // convert to byte array
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        // handle request
        UnityWebRequest request = new UnityWebRequest(defaultBaseUrl+url, type);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();
        request.SetRequestHeader("Content-Type", "application/json");

        if (AuthManager.AccessToken != null)
        {
            request.SetRequestHeader("Authorization", $"Bearer {AuthManager.AccessToken}");
        }

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            onError?.Invoke(request.error);
            Debug.LogError("Error: " + request.error);
        }
    }
}

[System.Serializable]
public class LoginResponse
{
    public string accessToken;
    public string refreshToken;
}

public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // always accept
    }
}