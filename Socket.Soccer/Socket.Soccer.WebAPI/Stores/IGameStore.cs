using Socket.Soccer.WebAPI.Entities;

namespace Socket.Soccer.WebAPI.Stores
{
    public interface IGameStore
    {
        Task<Entities.Game> GetOrCreateGame();
        Task SaveGame(Entities.Game game);
    }
}
