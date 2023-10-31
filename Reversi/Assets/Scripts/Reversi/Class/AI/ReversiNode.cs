namespace Reversi
{
    [System.Serializable]
    /// <summary>
    /// 探索のに用いる木構造を構成するノードクラス
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 子ノード
        /// </summary>
        public Node child = null;
        /// <summary>
        /// 兄弟ノード
        /// </summary>
        public Node sibling = null;
        /// <summary>
        /// 対象となるマス
        /// </summary>
        public Point point = new Point();
    }
}
