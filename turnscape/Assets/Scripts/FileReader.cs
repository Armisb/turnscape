using UnityEngine;

public static class FileReader
{
    public static Sprite GetTextureSprite(string fileName)
    {
        string fileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(fileName);

        Sprite sprite = Resources.Load<Sprite>($"Textures/{fileNameNoExt}");

        if (sprite == null)
        {
            Debug.Log($"Sprite not found in Resources/Textures: {fileNameNoExt}");
        }

        return sprite;
    }
}