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
        private Random random = new Random();
        public int KickStrength
        {
            get
            {
                return random.Next(1, 3);
            }
        }

        public int StepSize 
        {
            get
            {
                return random.Next(1, 2);
            }
        }

        public bool CanKickTheBall(Ball ball)
        {
            var xDiff = Math.Abs(Position.X - ball.Position.X);
            var yDiff = Math.Abs(Position.Y - ball.Position.Y);
            return xDiff <= 1 && yDiff <= 1;
        }
    }

}
