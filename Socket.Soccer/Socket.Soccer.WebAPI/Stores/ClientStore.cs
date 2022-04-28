using Socket.Soccer.WebAPI.Entities;

namespace Socket.Soccer.WebAPI.Stores
{
    public class ClientStore : IClientStore
    {
        public List<PlayerClient> PlayerClients { get; init; } = new List<PlayerClient>();

        public void Add(string playerClientId, List<Guid> playerIds)
        {
            if (PlayerClients.Count >= 2)
                throw new Exception("Nem léphetbe több játékos!");
            if (PlayerClients.Any(x => x.ClientId == playerClientId))
                throw new Exception("Ezzel az azonosítóval már regisztrált egy játékos.");
            PlayerClients.Add(new PlayerClient
            {
                ClientId = playerClientId,
                PlayerIds = playerIds,
                IsHome = !PlayerClients.Any()
            });
        }

        public PlayerClient? Get(string playerClientId)
        {
            return PlayerClients.FirstOrDefault(x => x.ClientId == playerClientId);
        }

        public bool Check(string playerClientId, Guid playerId)
        {
            var client = PlayerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (client == null)
                throw new Exception("Nincs ilyen kliens regisztrálva.");

            return client.PlayerIds.Contains(playerId);
        }

        public void Remove(string playerClientId)
        {
            var client = PlayerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (client == null)
                throw new Exception("Nincs ilyen kliens regisztrálva.");

            PlayerClients.Remove(client);
        }

        public void AddPlayers(string playerClientId, List<Guid> playerIds)
        {
            var client = PlayerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (client == null)
                throw new Exception("Nincs ilyen kliens regisztrálva.");

            client.PlayerIds = playerIds;
        }
    }
}
