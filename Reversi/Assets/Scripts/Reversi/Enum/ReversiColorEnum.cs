﻿using Unity.VisualScripting;
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
            default:
                return Color.gray;
            }
        }

    }
}

