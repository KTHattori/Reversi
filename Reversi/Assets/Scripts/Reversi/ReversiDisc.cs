namespace Reversi
{
    [System.Serializable]
    public class Disc : Point
    {
        DiscColor color;

        Disc() : base(0, 0)
        {
            color = DiscColor.Empty;
        }

        Disc(int x, int y, DiscColor color) : base(x, y)
        {
            this.color = color;
        }
    }
}