using System.Collections;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;


public class Networking : MonoBehaviour
{

    [SerializeField] private LoginScreen loginScreen;
    

    public void SendPostRequest(PlayerData data)
    {
        StartCoroutine(SendPost(data));
    }

    IEnumerator SendObject()
    {
        string url = "https://localhost:7232/user/login";

        //PlayerData data = new PlayerData("arminas", "Juodckis");
        PlayerData data = new PlayerData("a", "a");
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

    
    

    public IEnumerator SendPost(PlayerData player)
    {
        string url = "https://localhost:7232/user/login";

        // Create JSON data
        string jsonData = $"{{\"UserName\":\"{player.getUsername()}\",\"Password\":\"{player.getPassword()}\"}}";

        // Convert to byte array
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        // handle request
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        // if request was successful
        if (request.result == UnityWebRequest.Result.Success)
        {
            loginScreen.SucessfullLogin(player);
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        // request failed
        else
        {
            loginScreen.SetErrorMessage(request.error);
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