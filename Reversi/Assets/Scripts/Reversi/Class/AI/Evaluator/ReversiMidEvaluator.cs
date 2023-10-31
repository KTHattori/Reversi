namespace Reversi
{
    /// <summary>
    /// 中盤の評価関数
    /// </summary>
    public class MidEvaluator : IEvaluator
    {
        /// <summary>
        /// 辺の評価に関するパラメータをまとめたクラス
        /// </summary>
        private class EdgeParam
        {
            /// <summary>
            /// 確定石の個数
            /// </summary>
            public byte stable = 0;

            /// <summary>
            /// ウイングの個数
            /// </summary>
            public byte wing = 0;

            /// <summary>
            /// 山の個数
            /// </summary>
            public byte mountain = 0;

            /// <summary>
            /// 危険なC打ちの個数
            /// </summary>
            public byte dangerCMove = 0;

            /// <summary>
            /// 加算
            /// </summary>
            /// <param name="src"></param>
            /// <returns></returns>
            public EdgeParam Add(EdgeParam src)
            {
                stable += src.stable;
                wing += src.wing;
                mountain += src.mountain;
                dangerCMove += src.dangerCMove;

                return this;
            }

            /// <summary>
            /// 代入
            /// </summary>
            /// <param name="src"></param>
            public void Set(EdgeParam src)
            {
                stable = src.stable;
                wing = src.wing;
                mountain = src.mountain;
                dangerCMove = src.dangerCMove;
            }
        }

        /// <summary>
        /// 辺の評価情報を格納するクラス
        /// </summary>
        private class EdgeStat
        {
            /// <summary>
            /// 各石色ごとの辺評価情報
            /// </summary>
            ColoredContainer<EdgeParam> stat = new ColoredContainer<EdgeParam>();

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public EdgeStat()
            {
                stat[DiscColor.Black] = new EdgeParam();
                stat[DiscColor.White] = new EdgeParam();               
                stat[DiscColor.Empty] = new EdgeParam();
            }

            /// <summary>
            /// 各石色のデータに加算
            /// </summary>
            /// <param name="value"></param>
            public void Add(EdgeStat value)
            {
                stat[DiscColor.Black].Add(value.stat[DiscColor.Black]);
                stat[DiscColor.White].Add(value.stat[DiscColor.White]);            
                stat[DiscColor.Empty].Add(value.stat[DiscColor.Empty]);
            }

            /// <summary>
            /// 指定した石色のデータを取得
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            public EdgeParam this[DiscColor color]
            {
                get { return stat[color]; }
            }

            public void _Log()
            {
                UnityEngine.Debug.Log($"[BLACK]stable: {stat[DiscColor.Black].stable}, wing: {stat[DiscColor.Black].wing}, mountain:{stat[DiscColor.Black].mountain}, CMove:{stat[DiscColor.Black].dangerCMove}");
                UnityEngine.Debug.Log($"[White]stable: {stat[DiscColor.White].stable}, wing: {stat[DiscColor.White].wing}, mountain:{stat[DiscColor.White].mountain}, CMove:{stat[DiscColor.White].dangerCMove}");
            }
        }

        /// <summary>
        /// 隅の評価に関するクラス
        /// </summary>
        private class CornerParam
        {
            /// <summary>
            /// 隅にある石の数
            /// </summary>
            public byte corner = 0;
            /// <summary>
            /// 危険なXうちの数
            /// </summary>
            public byte dangerXmove = 0;
        }

        /// <summary>
        /// 隅の評価情報を格納するクラス
        /// </summary>
        private class CornerStat
        {
            /// <summary>
            /// 各色ごとの隅評価情報
            /// </summary>
            ColoredContainer<CornerParam> stat = new ColoredContainer<CornerParam>();

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public CornerStat()
            {
                stat[DiscColor.Black] = new CornerParam();
                stat[DiscColor.White] = new CornerParam();               
                stat[DiscColor.Empty] = new CornerParam();
            }

            /// <summary>
            /// 指定した石色のデータを取得
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            public CornerParam this[DiscColor color]
            {
                get { return stat[color]; }
            }

            public void _Log()
            {
                UnityEngine.Debug.Log($"[BLACK]corner: {stat[DiscColor.Black].corner}, XMove: {stat[DiscColor.Black].dangerXmove}");
                UnityEngine.Debug.Log($"[White]corner: {stat[DiscColor.White].corner}, XMove: {stat[DiscColor.White].dangerXmove}");
            }
        }

        /// <summary>
        /// 評価の重みづけを定義するクラス
        /// </summary>
        private class Weight
        {
            public int mobility_w;
            public int liberty_w;
            public int stable_w;
            public int wing_w;
            public int dangerXmove_w;
            public int dangerCmove_w;
        }

        /// <summary>
        /// 評価の重みづけ
        /// </summary>
        private Weight _evalWeight;
        
        /// <summary>
        /// テーブルのサイズ 3の8乗
        /// </summary>
        private const int Table_Size = 6561;    // 3 ^ 8

        /// <summary>
        /// 静的変数：辺の評価テーブルを格納する
        /// </summary>
        private static EdgeStat[] _edgeTable = new EdgeStat[Table_Size];

        /// <summary>
        /// 静的変数：評価テーブルが初期化されたかどうか
        /// </summary>
        private static bool _tableInitialized = false;

        /// <summary>
        /// 試合中盤で用いられる評価関数のコンストラクタ
        /// </summary>
        public MidEvaluator()
        {
            if(!_tableInitialized)
            {
                // 初回起動時にテーブル作成
                DiscColor[] line = new DiscColor[Constant.BoardSize];
                GenerateEdge(line,0);

                _tableInitialized = true;
            }

            _evalWeight = new Weight
            {
                mobility_w = 67,
                liberty_w = -13,
                stable_w = 101,
                wing_w = -308,
                dangerXmove_w = -449,
                dangerCmove_w = -552
            };
        }

        /// <summary>
        /// 評価を行う。評価値を返す。
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public int Evaluate(in Board board)
        {
            EdgeStat edgeStat = new EdgeStat();
            CornerStat cornerStat = new CornerStat();
            int result;

            // 辺の評価
            edgeStat.Add(_edgeTable[GetIndex(board,Board.DirectionQuad.Top)]);
            edgeStat.Add(_edgeTable[GetIndex(board,Board.DirectionQuad.Bottom)]);
            edgeStat.Add(_edgeTable[GetIndex(board,Board.DirectionQuad.Right)]);
            edgeStat.Add(_edgeTable[GetIndex(board,Board.DirectionQuad.Left)]);


            // 隅の評価
            cornerStat = EvaluateCorner(board);

            // 確定石の補正
            edgeStat[DiscColor.Black].stable -= cornerStat[DiscColor.Black].corner;
            edgeStat[DiscColor.White].stable -= cornerStat[DiscColor.White].corner;

            // パラメータ線形結合
            // もう少し表記を簡略化できそう.
            result =
                  edgeStat[DiscColor.Black].stable * _evalWeight.stable_w
                - edgeStat[DiscColor.White].stable * _evalWeight.stable_w
                + edgeStat[DiscColor.Black].wing * _evalWeight.wing_w
                - edgeStat[DiscColor.White].wing * _evalWeight.wing_w
                + cornerStat[DiscColor.Black].dangerXmove * _evalWeight.dangerXmove_w
                - cornerStat[DiscColor.White].dangerXmove * _evalWeight.dangerXmove_w
                + edgeStat[DiscColor.Black].dangerCMove * _evalWeight.dangerCmove_w
                - edgeStat[DiscColor.White].dangerCMove * _evalWeight.dangerCmove_w
                ;
            
            // 開放度・着手可能手数の評価
            if(_evalWeight.liberty_w != 0)
            {
                ColoredContainer<int> lib = CountLiberty(board);
                result += lib[DiscColor.Black] * _evalWeight.liberty_w;
                result -= lib[DiscColor.White] * _evalWeight.liberty_w;
            }

            result +=
                    (int)board.GetCurrentColor()
                *   board.GetMovablePoints().Count
                *   _evalWeight.mobility_w;

            return (int)board.GetCurrentColor() * result;
        }

        /// <summary>
        /// 辺情報を作成
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="count"></param>
        private void GenerateEdge(DiscColor[] edge,int count)
        {
            if(count == Constant.BoardSize)
            {
                // パターンが完成したので、局面カウント
                EdgeStat stat = new EdgeStat();

                stat[DiscColor.Black].Set(EvaluateEdge(edge,DiscColor.Black));
                stat[DiscColor.White].Set(EvaluateEdge(edge,DiscColor.White));

                _edgeTable[IndexLine(edge)] = stat;

                return;
            }

            // 再帰的に全パターン網羅
            edge[count] = DiscColor.Empty;
            GenerateEdge(edge,count + 1);
            edge[count] = DiscColor.Black;
            GenerateEdge(edge,count + 1);
            edge[count] = DiscColor.White;
            GenerateEdge(edge,count + 1);
            
            return;
        }

        /// <summary>
        /// 辺を評価する
        /// </summary>
        /// <param name="line"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private EdgeParam EvaluateEdge(DiscColor[] line,DiscColor color)
        {
            EdgeParam edgeParam = new EdgeParam();

            int x;

            // ウイングなどのカウント
            if(line[0] == DiscColor.Empty && line[7] == DiscColor.Empty)
            {
                x = 2;
                while(x <= 5)
                {
                    if(line[x] != color) break;
                    x++;
                }

                if(x == 6)  // ブロックができている
                {
                    if(line[1] == color && line[6] == DiscColor.Empty)
                        edgeParam.wing = 1;
                    else if(line[1] == DiscColor.Empty && line[6] == color)
                        edgeParam.wing = 1;
                    else if(line[1] == color && line[6] == color)
                        edgeParam.mountain = 1;
                }
                else
                {
                    if(line[1] == color)
                        edgeParam.dangerCMove++;
                    if(line[6] == color)
                        edgeParam.dangerCMove++;
                }
            }

            // 確定石カウント

            // 左から右に
            for(x = 0;x < 8; x++)
            {
                if(line[x] != color) break;
                edgeParam.stable++;
            }

            if(edgeParam.stable < 8)
            {
                // 右から左に
                for(x = 7;x > 0; x--)
                {
                    if(line[x] != color) break;
                    edgeParam.stable++;
                }
            }

            return edgeParam;
        }

        /// <summary>
        /// 隅を評価する
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private CornerStat EvaluateCorner(in Board board)
        {
            CornerStat cornerStat = new CornerStat();

            cornerStat[DiscColor.Black].corner = 0;
            cornerStat[DiscColor.Black].dangerXmove = 0;
            cornerStat[DiscColor.White].corner = 0;
            cornerStat[DiscColor.White].dangerXmove = 0;

            Point point;

            // 左上
            point = Constant.Corner_TopLeft;
            cornerStat[board.GetColorAt(point)].corner++;
            if(board.GetColorAt(point) == DiscColor.Empty)
            {
                point.x = 2;
                point.y = 2;
                cornerStat[board.GetColorAt(point)].dangerXmove++;
            }

            // 左下
            point = Constant.Corner_BottomLeft;
            cornerStat[board.GetColorAt(point)].corner++;
            if(board.GetColorAt(point) == DiscColor.Empty)
            {
                point.x = 2;
                point.y = 7;
                cornerStat[board.GetColorAt(point)].dangerXmove++;
            }

            // 右上
            point = Constant.Corner_TopRight;
            cornerStat[board.GetColorAt(point)].corner++;
            if(board.GetColorAt(point) == DiscColor.Empty)
            {
                point.x = 7;
                point.y = 2;
                cornerStat[board.GetColorAt(point)].dangerXmove++;
            }

            // 右下
            point = Constant.Corner_BottomRight;
            cornerStat[board.GetColorAt(point)].corner++;
            if(board.GetColorAt(point) == DiscColor.Empty)
            {
                point.x = 7;
                point.y = 7;
                cornerStat[board.GetColorAt(point)].dangerXmove++;
            }

            return cornerStat;
        }

        /// <summary>
        /// 各石色の開放度を取得
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private ColoredContainer<int> CountLiberty(in Board board)
        {
            ColoredContainer<int> liberty = new ColoredContainer<int>();

            liberty[DiscColor.Black] = 0;
            liberty[DiscColor.White] = 0;
            liberty[DiscColor.Empty] = 0;

            Point point = new Point();

            for(int x = 1; x <= Constant.BoardSize; x++)
            {
                point.x = x;
                for(int y = 1; y <= Constant.BoardSize; y++)
                {
                    point.y = y;
                    int lib = liberty[board.GetColorAt(point)];
                    lib += board.GetLibertyAt(point);
                    liberty[board.GetColorAt(point)] = lib;
                }            
            }


            return liberty;
        }

        /// <summary>
        /// 探索方向に応じたIndexを取得する
        /// </summary>
        /// <param name="board"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private int GetIndex(in Board board,Board.DirectionQuad dir)
        {
            int index = 0;
            int m = 1;
            Point point = Point.GetBoardDirQuad(dir);

            if((dir & (Board.DirectionQuad.Top | Board.DirectionQuad.Bottom)) != 0)
            {
                for(int i = Constant.BoardSize;i > 0;i--)
                {
                    point.x = i;
                    index += m * ((int)board.GetColorAt(point) + 1);
                    m *= 3;
                }
            }
            else if((dir & (Board.DirectionQuad.Right | Board.DirectionQuad.Left)) != 0)
            {
                for(int i = Constant.BoardSize;i > 0;i--)
                {
                    point.y = i;
                    index += m * ((int)board.GetColorAt(point) + 1);
                    m *= 3;
                }
            }

            return index;
        }

        
        private int IndexLine(DiscColor[] line)
        {
            return 3 *(3 *(3 *(3 *(3 *(3 *(3 *
                ((int)line[0] + 1)
                +(int)line[1] + 1)
                +(int)line[2] + 1)
                +(int)line[3] + 1)
                +(int)line[4] + 1)
                +(int)line[5] + 1)
                +(int)line[6] + 1)
                +(int)line[7] + 1;
        }
    }
}