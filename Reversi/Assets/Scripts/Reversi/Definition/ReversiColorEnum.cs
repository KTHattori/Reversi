using Unity.VisualScripting;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// 石の色を表す列挙体
    /// </summary>
    public enum DiscType : int
    {
        Empty = 0,
        White = -1,
        Black = 1,
        Wall = 2,
        White_Placeable = -10,
        Black_Placeable = 10,
    }

    public static class DiscColorUtil
    {
        // extension method

        /// <summary>
        /// 反対の石色を取得する。<br/>
        /// 空や、壁の場合はそのまま返す。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static DiscType GetInvertedColor(this DiscType color)
        {
            switch(color)
            {
                case DiscType.White:
                return DiscType.Black;
                case DiscType.Black:
                return DiscType.White;
                default:
                return color;
            }
        }

        /// <summary>
        /// 石色に対応したColorの値を返す。
        /// 空の場合は透明、壁の場合は赤を返す。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(this DiscType color)
        {
            switch(color)
            {
                case DiscType.White:
                return Color.white;
                case DiscType.Black:
                return Color.black;
                case DiscType.Empty:
                return new Color(0.0f,0.0f,0.0f,0.0f);
                case DiscType.Wall:
                return Color.red;
                case DiscType.White_Placeable:
                return new Color(1.0f,1.0f,1.0f,0.5f);
                case DiscType.Black_Placeable:
                return new Color(0.0f,0.0f,0.0f,0.5f);
                default:
                return Color.gray;
            }
        }

        /// <summary>
        /// 石色に対応した配置可能状態を返す。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static DiscType GetPlaceable(this DiscType color)
        {
            switch(color)
            {
                case DiscType.White:
                return DiscType.White_Placeable;
                case DiscType.Black:
                return DiscType.Black_Placeable;
                default:
                return color;
            }
        }
    }
}

