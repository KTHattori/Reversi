using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// Alpha-Beta法を用いるAIクラス
    /// </summary>
    public class AlphaBetaAI : AI
    {
        /// <summary>
        /// 手と評価値をまとめるクラス
        /// </summary>
        private class MoveEval : Point
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

        // variables

        /// <summary>
        /// 評価関数のインスタンス
        /// </summary>
        private IEvaluator _evaluator;

        /// <summary>
        /// 行動する
        /// </summary>
        /// <param name="board"></param>
        public override void Move(in Board board)
        {
            List<Point> movablePoints = BookManager.Instance.Find(board);

            // 打てる場所がなければパス
            if(movablePoints.Count <= 0)
            {
                Pass();
                return;
            }

            // 打てる場所が一か所だけなら探索を行わず、そこに打つ
            if(movablePoints.Count == 1)
            {
                SelectPoint(movablePoints[0]);
                return;
            }

            int limit;
            _evaluator = new MidEvaluator();
            Sort(board,movablePoints,difficulty.PresearchDepth);  // 事前に手をよさそうな順にソート

            // 必勝読みを始めるかどうか
            if(Constant.MaxTurn - board.GetCurrentTurn() <= difficulty.WLDDepth)
            {
                limit = int.MaxValue;
                if(Constant.MaxTurn - board.GetCurrentTurn() <= difficulty.PerfectDepth)
                    _evaluator = new PerfectEvaluator();
                else
                    _evaluator = new WLDEvaluator();
            }
            else
            {
                limit = difficulty.NormalDepth;
            }

            int eval,eval_max = int.MinValue;
            Point point = null;

            // 最良の結果を探索
            for(int i = 0; i < movablePoints.Count; i++)
            {
                board.Move(movablePoints[i]);
                eval = -CalcAlphaBeta(board,limit - 1,int.MinValue,int.MaxValue);

                board.Undo();

                if(eval > eval_max)
                {
                    eval_max = eval;
                    point = movablePoints[i];
                }
            }

            // 最終決定
            SelectPoint(point);
            Debug.Log($"x:{point.x}, y:{point.y}");
        }

        public override void Pass()
        {
            ReversiBoard3D.Pass();
        }

        // 実際に手を打つ
        public override void SelectPoint(Point point)
        {
            ReversiBoard3D.SelectPoint(point);
        }

        /// <summary>
        /// Alpha-Beta法での探索
        /// </summary>
        /// <param name="board"></param>
        /// <param name="limit"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private int CalcAlphaBeta(in Board board,int limit,int alpha,int beta)
        {
            // 深さ制限に達したら評価値を返す
            if(board.IsGameOver() || limit == 0) return _evaluator.Evaluate(board);

            List<Point> points = board.GetMovablePoints();
            int eval;

            // 打つ場所がなければパス
            if(points.Count == 0)
            {
                board.Pass();
                eval = -CalcAlphaBeta(board,limit,-beta,-alpha);
                board.Undo();
                return eval;
            }

            // 試行
            for(int i = 0;i < points.Count; i++)
            {
                board.Move(points[i]);
                eval = -CalcAlphaBeta(board,limit - 1,-beta,-alpha);
                board.Undo();

                alpha = Mathf.Max(alpha,eval);

                if(alpha >= beta)
                {
                    return alpha;   // Beta刈り
                }
            }
            return alpha;
        }

        /// <summary>
        /// 評価値の高い順に並びかえる
        /// </summary>
        /// <param name="board"></param>
        /// <param name="movablePoints"></param>
        /// <param name="limit"></param>
        private void Sort(in Board board,in List<Point> movablePoints, int limit)
        {
            List<MoveEval> moveEvals = new List<MoveEval>();

            for(int i = 0;i < movablePoints.Count; i++)
            {
                int eval;
                Point point = movablePoints[i];

                board.Move(point);
                eval = -CalcAlphaBeta(board,limit - 1,int.MinValue,int.MaxValue);
                board.Undo();

                MoveEval move = new MoveEval(point.x,point.y,eval);
                moveEvals.Add(move);
            }

            // 評価値の大きい順にソート（選択ソート）
            int begin,current;

            for(begin = 0;begin < moveEvals.Count - 1;begin++)
            {
                for(current = begin + 1;current< moveEvals.Count;current++)
                {
                    MoveEval beg = moveEvals[begin];
                    MoveEval cur = moveEvals[current];

                    if(beg.eval < cur.eval)
                    {
                        // 交換する
                        moveEvals[begin] = cur;
                        moveEvals[current] = beg;
                    }
                }
            }

            // 結果の書き戻し
            movablePoints.Clear();
            for(int i = 0;i < moveEvals.Count;i++)
            {
                movablePoints.Add(moveEvals[i]);
            }

            return;
        }
        
    }
}
