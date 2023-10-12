namespace Reversi
{
    public interface IEvaluator
    {
        public abstract int Evaluate(in Board board);
    }
}
