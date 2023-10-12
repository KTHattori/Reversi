namespace Reversi
{
    public class WLDEvaluator : IEvaluator
    {
        public const int WIN = 1;
        public const int DRAW = 0;
        public const int LOSE = -1;

        public int Evaluate(in Board board)
        {
            int discdiff = board.GetDiscDiff();

            if(discdiff > 0) return WIN;
            else if(discdiff < 0) return LOSE;
            else return DRAW;
        }
    }
}