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

        public Point(Point point)
        {
            this.x = point.x;
            this.y = point.y;
        }

        public Point(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 文字列によるマス指定
        /// </summary>
        /// <param name="coord"></param>
        /// <exception cref="ArgumentException"></exception>
        public Point(string coord)
        {
            if(coord == null || coord.Length < 2) throw new ArgumentException("リバーシ座標で指定してください!");
            x = coord[0] - 'a' + 1;
            y = coord[1] - '1' + 1;
        }

        /// <summary>
        /// 等しいかどうかを返す
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Equals(Point point)
        {
            if(this.x != point.x) return false;
            if(this.y != point.y) return false;
            return true;
        }

        /// <summary>
        /// 文字列でのマス表記に変換する
        /// </summary>
        /// <returns></returns>
        public string ToStrCoord()
        {
            string coord = "";
            coord += (char)('a' + x - 1);
            coord += (char)('1' + y - 1);

            return coord;
        }

        /// <summary>
        /// ボード上での方向座標を返す
        /// </summary>
        /// <param name="dirQuad"></param>
        /// <returns></returns>
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

        public void _Log(string prefix = "",string suffix = "")
        {
            UnityEngine.Debug.Log($"{prefix}x = {x}, y = {y} ({ToStrCoord()}){suffix}");
        }

        public static Point Top{ get { return new Point(0,1); } }
        public static Point Bottom{ get {return new Point(0,8);} }
        public static Point Left{ get {return new Point(8,0);} }
        public static Point Right{ get { return new Point(1,0); } }

        /// <summary>
        /// Undoしたとして扱うPoint
        /// </summary>
        public static Point Undone{ get { return new Point(-99,-99); }}

        /// <summary>
        /// Passしたとして扱うPoint
        /// </summary>
        public static Point Passed{ get { return new Point(-1,-1); }}

    }
}

