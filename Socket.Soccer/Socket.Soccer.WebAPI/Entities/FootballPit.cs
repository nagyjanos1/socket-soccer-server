namespace Socket.Soccer.WebAPI.Entities
{
    /// <summary>
    /// Pálya,
    /// A labda és a játékos helyzetet fogjuk vizsgálni
    /// Ha nem kell kidobjuk
    /// 
    ///             20
    ///    -10               10
    ///    5|--------|--------|
    ///     |        |0       |
    /// 10  |--------|--------|
    ///     |       0|        |
    ///   -5|--------|--------|
    /// </summary>
    public class FootballPit
    {
        public const int WIDTH = 25;
        public const int HEIGHT = 15;

        private readonly object?[,] _tiles = new object[WIDTH, HEIGHT];

        public FootballGoal Home { get; }
        public FootballGoal Away { get; }

        public FootballPit()
        {
            Home = new FootballGoal().Create(Team.Home);
            Away = new FootballGoal().Create(Team.Away);

            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    _tiles[i, j] = null;
                }
            }           
        }

        public void SetTile(Position position, object anything)
        {
            _tiles[position.X, position.Y] = anything;
        }

        internal bool IsOccupied(int x, int y)
        {
            return _tiles[x, y] != null;
        }

        public void ClearTile(Position position)
        {
            _tiles[position.X, position.Y] = null;
        }

        public bool IsBallOut(Ball ball)
        {
            return ball.Position.X >= WIDTH - 1 
                || ball.Position.X < 0
                || ball.Position.Y >= HEIGHT - 1
                || ball.Position.Y < 0;
        }

        public bool IsGoal(Ball ball, out Team? team)
        {
            if (IsGoal(ball, Home))
            {
                team = Team.Home;
                return true;
            }

            if (IsGoal(ball, Away))
            {
                team = Team.Away;
                return true;
            }

            team = null;
            return false;
        }
               
    
        private bool IsGoal(Ball ball, FootballGoal footballGoal) 
            => ball.Position.X == footballGoal.StartPoint.X
            && ball.Position.Y >= footballGoal.StartPoint.Y 
            && ball.Position.Y <= footballGoal.EndPoint.Y;

        
    }
}
