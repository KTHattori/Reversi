using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T0R1.UI
{
    public class ModalPanel : Panel, IModal
    {
        protected override void OnAwake()
        {
            _isModal = true;
        }

        public void HideModal()
        {
            Hide();
        }
    }
}
