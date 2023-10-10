using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    public interface IPointSelector
    {
        /// <summary>
        /// マスを選択する
        /// </summary>
        /// <param name="point"></param>
        public abstract void SelectPoint(Point point);
    }

}
