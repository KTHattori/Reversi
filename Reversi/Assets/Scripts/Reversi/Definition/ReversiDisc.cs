namespace Reversi
{
    /// <summary>
    /// オセロの石を表すクラス
    /// </summary>
    [System.Serializable]
    public class Disc : Point
    {
        public DiscType discColor;

        public Disc() : base(0, 0)
        {
            discColor = DiscType.Empty;
        }

        public Disc(int x, int y, DiscType color) : base(x, y)
        {
            this.discColor = color;
        }
    }
}