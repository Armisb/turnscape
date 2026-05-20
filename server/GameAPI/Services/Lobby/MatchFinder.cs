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
		bool match_found = true;
		while (match_found && !stoppingToken.IsCancellationRequested) {
                    // Create a new scope for each iteration
                    using var scope = _services.CreateScope();
                    var lobbyService = scope.ServiceProvider.GetRequiredService<ILobbyService>();
                    match_found = await lobbyService.FindMatch();
		    await Task.Delay(2, stoppingToken);
		}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Matchmaker error: " + ex.Message);
            }

            await Task.Delay(1000, stoppingToken); // check every second
        }
    }
}
