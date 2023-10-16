using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

namespace Reversi
{
    /// <summary>
    /// リバーシ用AIのベースクラス
    /// </summary>
    public abstract class AI
    {
        private bool _searchCompleted;
        private Point _selectingPoint;

        public bool SearchCompleted{ get {return _searchCompleted;}}

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
        /// 行動する
        /// </summary>
        /// <param name="board"></param>
        public Point Think(in Board board)
        {
            // マス探索
            return SearchPoint(board);
        }
        

        public IReversiPlayer.ActionResult FinishTurn(in Board board)
        {
            // 見つからなければパス、見つかれば配置
            if(_selectingPoint == null)
            {
                board.Pass();
                return IReversiPlayer.ActionResult.Passed;
            }
            else
            {
                board.Move(_selectingPoint);
                return IReversiPlayer.ActionResult.Placed;
            }
        }

        /// <summary>
        /// 次に置くマスを探索し、結果をPointで返す。
        /// 見つからなければnullで返す。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        protected abstract Point SearchPoint(in Board board);
    }
}
