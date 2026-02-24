using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string Username {  get;  set; }
    public string Password { get;  set; }

    public PlayerData(string username, string password)
    {
        this.Username = username;
        this.Password = password;
    }


}
