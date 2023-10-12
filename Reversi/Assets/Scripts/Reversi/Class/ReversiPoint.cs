using System;

namespace Reversi
{
    /// <summary>
    /// オセロのマス座標を表すクラス
    /// </summary>
    [System.Serializable]
    public class Point
    {
        public int x;
        public int y;

        public Point(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public Point(string coord)
        {
            if(coord == null || coord.Length < 2) throw new ArgumentException("リバーシ座標で指定してください!");
            x = coord[0] - 'a' + 1;
            y = coord[1] - '1' + 1;
        }

        public string ToStrCoord()
        {
            string coord = "";
            coord += (char)('a' + x - 1);
            coord += (char)('1' + y - 1);

            return coord;
        }

        public static Point GetBoardDirQuad(Board.DirectionQuad dirQuad)
        {
            switch(dirQuad)
            {
                case Board.DirectionQuad.Top:
                    return Top;
                case Board.DirectionQuad.Bottom:
                    return Bottom;
                case Board.DirectionQuad.Left:
                    return Left;
                case Board.DirectionQuad.Right:
                    return Right;
                default:
                    return new Point(0,0);
            }
        }

        public static Point Top{ get { return new Point(0,1); } }
        public static Point Bottom{ get {return new Point(0,8);} }
        public static Point Left{ get {return new Point(8,0);} }
        public static Point Right{ get { return new Point(1,0); } }

    }
}

