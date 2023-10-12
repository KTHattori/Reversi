namespace Reversi
{
    /// <summary>
    /// リバーシ用AIのベースクラス
    /// </summary>
    public abstract class AI : IPointSelector
    {
        /// <summary>
        /// 事前探索を行う際の先読みする手数
        /// </summary>
        public int presearchDepth = 3;

        /// <summary>
        /// 序盤・中盤の先読み手数
        /// </summary>
        public int normalDepth = 5;

        /// <summary>
        /// 終盤での必勝読みを始める残り手数
        /// </summary>
        public int wldDepth = 15;

        /// <summary>
        /// 終盤において完全読みを始める残り手数
        /// </summary>
        public int perfectDepth = 13;

        /// <summary>
        /// ターン内での行動
        /// </summary>
        /// <param name="board"></param>
        public abstract void Move(in Board board);

        /// <summary>
        /// マスを選択する
        /// </summary>
        /// <param name="point"></param>
        public abstract void SelectPoint(Point point);

    }
}
