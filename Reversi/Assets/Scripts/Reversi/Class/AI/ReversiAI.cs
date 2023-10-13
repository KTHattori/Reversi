namespace Reversi
{
    /// <summary>
    /// リバーシ用AIのベースクラス
    /// </summary>
    public abstract class AI
    {
        /// <summary>
        /// 難易度設定
        /// </summary>
        public ReversiAIDifficulty difficulty;

        /// <summary>
        /// 難易度情報ScriptableObjectをセットする
        /// </summary>
        /// <param name="diffSO"></param>
        public void SetDifficulty(ReversiAIDifficulty diffSO)
        {
            difficulty = diffSO;
        }

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

        /// <summary>
        /// パスする
        /// </summary>
        public abstract void Pass();

    }
}
