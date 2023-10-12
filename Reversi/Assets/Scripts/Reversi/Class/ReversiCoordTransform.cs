namespace Reversi
{
    /// <summary>
    /// 盤面上での座標変換を行うクラス
    /// </summary>
    public class CoordTransformer
    {
        /// <summary>
        /// 盤面を見る向き
        /// </summary>
        private int _rotation = 0;
        /// <summary>
        /// 反転して盤面を見るどうか
        /// </summary>
        private bool _isMirror = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="first"></param>
        public CoordTransformer(Point first)
        {
            if(first.Equals(new Point("d3")))
            {
                _rotation = 1;
                _isMirror = true;
            }
            else if(first.Equals(new Point("c4")))
            {
                _rotation = 2;
            }
            else if(first.Equals(new Point("e6")))
            {
                _rotation = -1;
                _isMirror = true;
            }
        }

        /// <summary>
        /// f5 を基準点とする座標系への変換
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point Normalize(in Point point)
        {
            Point norm = RotatePoint(point,_rotation);
            if(_isMirror) norm = MirrorPoint(norm);

            return norm;
        }
        
        /// <summary>
        /// f5 を基準点とする座標系からもとの座標系への変換
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point Denormalize(in Point point)
        {
            Point denorm = new Point(point.x,point.y);
            if(_isMirror) denorm = MirrorPoint(denorm);

            denorm = RotatePoint(denorm,-_rotation);

            return denorm;
        }

        /// <summary>
        /// 指定した向きに回転させた座標を得る
        /// </summary>
        /// <param name="original"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private Point RotatePoint(in Point original,int rotation)
        {
            rotation %= 4;
            if(rotation < 0) rotation += 4;

            Point rotated = new Point();

            switch(rotation)
            {
            case 1:
                rotated.x = original.y;
                rotated.y = Constant.BoardSize - original.x + 1;
                break;
            case 2:
                rotated.x = Constant.BoardSize - original.x + 1;
                rotated.y = Constant.BoardSize - original.y + 1;
                break;
            case 3:
                rotated.x = Constant.BoardSize - original.y + 1;
                rotated.y = original.x;
                break;
            case 4:
                rotated.x = Constant.BoardSize - original.y + 1;
                rotated.y = original.x;
                break;
            default:
                rotated.x = original.x;
                rotated.y = original.y;
                break;
            }

            return rotated;
        }

        /// <summary>
        /// 左右反転した盤面での座標を返す
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private Point MirrorPoint(in Point original)
        {
            Point mirrored = new Point();
            mirrored.x = Constant.BoardSize - original.x + 1;
            mirrored.y = original.y;

            return mirrored;
        }
    }
}
