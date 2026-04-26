using System;

namespace GameAPI.Services.Lobby;

public class MatchFinder :  BackgroundService
{

    private readonly IServiceProvider _services;

    public MatchFinder(IServiceProvider services)
    {
        _services = services;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a new scope for each iteration
                using var scope = _services.CreateScope();
                var lobbyService = scope.ServiceProvider.GetRequiredService<ILobbyService>();
                await lobbyService.FindMatch();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Matchmaker error: " + ex.Message);
            }

            await Task.Delay(1000, stoppingToken); // check every second
        }
    }
}
