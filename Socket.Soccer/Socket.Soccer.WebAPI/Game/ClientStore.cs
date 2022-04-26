namespace Socket.Soccer.WebAPI.Game
{
    public class ClientStore: IClientStore
    {
        public List<PlayerClient> PlayerClients { get; init; } = new List<PlayerClient>();

        public void Add(Guid playerClientId, List<Guid> playerIds)
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

        public PlayerClient? Get(Guid playerClientId)
        {
            return PlayerClients.FirstOrDefault(x => x.ClientId == playerClientId);
        }

        public bool Check(Guid playerClientId, Guid playerId)
        {
            var client = PlayerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (client == null)
                throw new Exception("Nincs ilyen kliens regisztrálva.");

            return client.PlayerIds.Contains(playerId);
        }

        public void Remove(Guid playerClientId)
        {
            var removable = PlayerClients.FirstOrDefault(x => x.ClientId == playerClientId);
            if (removable != null)
                PlayerClients.Remove(removable);
        }
    }
}
