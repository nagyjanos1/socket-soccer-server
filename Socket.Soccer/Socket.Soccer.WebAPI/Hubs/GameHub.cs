using Microsoft.AspNetCore.SignalR;
using Socket.Soccer.WebAPI.Entities;
using Socket.Soccer.WebAPI.Game;

namespace Socket.Soccer.WebAPI.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameplay _gameplay;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IGameplay gameplay, ILogger<GameHub> logger)
        {
            _gameplay = gameplay;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var team = await _gameplay.RegisterClient(Context.ConnectionId);
                       
            await base.OnConnectedAsync();

            await Clients.Caller.SendAsync("OnConnectedAsync", team);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _gameplay.UnregisterClient(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public Task ThrowException() => throw new HubException("This error will be sent to the client.");

        public async Task InitPlayers(List<Guid> playerIds)
        {
            await _gameplay.InitPlayers(Context.ConnectionId, playerIds);

            await Clients.Caller.SendAsync(GameHubHelpers.PLAYERS_INITED, "Players of the client were added.");
        }

        public async Task HandlePlayerCommand(PlayerCommand command)
        {
            try
            {
                _ = await _gameplay.HandleClientCommand(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }       

        public async Task ResetServer()
        {
            await _gameplay.ResetGameplay();
            await Clients.All.SendAsync(GameHubHelpers.SERVER_WAS_RESET, "Game was reset.");
        }
    }
}
