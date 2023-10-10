using UnityEngine;
using Reversi;

public class ReversiPointSelector : MonoBehaviour,IPointSelector
{
    Point _point;

    public void SetPoint(Point point)
    {
        _point = point;
    }

    public void SelectPoint(Point point)
    {
        ReversiBoard3D.SelectPoint(point);
    }

    public void OnPress()
    {
        SelectPoint(_point);
    }
}
