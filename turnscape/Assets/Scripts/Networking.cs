using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class Networking : MonoBehaviour
{
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

    [SerializeField] private LoginScreen loginScreen;

    public void SendPostGeneric(string url, string jsonData, Action<String> onSuccess, Action<String> onError)
    {
        StartCoroutine(SendPostGen(url, jsonData, onSuccess, onError));
    }
    
    public void Logout()
    {
        loginScreen.LogOut();
    }

    private IEnumerator SendPostGen(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        // convert to byte array
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        // handle request
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();
        request.SetRequestHeader("Content-Type", "application/json");

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