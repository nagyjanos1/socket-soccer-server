using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Socket.Soccer.WebAPI.Entities;
using Socket.Soccer.WebAPI.Hubs;

namespace Socket.Soccer.WebAPI.Background
{
    internal class ScopedProcessingService : IScopedProcessingService
    {
        private readonly IDistributedCache _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public ScopedProcessingService(IDistributedCache cache, IServiceProvider serviceProvider, ILogger<ScopedProcessingService> logger)
        {
            _cache = cache;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var hubContext = scope.ServiceProvider.GetService<IHubContext<GameHub>>();

                    var gamestateJson = _cache.GetString(nameof(GameState));
                    if (string.IsNullOrEmpty(gamestateJson)) {
                        await Task.Delay(1000, stoppingToken);
                        continue;
                    }

                    var gamestate = JsonConvert.DeserializeObject<GameState>(gamestateJson);
                    
                    await hubContext.Clients.All.SendAsync(GameHubHelpers.GET_GAMESTATE, gamestate).ConfigureAwait(false);

                    _logger.LogInformation("Sent a gamestate message to all.");

                    await Task.Delay(1000, stoppingToken);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                }                
            }
        }
    }
}
