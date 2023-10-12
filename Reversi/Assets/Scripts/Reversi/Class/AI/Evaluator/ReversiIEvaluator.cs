namespace Reversi
{
    /// <summary>
    /// 評価関数 共通継承インタフェース
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// 評価関数本体。評価値を返す。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public abstract int Evaluate(in Board board);
    }
}
