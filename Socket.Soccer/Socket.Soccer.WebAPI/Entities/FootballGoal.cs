namespace Socket.Soccer.WebAPI.Entities
{
    public class FootballGoal
    {
        public int Length { get; }
        public Position StartPoint { get; private set; }
        public Position EndPoint { get; private set; }
        public FootballGoal() => Length = 4;

        public FootballGoal Create(Team teamType)
        {
            var footballGoal = this;
            var x = teamType == Team.Home ? 0 : FootballPit.WIDTH - 1;
            StartPoint = new Position(x, 5);
            EndPoint = new Position(x, 9);
            return footballGoal;
        }
    }
}
