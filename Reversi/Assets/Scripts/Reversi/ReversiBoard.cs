using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Reversi
{
    [System.Serializable]
    public class Board
    {

        // --- private ---
        // enum
        [System.Flags]
        private enum Direction
        {
            None        = 0,
            Upper       = 1,
            UpperLeft   = 1 << 1,
            Left        = 1 << 2,
            LowerLeft   = 1 << 3,
            Lower       = 1 << 4,
            LowerRight  = 1 << 5,
            Right       = 1 << 6,
            UpperRight  = 1 << 7,
        }

        // variable
        private DiscColor[,] rawBoard = new DiscColor[Constant.BoardSize + 2, Constant.BoardSize + 2];

        private int turn;
        private DiscColor currentColor;

        private List<List<Disc>> updatedDiscList = new List<List<Disc>>();

        [SerializeField]
        private List<Point>[] movablePointList = new List<Point>[Constant.BoardSize + 1];
        private Direction[,,] movableDirection = new Direction[Constant.MaxTurn,Constant.BoardSize+2,Constant.BoardSize+2];
        DiscColorStorage<int> discList = new DiscColorStorage<int>();


        // method

        /// <summary>
        /// point で指定された座標に石をうち、挟み込むことができるすべての石を裏返す。<br/>
        /// 「打った石」と「裏返した石」を updatedDiscList に追加する。
        /// </summary>
        /// <param name="point">石を打つ座標</param>
        private void FlipDiscs(in Point point)
        {
            int x,y;
            Disc operation = new Disc(point.x,point.y,currentColor);

            Direction dir = movableDirection[turn,point.x,point.y];
            
            List<Disc> update = new List<Disc>();

            rawBoard[point.x,point.y] = currentColor;
            update.Add(operation);

            // 上に置ける場合
            if(dir.HasFlag(Direction.Upper))
            {
                y = point.y;
                operation.x = point.x;
                while(rawBoard[point.x,--y] != currentColor)
                {
                    rawBoard[point.x,y] = currentColor;
                    operation.y = y;
                    update.Add(operation);
                }
            }

            // 下に置ける場合
            if(dir.HasFlag(Direction.Lower))
            {
                y = point.y;
                operation.x = point.x;
                while(rawBoard[point.x,++y] != currentColor)
                {
                    rawBoard[point.x,y] = currentColor;
                    operation.y = y;
                    update.Add(operation);
                }
            }

            // 左に置ける場合
            if(dir.HasFlag(Direction.Left))
            {
                x = point.x;
                operation.y = point.y;
                while(rawBoard[--x,point.y] != currentColor)
                {
                    rawBoard[x,point.y] = currentColor;
                    operation.x = x;
                    update.Add(operation);
                }
            }

            // 右に置ける場合
            if(dir.HasFlag(Direction.Right))
            {
                x = point.x;
                operation.y = point.y;
                while(rawBoard[++x,point.y] != currentColor)
                {
                    rawBoard[x,point.y] = currentColor;
                    operation.x = x;
                    update.Add(operation);
                }
            }

            // 右上に置ける場合
            if(dir.HasFlag(Direction.UpperRight))
            {
                x = point.x;
                y = point.y;
                while(rawBoard[++x,--y] != currentColor)
                {
                    rawBoard[x,y] = currentColor;
                    operation.x = x;
                    operation.y = y;
                    update.Add(operation);
                }
            }

            // 左上に置ける場合
            if(dir.HasFlag(Direction.UpperLeft))
            {
                x = point.x;
                y = point.y;
                while(rawBoard[--x,--y] != currentColor)
                {
                    rawBoard[x,y] = currentColor;
                    operation.x = x;
                    operation.y = y;
                    update.Add(operation);
                }
            }

            // 左に置ける場合
            if(dir.HasFlag(Direction.LowerLeft))
            {
                x = point.x;
                y = point.y;
                while(rawBoard[--x,++y] != currentColor)
                {
                    rawBoard[x,y] = currentColor;
                    operation.x = x;
                    operation.y = y;
                    update.Add(operation);
                }
            }

            // 右に置ける場合
            if(dir.HasFlag(Direction.LowerRight))
            {
                x = point.x;
                y = point.y;
                while(rawBoard[++x,++y] != currentColor)
                {
                    rawBoard[x,y] = currentColor;
                    operation.x = x;
                    operation.y = y;
                    update.Add(operation);
                }
            }


            // 石の数を更新、反映
            int discdiff = update.Count;

            discList[currentColor] += discdiff;
            discList[currentColor.Inverted()] -= discdiff - 1;
            discList[DiscColor.Empty]--;

            updatedDiscList.Add(update);
        }

        /// <summary>
        /// disc で指定された座標に disc.color の色の石を打てるかどうか、<br/>
        /// また、どの方向に石を裏返せるかを判定する。<br/>
        /// </summary>
        /// <param name="disc">指定する座標</param>
        /// <returns>石を裏返せる方向にフラグが立った整数値を返す</returns>
        /// <seealso cref="Reversi.Board.Direction"/>
        private Direction CheckMobility(in Disc disc)
        {
            // 既に石が置かれていたら置けない
            // not placeable if a disc is already placed
            if (rawBoard[disc.x, disc.y] != DiscColor.Empty) return Direction.None;

            int x, y;
            Direction dir = Direction.None;

            // 上 - Upper
            if (rawBoard[disc.x,disc.y - 1] == disc.color.Inverted())
            {
                x = disc.x;
                y = disc.y - 2;
                while (rawBoard[x,y] == disc.color.Inverted()) { y--; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.Upper;
            }

            // 下 - Lower
            if (rawBoard[disc.x,disc.y + 1] == disc.color.Inverted())
            {
                x = disc.x;
                y = disc.y + 2;
                while (rawBoard[x,y] == disc.color.Inverted()) { y++; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.Lower;
            }

            // 左 - Left
            if (rawBoard[disc.x - 1, disc.y] == disc.color.Inverted())
            {
                x = disc.x - 2;
                y = disc.y;
                while (rawBoard[x, y] == disc.color.Inverted()) { x--; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.Left;
            }

            // 右 - Right
            if (rawBoard[disc.x + 1, disc.y] == disc.color.Inverted())
            {
                x = disc.x + 2;
                y = disc.y;
                while (rawBoard[x, y] == disc.color.Inverted()) { x++; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.Right;
            }

            // 右上 - UpperRight
            if (rawBoard[disc.x + 1, disc.y - 1] == disc.color.Inverted())
            {
                x = disc.x + 2;
                y = disc.y - 2;
                while (rawBoard[x, y] == disc.color.Inverted()) { x++; y--; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.UpperRight;
            }

            // 左上 - UpperLeft
            if (rawBoard[disc.x - 1, disc.y - 1] == disc.color.Inverted())
            {
                x = disc.x - 2;
                y = disc.y - 2;
                while (rawBoard[x, y] == disc.color.Inverted()) { x--; y--; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.UpperLeft;
            }

            // 左下 - LowerLeft
            if (rawBoard[disc.x - 1, disc.y + 1] == disc.color.Inverted())
            {
                x = disc.x - 2;
                y = disc.y + 2;
                while (rawBoard[x, y] == disc.color.Inverted()) { x--; y++; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.LowerLeft;
            }


            // 右下 - LowerRight
            if (rawBoard[disc.x + 1, disc.y + 1] == disc.color.Inverted())
            {
                x = disc.x + 2;
                y = disc.y + 2;
                while (rawBoard[x, y] == disc.color.Inverted()) { x++; y++; }
                if (rawBoard[x, y] == disc.color) dir |= Direction.LowerRight;
            }

            return dir;
        }

        /// <summary>
        /// 現在の手番における着手可能な手を調べなおす。<br/>
        /// movablePoint, movableDirection を更新する。
        /// </summary>
        private void InitMovable()
        {
            Disc disc = new Disc(0,0,currentColor);
                
            Direction dir;
            movablePointList[turn].Clear();

            for(int x = 1; x <= Constant.BoardSize; x++)
            {
                disc.x = x;
                for(int y = 1; y <= Constant.BoardSize; y++)
                {
                    disc.y = y;

                    dir = CheckMobility(disc);
                    if(dir != Direction.None)
                    {
                        // 配置可能な場所に追加
                        movablePointList[turn].Add(disc);
                    }
                    movableDirection[turn,x,y] = dir;
                }
            }
        }


        // --- public ---
        // method

        public void Init()
        {

        }

        /// <summary>
        /// point で指定された位置に石を打つ。
        /// </summary>
        /// <param name="point">石を打つ位置</param>
        /// <returns>
        /// 処理が成功したらtrue、失敗したらfalseを返す。
        /// </returns>
        public bool Move(in Point point)
        {
            // 石が打てる位置かどうかを判定する
            // 打てない位置ならfalseで処理抜け
            // 座標の値が正しい範囲かどうかもここでチェック
            if(point.x < 0 || point.x >= Constant.BoardSize) return false;
            if(point.y < 0 || point.y >= Constant.BoardSize) return false;
            if(movableDirection[turn,point.x,point.y] == (uint)Direction.None) return false;

            // 石を返す
            FlipDiscs(point);

            // 手番の色や現在の手数などを更新
            turn++;
            currentColor = (DiscColor)System.Enum.ToObject(typeof(DiscColor), -(uint)currentColor);

            // movableDir, movablePointを調べなおす
            InitMovable();

            return true;
        }

        /// <summary>
        /// パスを試み、その結果を返す。
        /// </summary>
        /// <returns>成功すればtrue、失敗したらfalseを返す。</returns>
        public bool Pass()
        {
            // 打つ手があれば、パスはできない
            if(movablePointList[turn].Count != 0) return false;

            // ゲームが終了しているなら、パスはできない
            if(IsGameOver()) return false;
            currentColor = currentColor.Inverted();

            // 空の更新情報を追加
            updatedDiscList.Add(new List<Disc>());

            InitMovable();

            return false;
        }

        /// <summary>
        /// 直前の一手を元に戻すことを試み、その結果を返す。
        /// </summary>
        /// <returns>成功するとtrue、元に戻せない場合はfalseを返す</returns>
        public bool Undo()
        {
            if(turn == 0) return false;

            currentColor = currentColor.Inverted();

            List<Disc> update = updatedDiscList[updatedDiscList.Count - 1];

            if(update.Count == 0)   // 前回がパスの場合
            {
                // movablePointListとmovableDirectionを再構築
                movablePointList[turn].Clear();
                for(int x = 1;x <= Constant.BoardSize;x++)
                {
                    for(int y = 1;y <= Constant.BoardSize;y++)
                    {
                        movableDirection[turn,x,y] = Direction.None;
                    }
                }
            }
            else    // 前回がパスでない
            {
                turn--;

                // 石を元に戻す
                rawBoard[update[0].x,update[0].y] = DiscColor.Empty;
                for(int i = 1; i < update.Count; i++)
                {
                    rawBoard[update[i].x,update[i].y] = currentColor.Inverted();
                }

                // 石数の更新
                int discdiff = update.Count;
                discList[currentColor] -= discdiff;
                discList[currentColor.Inverted()] += discdiff - 1;
                discList[DiscColor.Empty]++;
            }

            // 不要になったupdateを1つ削除
            updatedDiscList.RemoveAt(updatedDiscList.Count - 1);

            return true;
        }

        /// <summary>
        /// ゲームが終了しているかどうかを返す。
        /// </summary>
        /// <returns>終了していればtrue, 終了していなければfalseを返す。</returns>
        public bool IsGameOver()
        {
            // 60手に達していたらゲーム終了
            if(turn == Constant.MaxTurn) return true;

            // 打てる手があればゲーム終了ではない
            if(movablePointList[turn].Count != 0) return false;

            // 現在の手番と逆の色が打てるかどうか調べる
            Disc disc = new Disc();
            disc.color = currentColor.Inverted();
            for(int x = 1; x <= Constant.BoardSize; x++)
            {
                disc.x = x;
                for(int y = 1; y <= Constant.BoardSize; y++)
                {
                    disc.y = y;
                    // 置ける場所が1つでもある場合、ゲーム終了ではない
                    if(CheckMobility(disc) != Direction.None) return false;
                }
            }

            return true;
        }

        public uint CountDisc(DiscColor color)
        {
            return 0;
        }

        public DiscColor GetColor(in Point point)
        {
            return DiscColor.Empty;
        }

        public void GetMovablePoint(out List<Point> movablePointList)
        {
            movablePointList = new List<Point>();
        }

        public List<Disc> GetUpdate()
        {
            return new List<Disc>();
        }

        public DiscColor GetCurrentColor()
        {
            return DiscColor.Empty;
        }

        public uint GetTurns()
        {
            return 0;
        }


    }
}

