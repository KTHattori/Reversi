namespace Reversi
{
    /// <summary>
    /// オセロに関係した定数を定義したクラス
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// ボードの1辺サイズ
        /// </summary>
        public static readonly int BoardSize = 8;
        /// <summary>
        /// 最大ターン数（マス目総数）
        /// </summary>
        public static readonly int MaxTurn = 60;

        /// <summary>
        /// 左上隅の座標
        /// </summary>
        public static readonly Point Corner_TopLeft = new Point(1,1);

        /// <summary>
        /// 左下隅の座標
        /// </summary>
        public static readonly Point Corner_BottomLeft = new Point(1,BoardSize);
        
        /// <summary>
        /// 右上隅の座標
        /// </summary>
        public static readonly Point Corner_TopRight = new Point(BoardSize,1);

        /// <summary>
        /// 右下隅の座標
        /// </summary>
        public static readonly Point Corner_BottomRight = new Point(BoardSize,BoardSize);
    }
}
