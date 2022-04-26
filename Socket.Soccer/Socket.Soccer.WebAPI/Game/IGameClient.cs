namespace Socket.Soccer.WebAPI.Game
{
    public interface IGameClient
    {
        Task ReceiveMessage(GameState state);

        Task ReceiveInitResponse(string message);
    }
}
