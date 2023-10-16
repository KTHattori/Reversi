using System.Collections;

namespace Reversi
{
    /// <summary>
    /// リバーシの手を打つことができるクラスがもつインタフェース
    /// </summary>
    public interface IReversiPlayer
    {
        /// <summary>
        /// 行動の結果を表す列挙体
        /// </summary>
        public enum ActionResult
        {
            Placed = 0,
            Undone = 1,
            Passed = 2
        }

        /// <summary>
        /// 手番が来た時の思考内容。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public abstract void Think(in Board board);

        /// <summary>
        /// 自分の手番が来た時の行動内容。
        /// </summary>
        /// <param name="board"></param>
        public abstract ActionResult Act(in Board board,Point point);
    }
}
