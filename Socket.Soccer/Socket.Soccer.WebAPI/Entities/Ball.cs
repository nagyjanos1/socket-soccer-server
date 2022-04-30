namespace Socket.Soccer.WebAPI.Entities
{
    public class Ball
    {
        /// <summary>
        /// Labda pozíciója
        /// </summary>
        public Position Position { get; set; }

        public Ball()
        {
            Position = new Position
            {
                X = 0,
                Y = 0
            };
        }

        /// <summary>
        /// A játékos iránya (merre néz) mondja meg mi lesz a következő helyzete a labdának
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="kickStrength"></param>
        internal void SetNewPosition(Direction direction, int kickStrength)
        {
            var (x, y) = DeterminePosition(Position, direction, kickStrength);

            Position.X = x;
            Position.Y = y;

        }

        private static (double x, double y) DeterminePosition(Position position, Direction direction, int kickStrength)
        {
            return direction switch
            {
                Direction.North => (position.X, position.Y + position.Direction * kickStrength),
                Direction.Easth => (position.X + position.Direction * kickStrength, position.Y),
                Direction.South => (position.X, position.Y + position.Direction * kickStrength),
                Direction.West => (position.X + position.Direction * kickStrength, position.Y),
                _ => (5, 10),
            };
        }
    }
}
