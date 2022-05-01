namespace Socket.Soccer.WebAPI.Entities
{
    /// <summary>
    /// Játékos
    /// </summary>
    public class Player
    {
        public Guid Id { get; set; }
        public Position Position { get; set; }
        public Team Team { get; set; }
        public int KickStrength
        {
            get
            {
                // Ezt lehetne randomizálni
                return 2;
            }
        }

        public bool IsBallHere(Ball ball)
        {
            return ball.Position.X == Position.X && ball.Position.Y == Position.Y;
        }
    }

}
