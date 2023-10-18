using System.Collections.Generic;
using System.Threading;

namespace Reversi
{
    /// <summary>
    /// リバーシ用AIのベースクラス
    /// </summary>
    public abstract class AI
    {
        /// <summary>
        /// 手と評価値をまとめるクラス
        /// </summary>
        protected class MoveEval : Point
        {
            /// <summary>
            /// 評価値
            /// </summary>
            public int eval = 0;

            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public MoveEval() : base(0, 0)
            {
                eval = 0;
            }

            /// <summary>
            /// 引数付きコンストラクタ
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="e"></param>
            public MoveEval(int x, int y, int e) : base(x,y)
            {
                eval = e;
            }
        }

        /// <summary>
        /// 手の評価値を保存するリスト
        /// </summary>
        protected List<MoveEval> _evaluatedScores = new List<MoveEval>();

        /// <summary>
        /// 難易度設定
        /// </summary>
        public ReversiAIDifficulty difficulty;

        /// <summary>
        /// 探索キャンセル監視用トークン
        /// </summary>
        protected CancellationToken searchCancelToken;

        /// <summary>
        /// プロパティ探索がキャンセルされたかどうか
        /// </summary>
        protected bool IsSearchCancelled
        {
            get { return  searchCancelToken.IsCancellationRequested; }
        }

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
        public Point Think(Board board,CancellationToken cancelToken)
        {
            // マス探索
            searchCancelToken = cancelToken;
            return SearchPoint(board);
        }
        
        public void RelayEvalScores()
        {
            foreach(MoveEval ev in _evaluatedScores)
            {
                ReversiGameManager.Instance.DisplayEvalScore(new Point(ev.x,ev.y),ev.eval);
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
