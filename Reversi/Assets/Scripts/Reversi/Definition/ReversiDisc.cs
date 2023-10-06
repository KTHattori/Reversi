namespace Reversi
{
    /// <summary>
    /// オセロの石を表すクラス
    /// </summary>
    [System.Serializable]
    public class Disc : Point
    {
        public DiscType discType;

        public Disc() : base(0, 0)
        {
            discType = DiscType.Empty;
        }

        public Disc(int x, int y, DiscType color) : base(x, y)
        {
            this.discType = color;
        }
    }
}