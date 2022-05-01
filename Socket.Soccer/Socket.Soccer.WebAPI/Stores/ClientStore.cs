using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Socket.Soccer.WebAPI.Entities;

namespace Socket.Soccer.WebAPI.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly IDistributedCache _cache;

        public ClientStore(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<Team> Add(string playerClientId, List<Guid> playerIds)
        {
            var playerClients = await GetOrCreatePlayersList();

            if (playerClients.Count >= 2)
                throw new Exception("Nem léphetbe több játékos!");
            if (playerClients.Any(x => x.ClientId == playerClientId))
                throw new Exception("Ezzel az azonosítóval már regisztrált egy játékos.");
            var client = new PlayerClient
            {
                ClientId = playerClientId,
                PlayerIds = playerIds,
                Team = !playerClients.Any() ? Team.Home : Team.Away,
            };
            playerClients.Add(client);

            await SavePlayers(playerClients);

            return client.Team;
        }

        public async Task<PlayerClient?> Get(string playerClientId)
        {
            var playerClients = await GetOrCreatePlayersList();
            return playerClients.FirstOrDefault(x => x.ClientId == playerClientId);
        }

        public async Task<bool> Check(string playerClientId, Guid playerId)
        {
            var playerClients = await GetOrCreatePlayersList();

            var client = playerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (client == null)
                throw new Exception("Nincs ilyen kliens regisztrálva.");

            return client.PlayerIds.Contains(playerId);
        }

        public async Task Remove(string playerClientId)
        {
            var playerClients = await GetOrCreatePlayersList();

            var client = playerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (client == null)
                throw new Exception("Nincs ilyen kliens regisztrálva.");

            playerClients.Remove(client);
        }

        public async Task<bool> ArePlayersReadyToGame()
        {
            var playerClients = await GetOrCreatePlayersList();
            return playerClients.Count > 1;
        }

        public async Task AddPlayers(string playerClientId, List<Guid> playerIds)
        {
            var playerClients = await GetOrCreatePlayersList();

            var client = playerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (client == null)
                throw new Exception("Nincs ilyen kliens regisztrálva.");

            client.PlayerIds = playerIds;
        }

        private async Task<List<PlayerClient>> GetOrCreatePlayersList()
        {
            List<PlayerClient>? players;
            var key = nameof(List<PlayerClient>);
            var playersAsStr = await _cache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(playersAsStr))
            {
                players = new List<PlayerClient>();
                await _cache.SetStringAsync(key, JsonConvert.SerializeObject(players));
            }
            else
            {
                players = JsonConvert.DeserializeObject<List<PlayerClient>>(playersAsStr);
                if (players == null)
                {
                    throw new Exception(JsonConvert.SerializeObject(players));
                }
            }
            return players;
        }

        private async Task SavePlayers(List<PlayerClient> playersIds)
        {
            await _cache.SetStringAsync(nameof(List<PlayerClient>), JsonConvert.SerializeObject(playersIds));
        }

        public async Task Reset()
        {
            await _cache.RemoveAsync(nameof(List<PlayerClient>));
        }        
    }
}
