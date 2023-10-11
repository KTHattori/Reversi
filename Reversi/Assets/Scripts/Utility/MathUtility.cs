using UnityEngine;

/// <summary>
/// 計算関数の追加定義など
/// </summary>
public static class MathUtility
{
    /// <summary>
    /// 与えられた数値を任意の数値範囲にマッピングする
    /// </summary>
    /// <param name="value">もとの値</param>
    /// <param name="valueMin">もとの値の範囲最低値</param>
    /// <param name="valueMax">もとの値の範囲最高値</param>
    /// <param name="targetMin">変換先の範囲最低値</param>
    /// <param name="targetMax">変換先の範囲最高値</param>
    /// <returns>マッピングされた値</returns>
    public static float Remap(float value,float valueMin,float valueMax,float targetMin,float targetMax)
    {
        return targetMin + (targetMax - targetMin) * ((value - valueMin) / (valueMax - valueMin));
    }

    /// <summary>
    /// 1から引いた値を返す。必ず 0 ~ 1 の範囲に丸める。
    /// </summary>
    /// <param name="valueMax"></param>
    /// <returns></returns>
    public static float OneMinus(float valueMax)
    {
        return Mathf.Clamp01(1.0f - valueMax);
    }
}
