using Socket.Soccer.WebAPI.Entities;

namespace Socket.Soccer.WebAPI.Stores
{
    public interface IClientStore
    {
        Task<Team> Add(string playerClientId, List<Guid> playerIds);
        Task Remove(string playerClientId);
        Task<PlayerClient?> Get(string playerClientId);
        Task<bool> Check(string playerClientId, Guid playerId);
        Task<bool> ArePlayersReadyToGame();
        Task AddPlayers(string connectionId, List<Guid> playerIds);
        Task Reset();
    }
}
