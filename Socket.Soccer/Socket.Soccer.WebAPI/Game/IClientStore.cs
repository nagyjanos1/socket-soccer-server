namespace Socket.Soccer.WebAPI.Game
{
    public interface IClientStore
    {
        void Add(string playerClientId, List<Guid> playerIds);
        void Remove(string playerClientId);
        PlayerClient? Get(string playerClientId);
        bool Check(string playerClientId, Guid playerId);
        void AddPlayers(string connectionId, List<Guid> playerIds);
    }
}
