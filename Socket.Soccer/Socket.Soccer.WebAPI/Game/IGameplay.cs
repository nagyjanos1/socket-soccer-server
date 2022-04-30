using Socket.Soccer.WebAPI.Entities;

namespace Socket.Soccer.WebAPI.Game
{
    public interface IGameplay
    {
        Task RegisterClient(string clientId);
        Task<Entities.Game> InitPlayers(string clientId, List<Guid> playerIds);
        Task<Entities.Game> HandleClientCommand(PlayerCommand command);
        Task UnregisterClient(string clientId);
    }
}
