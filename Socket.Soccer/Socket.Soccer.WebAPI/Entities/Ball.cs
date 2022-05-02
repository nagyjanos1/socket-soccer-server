namespace Socket.Soccer.WebAPI.Entities
{
    public class Ball
    {
        /// <summary>
        /// Labda pozíciója
        /// </summary>
        public readonly Position BALL_START_POSITION = new Position(12, 8);
        public Position Position { get; set; }

        public Ball()
        {
            Position = new Position(BALL_START_POSITION.X, BALL_START_POSITION.Y);
        }

        public (int x, int y) DetermineNewPosition(Position position, Direction direction, int kickStrength)
        {
            return direction switch
            {
                Direction.North => (position.X, position.Y - position.Direction * kickStrength),
                Direction.Easth => (position.X + position.Direction * kickStrength, position.Y),
                Direction.South => (position.X, position.Y + position.Direction * kickStrength),
                Direction.West => (position.X - position.Direction * kickStrength, position.Y),
                _ => (5, 10),
            };
        }       
    }
}
