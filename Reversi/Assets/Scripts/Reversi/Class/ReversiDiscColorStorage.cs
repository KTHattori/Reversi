using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// 色ごとに情報を管理するためのジェネリック クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColoredContainer<T>
    {
        private T[] _data = new T[3];

        // インデクサ定義
        public T this[DiscColor color]
        {
            set { _data[(int)color + 1] = value; }
            get { return _data[(int)color + 1]; }
        }
    }
}

