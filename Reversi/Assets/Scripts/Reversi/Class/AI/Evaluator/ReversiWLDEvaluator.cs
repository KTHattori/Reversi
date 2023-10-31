namespace Reversi
{
    /// <summary>
    /// 終盤必勝読み評価関数
    /// </summary>
    public class WLDEvaluator : IEvaluator
    {
        public const int WIN = 1;
        public const int DRAW = 0;
        public const int LOSE = -1;

        /// <summary>
        /// 評価を行う。評価値を返す。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public int Evaluate(in Board board)
        {
            int discdiff = board.GetDiscDiff();

            if(discdiff > 0) return WIN;
            else if(discdiff < 0) return LOSE;
            else return DRAW;
        }
    }
}