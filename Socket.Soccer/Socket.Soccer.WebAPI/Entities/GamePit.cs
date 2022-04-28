namespace Socket.Soccer.WebAPI.Entities
{
    /// <summary>
    /// Pálya,
    /// A labda és a játékos helyzetet fogjuk vizsgálni
    /// Ha nem kell kidobjuk
    /// 
    ///             20
    /// 
    ///     |--------|--------|
    ///     |        |        |
    /// 10  |        |        |
    ///     |        |        |
    ///     |--------|--------|
    /// </summary>
    public class GamePit
    {
        public double LongSide { get; }
        public double ShortSide { get; }

        public GamePit()
        {
            LongSide = 20.0;
            ShortSide = 10.0;
        }
    }
}
