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
        // Replace with your actual endpoint
        string url = "https://localhost:7232/Item/9c98d905-6526-4c8d-b4db-73715d8d0206";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Inventory JSON received:\n" + json);

            // Save to Assets folder so you can inspect it
            string path = Path.Combine(Application.dataPath, "InventoryData.json");
            File.WriteAllText(path, json);

            Debug.Log("Inventory JSON saved to: " + path);
        }
        else
        {
            Debug.LogError("Error downloading inventory JSON: " + request.error);
        }
    }
}
