using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T0R1.UI
{
    public class ModalWindow : Window, IModal
    {
        protected override void OnAwake()
        {
            Debug.Log(this);
            Debug.Log(_closeButton);
            _isModal = true;
            _closeButton.SetParent(this);
        }

        public void HideModal()
        {
            Hide();
        }
    }

}
