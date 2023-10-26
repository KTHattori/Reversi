using UnityEngine;
using TMPro;

namespace T0R1.UI
{
    public class ErrorWindow : ModalWindow
    {
        [SerializeField]
        protected TextMeshProUGUI _errorText;
        public void Show(string content)
        {
            Show();
            _errorText.SetText(content);
        }
    }
}

