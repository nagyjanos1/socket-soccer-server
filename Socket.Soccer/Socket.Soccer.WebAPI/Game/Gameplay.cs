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
                var direction = team?.Team == Team.Home ? 1 : -1;
                var starPosX = team?.Team == Team.Home ? 12 : 14;
                var starPosY = team?.Team == Team.Home ? 7 :9;
                gameState.Players.Add(new Player
                {
                    Id = playerId,
                    Position = new Position(starPosX, starPosY, direction),
                    Team = team?.Team ?? Team.Home
                });
            }

            await _gameStore.SaveGame(gameState);

            return gameState;
        }

        public async Task<Entities.Game?> HandleClientCommand(PlayerCommand command)
        {
            var gameState = await _gameStore.GetOrCreateGame();

            if (!await _clientStore.ArePlayersReadyToGame())
            {
                return null;
            }

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
                            if (player.CanKickTheBall(game.Ball))
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
                            var (x, y) = DetermineNewPosition(player.Position, command.Direction, player.StepSize);
                            (x, y) = ValidateNewPosition(x, y);
                            if (!game.Pit.IsOccupied(x, y))
                            {
                                game.Pit.ClearTile(player.Position);
                                
                                player.Position.X = x;
                                player.Position.Y = y;

                                game.Pit.SetTile(player.Position, player);
                            }                            
                        }
                        break;

                    case CommandType.Trick:
                        break;

                    case CommandType.Pass:
                        break;
                }
            }

            return game;
        }

        private static (int x, int y) DetermineNewPosition(Position position, Direction direction, int stepSize)
        {
            return direction switch
            {
                Direction.North => (position.X, position.Y - stepSize),
                Direction.Easth => (position.X + stepSize, position.Y),
                Direction.South => (position.X, position.Y + stepSize),
                Direction.West => (position.X - stepSize, position.Y),
                _ => (12, 8),
            };
        }

        private static (int x, int y) ValidateNewPosition(int x, int y)
        {
            var _x = x;
            var _y = y;
            if (x < 0)
            {
                _x = 0;
            }
            if (x > FootballPit.WIDTH)
            {
                _x = FootballPit.WIDTH - 1;
            }
            if (y < 0)
            {
                _y = 0;
            }
            if (y > FootballPit.HEIGHT)
            {
                _y = FootballPit.HEIGHT - 1;
            }
            return (_x, _y);
        }
    }
}
