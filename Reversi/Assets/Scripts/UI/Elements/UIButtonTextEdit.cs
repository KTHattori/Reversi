using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace T0R1.UI
{
    public class ButtonTextEdit : ButtonBase
    {
        [SerializeField]
        protected TextMeshProUGUI _textMP;

        /// <summary>
        /// コンポーネントを取得
        /// </summary>
        protected override void OnReset()
        {
            _textMP = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetLabel(string text)
        {
            _textMP.SetText(text);
        }
    }
}
