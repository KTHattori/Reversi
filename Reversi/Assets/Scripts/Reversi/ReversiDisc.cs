namespace Reversi
{
    [System.Serializable]
    public class Disc : Point
    {
        public DiscColor color;

        public Disc() : base(0, 0)
        {
            color = DiscColor.Empty;
        }

        public Disc(int x, int y, DiscColor color) : base(x, y)
        {
            this.color = color;
        }
    }
}