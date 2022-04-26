using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Socket.Soccer.WebAPI.Game
{
    public class GameHub : Hub<IGameClient>
    {
        private const string STATE = "state";

        private readonly IDistributedCache _cache;
        private readonly IClientStore _clientStore;

        public GameHub(IDistributedCache cache, IClientStore clientStore)
        {
            _cache = cache;
            _clientStore = clientStore;
        }

        public override async Task OnConnectedAsync()
        {
            _clientStore.Add(Context.ConnectionId, new List<Guid>());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _clientStore.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Csatalkozás után regisztráljuk a játékosokat
        /// </summary>
        /// <param name="playerIds"></param>
        /// <returns></returns>
        public async Task InitPlayers(List<Guid> playerIds)
        {
            _clientStore.AddPlayers(Context.ConnectionId, playerIds);

            await Clients.User(Context.ConnectionId).ReceiveInitResponse("Regisztrálva.");
        }

        /// <summary>
        /// Itt meg kell vizsgálni hogy a Client1, ne férhessen hozzá a Client2 játékosaihoz
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SendMessage(PlayerCommand command)
        {
            var gameState = await GetGameState();

            var calculatedGameState = await CalculateGameState(command, gameState);

            // Elmentjük az állást
            await _cache.SetStringAsync(STATE, JsonConvert.SerializeObject(calculatedGameState));

            await Clients.All.ReceiveMessage(calculatedGameState);
        }


        /// <summary>
        /// Kiszámoljuk, hol a labda és hol a játékos
        /// </summary>
        /// <param name="command"></param>
        /// <param name="gameState"></param>
        /// <returns></returns>
        private async Task<GameState> CalculateGameState(PlayerCommand command, GameState gameState)
        {
            var player = gameState.Players.FirstOrDefault(x => x.Id == command.PlayerId);
            if (player != null)
            {
                switch (command.Command)
                {
                    case CommandType.Kick:
                        {
                            // El tudja rúgni, mert nála a labda
                            if (player.IsBallHere(gameState.Ball))
                            {
                                gameState.Ball.SetNewPosition(command.Direction, player.KickStrength);
                            }
                        }
                        break;

                    case CommandType.Move:
                        {
                            var (x, y) = DeterminePosition(player.Position, command.Direction);
                            player.Position.X = x;
                            player.Position.Y = y;
                        }
                        break;

                        // TODO: Passzolás
                }
            }

            return gameState;
        }

        private static (double x, double y) DeterminePosition(Position position, Direction direction)
        {
            return direction switch
            {
                Direction.North => (position.X, position.Y + position.Direction),
                Direction.Easth => (position.X + position.Direction, position.Y),
                Direction.South => (position.X, position.Y + position.Direction),
                Direction.West => (position.X + position.Direction, position.Y),
                _ => (5, 0),
            };
        }

        /// <summary>
        /// Csak simán a cache-ben tároljuk a játékot
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<GameState> GetGameState()
        {
            GameState? gameState;
            var state = await _cache.GetStringAsync(STATE);
            if (string.IsNullOrWhiteSpace(state))
            {
                gameState = new GameState();
                await _cache.SetStringAsync(STATE, JsonConvert.SerializeObject(gameState));
            }
            else
            {
                gameState = JsonConvert.DeserializeObject<GameState>(state);
                if (gameState == null)
                {
                    throw new Exception(JsonConvert.SerializeObject(gameState));
                }
            }
            return gameState;
        }
    }
}
