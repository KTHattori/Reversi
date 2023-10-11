using Unity.VisualScripting;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// 石の色を表す列挙体
    /// </summary>
    public enum DiscColor : int
    {
        Empty = 0,
        White = -1,
        Black = 1,
        Wall = 2,
        White_Placeable = -10,
        Black_Placeable = 10,
    }

    /// <summary>
    /// 石色に関する拡張メソッドなどを定義したクラス。
    /// </summary>
    public static class DiscColorUtil
    {
        // extension method

        /// <summary>
        /// 反対の石色を取得する。<br/>
        /// 空や、壁の場合はそのまま返す。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static DiscColor GetInvertedColor(this DiscColor color)
        {
            switch(color)
            {
            case DiscColor.White:
                return DiscColor.Black;
            case DiscColor.Black:
                return DiscColor.White;
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
        public static Color ToColor(this DiscColor color)
        {
            switch(color)
            {
            case DiscColor.White:
                return Color.white;
            case DiscColor.Black:
                return Color.black;
            case DiscColor.Empty:
                return new Color(0.0f,0.0f,0.0f,0.0f);
            case DiscColor.Wall:
                return Color.red;
            case DiscColor.White_Placeable:
                return new Color(1.0f,1.0f,1.0f,0.5f);
            case DiscColor.Black_Placeable:
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
        public static DiscColor GetPlaceable(this DiscColor color)
        {
            switch(color)
            {
            case DiscColor.White:
                return DiscColor.White_Placeable;
            case DiscColor.Black:
                return DiscColor.Black_Placeable;
            default:
                return color;
            }
        }
    }
}

