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
        // ColorStorage discList = new ColorStorage();


        // method
        private void FlipDiscs(in Point point)
        {

        }

        private uint CheckMobility(in Disc disc)
        {
            if (rawBoard[disc.x, disc.y] != DiscColor.Empty) return ((uint)Direction.None);

            int x, y;
            uint dir = (uint)Direction.None;

            //if ((int)rawBoard[disc.x,disc.y-1] == -(int)disc.color)
            //{
            //    x = disc.x;
            //    y = disc.y - 2;
            //    while ((int)rawBoard[x,y] == ())
            //}
            return 0;
        }

        private void InitMovable()
        {

        }


        // --- public ---
        // method

        public void Init()
        {

        }

        public bool Move(in Point point)
        {
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

