using Microsoft.AspNetCore.SignalR;
using Socket.Soccer.WebAPI.Hubs;
using Socket.Soccer.WebAPI.Stores;

namespace Socket.Soccer.WebAPI.Background
{
    internal class ScopedProcessingService : IScopedProcessingService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IGameStore _gameStore;
        private readonly ILogger _logger;

        public ScopedProcessingService(IHubContext<GameHub> hubContext, IGameStore gameStore, ILogger<ScopedProcessingService> logger)
        {
            _hubContext = hubContext;
            _gameStore = gameStore;
            _logger = logger;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var game = await _gameStore.GetOrCreateGame();

                    await _hubContext.Clients.All.SendAsync(
                            GameHubHelpers.GET_GAMESTATE,
                            new
                            {
                                game.State.HomeScores,
                                game.State.AwayScores,
                                game.State.IsGoal,
                                game.State.IsBallOut,
                                ball = game.Ball,
                                players = game.Players,
                                isOpponentInitialized = game.Players.Count > 1
                            },
                            cancellationToken: stoppingToken)
                        .ConfigureAwait(false);

                    _logger.LogInformation("Sent a gamestate message to all.");

                    if (game.State.IsGoal != null || game.State.IsBallOut)
                    {
                        game.ResetState();

                        await _gameStore.SaveGame(game);

                        _logger.LogInformation("State was reseted.");
                    }

                    await Task.Delay(500, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }
    }
}
