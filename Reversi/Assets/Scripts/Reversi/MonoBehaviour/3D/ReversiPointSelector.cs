using UnityEngine;
using Reversi;

public class ReversiPointSelector : MonoBehaviour
{
    Disc _disc;

    public void SetDisc(Disc disc)
    {
        _disc = disc;
    }

    public void SelectPoint()
    {
        if(_disc.discType != DiscType.Empty) return;
        ReversiBoard3D.SelectPoint(_disc);
    }
}
