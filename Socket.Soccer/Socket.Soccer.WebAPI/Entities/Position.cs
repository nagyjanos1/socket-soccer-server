namespace Socket.Soccer.WebAPI.Entities
{
    public class Position
    {
        private int _direction = -1;
        public int Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                if (value >= 1)
                    _direction = 1;
                else if (value <= -1)
                    _direction = -1;
                else
                    _direction = 1;
            }
        }
        public double X { get; set; }
        public double Y { get; set; }
        public Position()
        {

        }
        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

}
