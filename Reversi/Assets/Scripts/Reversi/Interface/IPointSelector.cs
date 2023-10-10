using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    public interface IPointSelector
    {
        public abstract void SelectPoint(Point point);
    }

}
