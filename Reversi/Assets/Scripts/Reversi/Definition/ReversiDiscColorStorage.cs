using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    /// <summary>
    /// 色ごとの石数を保存するためのクラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DiscColorStorage<T>
    {
        private T[] _data = new T[3];

        // インデクサ定義
        public T this[DiscType color]
        {
            set { _data[(int)color + 1] = value; }
            get { return _data[(int)color + 1]; }
        }
    }
}

