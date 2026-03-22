using System;
using UnityEngine;

public class LoginService
{
    public bool ValidateFields(string username, string password)
    {
        return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
    }

    public string FormatErrorMessage(string error)
    {
        if (!error.Contains("4")) return "Cannot reach the server.";

        switch (error.Substring(error.IndexOf("4"), 3))
        {
            case "400": return "Username already in use.";
            case "401": return "Invalid Username or Password";
            case "404": return "Cannot connect to the server";
            default:    return error;
        }
    }

    public (string userId, string username) ParseLoginResponse(string response)
    {
        LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
        string userId = JwtHelper.GetClaim(data.accessToken,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        string username = JwtHelper.GetClaim(data.accessToken,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        return (userId, username);
    }
}