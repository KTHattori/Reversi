using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace T0R1.UI
{
    public class ButtonTextEdit : ButtonBase
    {
        [SerializeField]
        protected TextMeshProUGUI _textMP;

        public void SetLabel(string text)
        {
            _textMP.SetText(text);
        }
    }
}
