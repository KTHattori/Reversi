using UnityEngine;
using Reversi;

/// <summary>
/// リバーシのマス選択用オブジェクト
/// </summary>
public class ReversiPointSelector : MonoBehaviour,IPointSelector
{
    /// <summary>
    /// 割り当てられたマス座標
    /// </summary>
    Point _point;

    /// <summary>
    /// マス座標を代入
    /// </summary>
    /// <param name="point"></param>
    public void SetPoint(Point point)
    {
        _point = point;
    }

    /// <summary>
    /// このマスを選ぶ
    /// </summary>
    /// <param name="point"></param>
    public void SelectPoint(Point point)
    {
        ReversiBoard3D.SelectPoint(point);
    }

    /// <summary>
    /// このオブジェクトが押されたときにコールされる関数
    /// </summary>
    public void OnPress()
    {
        SelectPoint(_point);
    }
}
