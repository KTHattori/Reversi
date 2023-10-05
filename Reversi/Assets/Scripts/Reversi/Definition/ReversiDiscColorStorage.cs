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
        private T[] data = new T[3];

        // インデクサ定義
        public T this[DiscType color]
        {
            set { data[(int)color + 1] = value; }
            get { return data[(int)color + 1]; }
        }
    }
}

