namespace Reversi
{
    /// <summary>
    /// リバーシの手を打つことができるクラスがもつインタフェース
    /// </summary>
    public interface IReversiPlayer
    {
        /// <summary>
        /// 自分の手番が来た時の行動内容。
        /// </summary>
        /// <param name="board"></param>
        public abstract void OnTurn(in Board board);
    }
}
