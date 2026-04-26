using System;
using Microsoft.AspNetCore.SignalR;

namespace GameAPI.Hubs;

public class QueryUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.GetHttpContext()?.Request.Query["userId"];
    }
}
