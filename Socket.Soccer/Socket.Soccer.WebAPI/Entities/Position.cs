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
        public int X { get; set; }
        public int Y { get; set; }
        public Position()
        {

        }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position(int x, int y, int direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }
    }

}
