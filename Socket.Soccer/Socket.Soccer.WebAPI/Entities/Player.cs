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

        public int StepSize 
        {
            get
            {
                // Ezt lehetne randomizálni
                return 1;
            }
        }

        public bool CanKickTheBall(Ball ball)
        {
            var xDiff = Math.Abs(Position.X - ball.Position.X);
            var yDiff = Math.Abs(Position.Y - ball.Position.Y);
            //return xDiff == 1 && yDiff == 0 || xDiff == 0 && yDiff == 1;
            return xDiff == 1 || yDiff == 1;
        }
    }

}
