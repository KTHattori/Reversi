using Unity.Android.Gradle.Manifest;

namespace Reversi
{
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

            public EdgeParam Add(EdgeParam src)
            {
                stable += src.stable;
                wing += src.wing;
                mountain += src.mountain;
                dangerCMove += src.dangerCMove;

                return this;
            }

            public void Set(EdgeParam src)
            {
                stable = src.stable;
                wing = src.wing;
                mountain = src.mountain;
                dangerCMove = src.dangerCMove;
            }
        }

        private class EdgeStat
        {
            ColoredContainer<EdgeParam> stat = new ColoredContainer<EdgeParam>();

            public EdgeStat()
            {
                stat[DiscColor.Black] = new EdgeParam();
                stat[DiscColor.White] = new EdgeParam();               
                stat[DiscColor.Empty] = new EdgeParam();
            }

            public void Add(EdgeStat value)
            {
                stat[DiscColor.Black].Add(value.stat[DiscColor.Black]);
                stat[DiscColor.White].Add(value.stat[DiscColor.White]);            
                stat[DiscColor.Empty].Add(value.stat[DiscColor.Empty]);
            }

            public EdgeParam this[DiscColor color]
            {
                get { return stat[color]; }
            }
        }

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

        private class CornerStat
        {
            ColoredContainer<CornerParam> stat = new ColoredContainer<CornerParam>();

            public CornerStat()
            {
                stat[DiscColor.Black] = new CornerParam();
                stat[DiscColor.White] = new CornerParam();               
                stat[DiscColor.Empty] = new CornerParam();
            }

            public CornerParam this[DiscColor color]
            {
                get { return stat[color]; }
            }
        }

        private class Weight
        {
            public int mobility_w;
            public int liberty_w;
            public int stable_w;
            public int wing_w;
            public int dangerXmove_w;
            public int dangerCmove_w;
        }

        private Weight evalWeight;
        
        private const int Table_Size = 6561;    // 3^8
        private static EdgeStat[] edgeTable = new EdgeStat[Table_Size];
        private static bool tableInitialized = false;

        public MidEvaluator()
        {
            if(!tableInitialized)
            {
                // 初回起動時にテーブル作成
                DiscColor[] line = new DiscColor[Constant.BoardSize];
                GenerateEdge(line,0);

                tableInitialized = true;
            }

            evalWeight = new Weight();
            evalWeight.mobility_w = 67;
            evalWeight.liberty_w = -13;
            evalWeight.stable_w = 101;
            evalWeight.wing_w = -308;
            evalWeight.dangerXmove_w = -449;
            evalWeight.dangerCmove_w = -552;
        }

        public int Evaluate(in Board board)
        {
            EdgeStat edgeStat;
            CornerStat cornerStat;
            int result;

            // 辺の評価
            edgeStat = edgeTable[GetIndex(board,Board.DirectionQuad.Top)];
            edgeStat.Add(edgeTable[GetIndex(board,Board.DirectionQuad.Bottom)]);
            edgeStat.Add(edgeTable[GetIndex(board,Board.DirectionQuad.Right)]);
            edgeStat.Add(edgeTable[GetIndex(board,Board.DirectionQuad.Left)]);

            // 隅の評価
            cornerStat = EvaluateCorner(board);

            // 確定石の補正
            edgeStat[DiscColor.Black].stable -= cornerStat[DiscColor.Black].corner;
            edgeStat[DiscColor.White].stable -= cornerStat[DiscColor.White].corner;

            // パラメータ線形結合
            // もう少し表記を簡略化できそう.
            result =
                  edgeStat[DiscColor.Black].stable * evalWeight.stable_w
                - edgeStat[DiscColor.White].stable * evalWeight.stable_w
                + edgeStat[DiscColor.Black].wing * evalWeight.wing_w
                - edgeStat[DiscColor.White].wing * evalWeight.wing_w
                + cornerStat[DiscColor.Black].dangerXmove * evalWeight.dangerXmove_w
                - cornerStat[DiscColor.White].dangerXmove * evalWeight.dangerXmove_w
                + edgeStat[DiscColor.Black].dangerCMove * evalWeight.dangerCmove_w
                - edgeStat[DiscColor.White].dangerCMove * evalWeight.dangerCmove_w
                ;
            
            // 開放度・着手可能手数の評価
            if(evalWeight.liberty_w != 0)
            {
                ColoredContainer<int> lib = CountLiberty(board);
                result += lib[DiscColor.Black] * evalWeight.liberty_w;
                result -= lib[DiscColor.White] * evalWeight.liberty_w;
            }

            result +=
                    (int)board.GetCurrentColor()
                *   board.GetMovablePoints().Count
                *   evalWeight.mobility_w;

            return (int)board.GetCurrentColor() * result;
        }

        private void GenerateEdge(DiscColor[] edge,int count)
        {
            if(count == Constant.BoardSize)
            {
                // パターンが完成したので、局面カウント
                EdgeStat stat = new EdgeStat();

                stat[DiscColor.Black].Set(EvaluateEdge(edge,DiscColor.Black));
                stat[DiscColor.White].Set(EvaluateEdge(edge,DiscColor.White));

                edgeTable[IndexLine(edge)] = stat;

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

        private CornerStat EvaluateCorner(in Board board)
        {
            CornerStat cornerStat = new CornerStat();

            cornerStat[DiscColor.Black].corner = 0;
            cornerStat[DiscColor.Black].dangerXmove = 0;
            cornerStat[DiscColor.White].corner = 0;
            cornerStat[DiscColor.White].dangerXmove = 0;

            Point point = new Point();

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
                    liberty[board.GetColorAt(point)] = 1;
                }            
            }


            return liberty;
        }

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
            return 3 *(3 *(3 *(3 *(3 *(3 *(3 *((int)line[0] + 1)+(int)line[1] + 1)+(int)line[2] + 1)+(int)line[3] + 1)+(int)line[4] + 1)+(int)line[5] + 1)+(int)line[6] + 1)+(int)line[7] + 1;
        }
    }
}