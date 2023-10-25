using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T0R1.UI
{
    public class ModalWindow : Window
    {
        void Awake()
        {
            _isModal = true;
            if(_closeButton != null) _closeButton.onClick.AddListener(OnCloseClick);
            else Debug.LogError("There is no close button set!");
        }
    }

}
