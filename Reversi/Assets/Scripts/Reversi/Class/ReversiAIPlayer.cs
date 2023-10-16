using System.Collections;
using UnityEngine;
namespace Reversi
{
    public class AIPlayer : IReversiPlayer
    {
        /// <summary>
        /// AI
        /// </summary>
        private AI _ai = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AIPlayer()
        {
            _ai = new AlphaBetaAI();
        }

        /// <summary>
        /// 引数付きコンストラクタ。難易度を設定する。
        /// </summary>
        /// <param name="difficultySO"></param>
        public AIPlayer(ReversiAIDifficulty difficultySO)
        {
            _ai = new AlphaBetaAI();
            SetDifficulty(difficultySO);
        }

        public void Think(in Board board)
        {
            Point point = _ai.Think(board);
            ReversiGameManager.Instance.SelectPoint(point);
        }

        public IReversiPlayer.ActionResult Act(in Board board, Point point)
        {
            return IReversiPlayer.ActionResult.Undone;
        }

        /// <summary>
        /// 難易度を設定する
        /// </summary>
        /// <param name="difficultySO"></param>
        public void SetDifficulty(ReversiAIDifficulty difficultySO)
        {
            _ai.SetDifficulty(difficultySO);
        }
    }
}
