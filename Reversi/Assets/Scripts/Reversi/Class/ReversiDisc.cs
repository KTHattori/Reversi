namespace Reversi
{
    /// <summary>
    /// オセロの石を表すクラス
    /// </summary>
    [System.Serializable]
    public class Disc : Point
    {
        /// <summary>
        /// 石色
        /// </summary>
        public DiscColor discColor;

        /// <summary>
        /// コンストラクタ。空マスとして作成。
        /// </summary>
        public Disc() : base(0, 0)
        {
            discColor = DiscColor.Empty;
        }

        /// <summary>
        /// 引数付きコンストラクタ。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public Disc(int x, int y, DiscColor color) : base(x, y)
        {
            this.discColor = color;
        }
    }
}