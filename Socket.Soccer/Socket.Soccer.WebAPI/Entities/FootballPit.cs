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
        private double WidthOffset => LongSide / 2;
        private double HeightOffset => ShortSide / 2;

        public double LongSide { get; }
        public double ShortSide { get; }

        public FootballGoal Home { get; }
        public FootballGoal Away { get; }

        public FootballPit()
        {
            LongSide = 20;
            ShortSide = 10.0;

            Home = new FootballGoal().Create(Team.Home);
            Away = new FootballGoal().Create(Team.Away);
        }

        public bool IsBallOut(Ball ball)
        {
            return ball.Position.X + WidthOffset >= LongSide 
                || ball.Position.X + WidthOffset < 0
                || ball.Position.Y + HeightOffset >= ShortSide
                || ball.Position.Y + HeightOffset < 0;
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
            => ball.Position.X + WidthOffset <= footballGoal.StartPoint.X 
            && ball.Position.Y <= footballGoal.StartPoint.Y 
            && ball.Position.Y >= footballGoal.EndPoint.Y;
    }
}
