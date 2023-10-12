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

