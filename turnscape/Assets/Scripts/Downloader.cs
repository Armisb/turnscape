using System.Collections;
using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class Downloader : MonoBehaviour
{
    public void GetInventoriesFromServer()
    {
        StartCoroutine(DownloadInventoryJson());
    }

    private IEnumerator DownloadInventoryJson()
    {
        string url = "https://turnscape-api.azurewebsites.net/Item";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            string path = Path.Combine(Application.dataPath, "InventoryData.json");
            File.WriteAllText(path, json);

            Debug.Log("Inventory JSON saved");
        }
        else
        {
            Debug.Log("Failed to download inventory JSON: " + request.error);
        }
    }
}
