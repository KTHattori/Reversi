namespace Reversi
{
    /// <summary>
    /// 終盤完全読み切り用評価関数
    /// </summary>
    public class PerfectEvaluator : IEvaluator
    {
        /// <summary>
        /// 評価を行う。評価値を返す。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public int Evaluate(in Board board)
        {
            return board.GetDiscDiff();
        }
    }
}
