namespace Reversi
{
    /// <summary>
    /// オセロの石を表すクラス
    /// </summary>
    [System.Serializable]
    public class Disc : Point
    {
        public DiscColor discColor;

        public Disc() : base(0, 0)
        {
            discColor = DiscColor.Empty;
        }

        public Disc(int x, int y, DiscColor color) : base(x, y)
        {
            this.discColor = color;
        }
    }
}