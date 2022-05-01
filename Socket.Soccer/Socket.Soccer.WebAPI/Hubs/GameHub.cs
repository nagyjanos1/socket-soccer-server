using Microsoft.AspNetCore.SignalR;
using Socket.Soccer.WebAPI.Entities;
using Socket.Soccer.WebAPI.Game;

namespace Socket.Soccer.WebAPI.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameplay _gameplay;

        public GameHub(IGameplay gameplay)
        {
            _gameplay = gameplay;
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

        /// <summary>
        /// Csatalkozás után regisztráljuk a játékosokat
        /// </summary>
        /// <param name="playerIds"></param>
        /// <returns></returns>
        public async Task InitPlayers(List<Guid> playerIds)
        {
            await _gameplay.InitPlayers(Context.ConnectionId, playerIds);

            await Clients.Caller.SendAsync(GameHubHelpers.PLAYERS_INITED, "Players of the client were added.");
        }

        /// <summary>
        /// Itt meg kell vizsgálni hogy a Client1, ne férhessen hozzá a Client2 játékosaihoz
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task HandlePlayerCommand(PlayerCommand command)
        {
            var gameState = await _gameplay.HandleClientCommand(command);

            await Clients.All.SendAsync(GameHubHelpers.GET_GAMESTATE, gameState);
        }       

        public async Task ResetServer()
        {
            await _gameplay.ResetGameplay();
            await Clients.All.SendAsync(GameHubHelpers.SERVER_WAS_RESET, "Game was reset.");
        }
    }
}
