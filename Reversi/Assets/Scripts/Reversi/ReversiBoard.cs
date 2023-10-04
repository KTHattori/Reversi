using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    [System.Serializable]


    public class Board
    {

        // --- private ---
        // enum
        private enum Direction
        {
            None = 0,
            Upper = 1,
            UpperLeft = 2,
            Left = 4,
            LowerLeft = 8,
            Lower = 16,
            LowerRight = 32,
            Right = 64,
            UpperRight = 128,
        }

        // variable
        private DiscColor[,] rawBoard = new DiscColor[Constant.BoardSize + 2, Constant.BoardSize + 2];

        private uint turn;
        private DiscColor currentColor;

        private List<List<Disc>> updatedDiscList = new List<List<Disc>>();

        [SerializeField]
        private List<Point> movablePointList = new List<Point>(Constant.MaxTurn + 1);
        private uint[,,] moveableDirection = new uint[Constant.MaxTurn,Constant.BoardSize+2,Constant.BoardSize+2];
        DiscColorStorage<uint> discList = new DiscColorStorage<uint>();


        // method
        private void FlipDiscs(in Point point)
        {

        }

        /// <summary>
        /// disc で指定された座標に disc.color の色の石を打てるかどうか、<br/>
        /// また、どの方向に石を裏返せるかを判定する。<br/>
        /// </summary>
        /// <param name="disc">指定する座標</param>
        /// <returns>石を裏返せる方向にフラグが立った整数値を返す</returns>
        /// <seealso cref="Reversi.Board.Direction"/>
        private uint CheckMobility(in Disc disc)
        {
            // 既に石が置かれていたら置けない
            // not placeable if a disc is already placed
            if (rawBoard[disc.x, disc.y] != DiscColor.Empty) return ((uint)Direction.None);

            int x, y;
            uint dir = (uint)Direction.None;

            // 上 - Upper
            if ((int)rawBoard[disc.x,disc.y - 1] == -(int)disc.color)
            {
                x = disc.x;
                y = disc.y - 2;
                while ((int)rawBoard[x,y] == -(int)disc.color) { y--; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.Upper;
            }

            // 下 - Lower
            if ((int)rawBoard[disc.x,disc.y + 1] == -(int)disc.color)
            {
                x = disc.x;
                y = disc.y + 2;
                while ((int)rawBoard[x,y] == -(int)disc.color) { y++; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.Lower;
            }

            // 左 - Left
            if ((int)rawBoard[disc.x - 1, disc.y] == -(int)disc.color)
            {
                x = disc.x - 2;
                y = disc.y;
                while ((int)rawBoard[x, y] == -(int)disc.color) { x--; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.Left;
            }

            // 右 - Right
            if ((int)rawBoard[disc.x + 1, disc.y] == -(int)disc.color)
            {
                x = disc.x + 2;
                y = disc.y;
                while ((int)rawBoard[x, y] == -(int)disc.color) { x++; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.Right;
            }

            // 右上 - UpperRight
            if ((int)rawBoard[disc.x + 1, disc.y - 1] == -(int)disc.color)
            {
                x = disc.x + 2;
                y = disc.y - 2;
                while ((int)rawBoard[x, y] == -(int)disc.color) { x++; y--; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.UpperRight;
            }

            // 左上 - UpperLeft
            if ((int)rawBoard[disc.x - 1, disc.y - 1] == -(int)disc.color)
            {
                x = disc.x - 2;
                y = disc.y - 2;
                while ((int)rawBoard[x, y] == -(int)disc.color) { x--; y--; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.UpperLeft;
            }

            // 左下 - LowerLeft
            if ((int)rawBoard[disc.x - 1, disc.y + 1] == -(int)disc.color)
            {
                x = disc.x - 2;
                y = disc.y + 2;
                while ((int)rawBoard[x, y] == -(int)disc.color) { x--; y++; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.LowerLeft;
            }


            // 右下 - LowerRight
            if ((int)rawBoard[disc.x + 1, disc.y + 1] == -(int)disc.color)
            {
                x = disc.x + 2;
                y = disc.y + 2;
                while ((int)rawBoard[x, y] == -(int)disc.color) { x++; y++; }
                if ((int)rawBoard[x, y] == (int)disc.color) dir |= (uint)Direction.LowerRight;
            }

            return dir;
        }

        private void InitMovable()
        {

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
            if(point.x < 0 || point.x >= Constant.BoardSize) return false;
            if(point.y < 0 || point.y >= Constant.BoardSize) return false;
            if(moveableDirection[turn,point.x,point.y] == (uint)Direction.None) return false;

            FlipDiscs(point);

            turn++;
            (uint)currentColor = -(uint)currentColor;

            InitMovable();

            return false;
        }

        public bool Pass()
        {
            return false;
        }

        public bool Undo()
        {
            return false;
        }

        public bool IsGameOver()
        {
            return false;
        }

        public uint CountDisc(DiscColor color)
        {
            return 0;
        }

        public DiscColor GetColor(in Point point)
        {
            return DiscColor.Empty;
        }

        public void GetMovablePoint(out List<Point> moveablePointList)
        {
            moveablePointList = new List<Point>();
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

