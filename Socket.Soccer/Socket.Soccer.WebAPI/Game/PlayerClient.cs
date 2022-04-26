namespace Socket.Soccer.WebAPI.Game
{
    public class PlayerClient
    {
        public Guid ClientId { get; set; }
        public List<Guid> PlayerIds { get; set; }
        public bool IsHome { get; set; }
    }
}
