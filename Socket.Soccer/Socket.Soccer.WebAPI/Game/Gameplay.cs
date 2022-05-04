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
            var game = await _gameStore.GetOrCreateGame();
            game.ResetState();
            await _gameStore.SaveGame(game);
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
                    Position = team?.Team == Team.Home ? Player.HOME_START_POSITION : Player.AWAY_START_POSITION,
                    Team = team?.Team ?? Team.Home
                });
            }

            await _gameStore.SaveGame(gameState);

            return gameState;
        }

        public async Task<Entities.Game?> HandleClientCommand(PlayerCommand command)
        {
            var game = await _gameStore.GetOrCreateGame();

            if (!await _clientStore.ArePlayersReadyToGame())
            {
                return null;
            }

            var calculatedGameState = CalculateGameState(command, game);

            // Elmentjük az állást
            await _gameStore.SaveGame(calculatedGameState);

            return game;
        }

        private async Task InitGameState()
        {
            var gameState = await _gameStore.GetOrCreateGame();
            await _gameStore.SaveGame(gameState);
        }

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
                                var (x, y) = CalculateBallPosition(game.Ball, command.Direction, player.KickStrength);
                                if (!game.Pit.IsOccupied(x, y))
                                {
                                    game.Pit.ClearTile(game.Ball.Position);

                                    game.Ball.Position.X = x;
                                    game.Ball.Position.Y = y;

                                    game.Pit.SetTile(game.Ball.Position, game.Ball);
                                }

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
                            var (x, y) = CalculatePlayerPosition(player, command.Direction, game.Pit);
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

        private static Direction GetRandomDirection()
        {
            Random rnd = new();
            var num = rnd.Next(0, 4);
            return (Direction)num;
        }

        private (int x, int y) CalculatePlayerPosition(Player player, Direction direction, FootballPit pit)
        {
            var (x, y) = DetermineNewPosition(player.Position, direction, player.StepSize);
            (x, y) = ValidateNewPosition(x, y);

            if (x == player.Position.X && y == player.Position.Y)
            {
                var randomDir = GetRandomDirection();
                (x, y) = CalculatePlayerPosition(player, randomDir, pit);
            }

            return (x, y);                                 
        }

        private (int x, int y) CalculateBallPosition(Ball ball, Direction direction, int kickStrength)
        {
            var (x, y) = ball.DetermineNewPosition(ball.Position, direction, kickStrength);
            (x, y) = ValidateNewPosition(x, y);

            if (x == ball.Position.X && y == ball.Position.Y)
            {
                var randomDir = GetRandomDirection();
                (x, y) = CalculateBallPosition(ball, randomDir, kickStrength);
            }

            return (x, y);
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
            if (x >= FootballPit.WIDTH)
            {
                _x = FootballPit.WIDTH - 1;
            }
            if (y < 0)
            {
                _y = 0;
            }
            if (y >= FootballPit.HEIGHT)
            {
                _y = FootballPit.HEIGHT - 1;
            }
            return (_x, _y);
        }
    }
}
