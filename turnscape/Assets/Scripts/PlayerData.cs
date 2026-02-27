using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private string Username {  get;  set; }
    private string Password { get;  set; }

    public static PlayerData newPlayerObject(string username, string password)
    {
        GameObject go = new GameObject();
        go.AddComponent<PlayerData>();
        go.GetComponent<PlayerData>().Username = username;
        go.GetComponent<PlayerData>().Password = password;
        return go.GetComponent<PlayerData>();
    }

    public string getUsername()
    {
        return Username;
    }

    public string getPassword()
    {
        return Password;
    }

}
