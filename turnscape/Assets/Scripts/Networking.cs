using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class Networking : MonoBehaviour
{
    void Start()
    {

    }

    public void SendPostRequest(string Username, string Password)
    {
        PlayerData data = new PlayerData(Username, Password);

        StartCoroutine(SendPost(data));
    }

    IEnumerator SendObject()
    {
        string url = "https://localhost:7232/user/login";

        PlayerData data = new PlayerData("arminas", "Juodckis");

        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    [ContextMenu("Send Post")]
    IEnumerator SendPost(PlayerData player)
    {
        string url = "https://localhost:7232/user/login";

        // Create JSON data
        //string jsonData = "{\"UserName\":\"arminas\",\"Password\":\"Juodckis\"}";
        string jsonData = $"{{\"UserName\":\"{player.Username}\",\"Password\":\"{player.Password}\"}}";

        // Convert to byte array
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new AcceptAllCertificates();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);

        }
        else
        {
            Debug.LogError("Error: " + request.error);
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