using System.Collections;
using System.Threading;
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

        /// <summary>
        /// 思考開始
        /// </summary>
        /// <param name="board"></param>
        public void Think(in Board board,CancellationToken cancelToken,SynchronizationContext mainThread)
        {
            Point point = _ai.Think(board,cancelToken);

            // MonoBehaviourにアクセスするため、メインスレッドから実行
            mainThread.Post(__ => 
            {
                ReversiGameLocal.Instance.SelectPoint(point);
            },null);
        }

        /// <summary>
        /// 思考の結果に基づいて行動する。
        /// </summary>
        /// <param name="board"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public IReversiPlayer.ActionResult Act(in Board board, Point point)
        {
            if(point.Equals(Point.Passed))
            {
                if(board.Pass()) return IReversiPlayer.ActionResult.Passed;
                else return IReversiPlayer.ActionResult.Failed;
            }
            else if(point.Equals(Point.Undone))
            {
                if(board.Undo()) return IReversiPlayer.ActionResult.Undone;
                else return IReversiPlayer.ActionResult.Failed;
            }
            else
            {
                if(board.Move(point)) return IReversiPlayer.ActionResult.Placed;
                else return IReversiPlayer.ActionResult.Failed;
            }
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
