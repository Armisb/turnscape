using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


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
        PlayerData data = PlayerData.newPlayerObject("a", "a");
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

    
    IEnumerator SendPost(PlayerData playerData)
    {
        string url = "https://localhost:7232/user/login";

        // Create JSON data
        //string jsonData = "{\"UserName\":\"arminas\",\"Password\":\"Juodckis\"}";
        string jsonData = $"{{\"UserName\":\"{playerData.getUsername()}\",\"Password\":\"{playerData.getPassword()}\"}}";

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
            loginScreen.sucessfullRequest = true;
            Debug.Log("Response: " + request.downloadHandler.text);
        
        }
        else
        {
            loginScreen.sucessfullRequest = false;
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