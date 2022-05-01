using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Socket.Soccer.WebAPI.Stores
{
    public class GameStateStore : IGameStore
    {
        private readonly IDistributedCache _cache;

        public GameStateStore(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<Entities.Game> GetOrCreateGame()
        {
            Entities.Game? gameState;
            var state = await _cache.GetStringAsync(nameof(Entities.Game));
            if (string.IsNullOrWhiteSpace(state))
            {
                gameState = new Entities.Game();
                await SaveGame(gameState);
            }
            else
            {
                gameState = JsonConvert.DeserializeObject<Entities.Game>((string)state);
                if (gameState == null)
                {
                    throw new Exception(JsonConvert.SerializeObject(gameState));
                }
            }
            return gameState;
        }

        public async Task SaveGame(Entities.Game gameState)
        {
            // Elmentjük az állást
            await _cache.SetStringAsync(nameof(Entities.Game), JsonConvert.SerializeObject(gameState));
        }

        public async Task ResetGame()
        {
            var gameState = new Entities.Game();
            await SaveGame(gameState);
        }
    }
}
