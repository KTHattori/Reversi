using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    [System.Serializable]

    public class Board
    {
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

        private DiscColor[,] rawBoard = new DiscColor[Constant.BoardSize + 2, Constant.BoardSize + 2];
        private int turn;
        private DiscColor currentColor;

        [SerializeField]
        private List<List<Disc>> updatedDiscList = new List<List<Disc>>();
        
        private List<Point> movablePointList = new List<Point>(Constant.MaxTurn + 1);
        private int[,,] moveableDirection = new int[Constant.MaxTurn,Constant.BoardSize+2,Constant.BoardSize+2];

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

        public int CountDisc(DiscColor color)
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

        DiscColor GetCurrentColor()
        {
            return DiscColor.Empty;
        }

        int GetTurns()
        {
            return 0;
        }


    }
}

