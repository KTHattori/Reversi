namespace Reversi
{
    public class PerfectEvaluator : IEvaluator
    {
        public int Evaluate(in Board board)
        {
            return board.GetDiscDiff();
        }
    }
}
