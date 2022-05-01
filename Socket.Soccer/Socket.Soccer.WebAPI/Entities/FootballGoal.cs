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
            if (teamType == Team.Home)
            {
                StartPoint = new Position(0, 5);
                EndPoint = new Position(0, 9);

                return footballGoal;
            }

            StartPoint = new Position(25, 5);
            EndPoint = new Position(25, 9);

            return footballGoal;
        }
    }
}
