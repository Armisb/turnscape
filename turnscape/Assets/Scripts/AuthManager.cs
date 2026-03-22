
public static class AuthManager
{
    public static string AccessToken { get; private set; }
    public static string RefreshToken { get; private set; }
    public static string PlayerName { get; private set; }
    public static string PlayerId { get; private set; }
    public static void SetPlayerName(string playerName) => PlayerName = playerName;
    public static void SetAccessToken(string token) => AccessToken = token;
    public static void SetRefreshToken(string token) => RefreshToken = token;
    public static void SetPlayerID (string id) => PlayerId = id;

    public static void ClearTokens()
    {
        AccessToken = null;
        RefreshToken = null;
        PlayerId = null;
    }
    
    
}
[System.Serializable]
public class LoginResponse
{
    public string accessToken;
    public string refreshToken;
}