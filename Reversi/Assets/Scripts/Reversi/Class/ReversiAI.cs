namespace Reversi
{
    public abstract class AI : IPointSelector
    {
        /// <summary>
        /// 事前探索を行う際の先読みする手数
        /// </summary>
        public int presearch_depth;

        /// <summary>
        /// 序盤・中盤の先読み手数
        /// </summary>
        public int normal_depth;

        /// <summary>
        /// 
        /// </summary>
        public int wld_depth;
        public int perfect_depth;

        public abstract void SelectPoint(Point point);

        public AI()
        {

        }
    }
}
