using UnityEngine;

public static class MathUtility
{
    public static float Remap(float value,float valueMin,float valueMax,float targetMin,float targetMax)
    {
        return targetMin + (targetMax - targetMin) * ((value - valueMin) / (valueMax - valueMin));
    }

    public static float OneMinus(float valueMax)
    {
        return Mathf.Clamp01(1.0f - valueMax);
    }
}
