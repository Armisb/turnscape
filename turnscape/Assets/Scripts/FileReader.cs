using System.IO;
using UnityEngine;

public static class FileReader
{
    public static string BaseAssetsPath = Application.dataPath;
    public static string BaseTexturesPath = Path.Combine(Application.dataPath, "Textures");

    public static Sprite GetTextureSprite(string fileName, string folderPath = null)
    {
        string path = folderPath ?? BaseTexturesPath;
        string fullPath = Path.Combine(path, fileName);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"File not found at '{fullPath}'");
            return null;
        }

        byte[] fileData = File.ReadAllBytes(fullPath);
        Texture2D tex = new Texture2D(2, 2);

        if (!tex.LoadImage(fileData))
            return null;

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}