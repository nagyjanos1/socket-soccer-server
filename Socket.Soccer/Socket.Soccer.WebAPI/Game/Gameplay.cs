using Socket.Soccer.WebAPI.Entities;
using Socket.Soccer.WebAPI.Stores;

namespace Socket.Soccer.WebAPI.Game
{
    public class Gameplay : IGameplay
    {
        private readonly IGameStore _gameStore;
        private readonly IClientStore _clientStore;
        private readonly ILogger<Gameplay> _logger;

        public Gameplay(IGameStore gameStateStore, IClientStore clientStore, ILogger<Gameplay> logger)
        {
            _gameStore = gameStateStore;
            _clientStore = clientStore;
            _logger = logger;
        }

        public async Task<Team> RegisterClient(string clientId)
        {
            var team = await _clientStore.Add(clientId, new List<Guid>());
            await InitGameState();
            return team;
        }

        public async Task UnregisterClient(string clientId)
        {
            await _clientStore.Remove(clientId);
        }

        public async Task ResetGameplay()
        {
            await _clientStore.Reset();
            await _gameStore.ResetGame();
        }

        public async Task<Entities.Game> InitPlayers(string clientId, List<Guid> playerIds)
        {
            var team = await _clientStore.Get(clientId);
            await _clientStore.AddPlayers(clientId, playerIds);

            var gameState = await _gameStore.GetOrCreateGame();
            if (!gameState.Players.Any())
            {
                gameState.Players = new List<Player>();
            }

            foreach (var playerId in playerIds)
            {
                gameState.Players.Add(new Player
                {
                    Id = playerId,
                    Position = new Position(team?.Team == Team.Home ? 1 : -1, 0, team?.Team == Team.Home ? 1 : -1),
                    Team = team?.Team ?? Team.Home
                });
            }

            await _gameStore.SaveGame(gameState);

            return gameState;
        }

        /// <summary>
        /// Itt meg kell vizsgálni hogy a Client1, ne férhessen hozzá a Client2 játékosaihoz
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<Entities.Game> HandleClientCommand(PlayerCommand command)
        {
            var gameState = await _gameStore.GetOrCreateGame();

            var calculatedGameState = CalculateGameState(command, gameState);

            // Elmentjük az állást
            await _gameStore.SaveGame(calculatedGameState);

            return gameState;
        }

        private async Task InitGameState()
        {
            var gameState = await _gameStore.GetOrCreateGame();
            await _gameStore.SaveGame(gameState);
        }

        /// <summary>
        /// Kiszámoljuk, hol a labda és hol a játékos
        /// </summary>
        /// <param name="command"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private Entities.Game CalculateGameState(PlayerCommand command, Entities.Game game)
        {


            var player = game.Players.FirstOrDefault(x => x.Id == command.PlayerId);
            if (player != null)
            {
                switch (command.Command)
                {
                    case CommandType.Kick:
                        {
                            // El tudja rúgni, mert nála a labda
                            if (player.IsBallHere(game.Ball))
                            {
                                game.Ball.SetNewPosition(command.Direction, player.KickStrength);

                                if (game.Pit.IsGoal(game.Ball, out var team))
                                {
                                    game.State.IsGoal = team;

                                    if (team == Team.Home)
                                    {
                                        game.State.HomeScores++;                                        
                                    } 
                                    if (team == Team.Away)
                                    {
                                        game.State.AwayScores++;
                                    }                               
                                } 
                                else
                                {
                                    game.State.IsBallOut = game.Pit.IsBallOut(game.Ball);
                                }
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

            return game;
        }

        private static (int x, int y) DeterminePosition(Position position, Direction direction)
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
    }
}
