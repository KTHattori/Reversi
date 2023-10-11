﻿namespace Reversi
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
    }
}
