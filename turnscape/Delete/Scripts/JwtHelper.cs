using System;
using System.Collections.Generic;
using UnityEngine;

public static class JwtHelper
{
    public static string GetClaim(string token, string claimKey)
    {
        var parts = token.Split('.');
        if (parts.Length != 3) return null;

        // Add padding if needed
        var payload = parts[1];
        payload = payload.Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        var data = JsonUtility.FromJson<Dictionary<string, string>>(json); // won't work — see below
        // Use SimpleJSON or manual parsing instead:
        return ExtractValue(json, claimKey);
    }

    private static string ExtractValue(string json, string key)
    {
        // Simple key search (no external lib needed)
        var searchKey = $"\"{key}\":\"";
        var start = json.IndexOf(searchKey);
        if (start < 0) return null;
        start += searchKey.Length;
        var end = json.IndexOf('"', start);
        return json.Substring(start, end - start);
    }
}