namespace Socket.Soccer.WebAPI.Entities
{
    public class PlayerClient
    {
        public string ClientId { get; set; }
        public List<Guid> PlayerIds { get; set; }
        public bool IsHome { get; set; }
    }
}
