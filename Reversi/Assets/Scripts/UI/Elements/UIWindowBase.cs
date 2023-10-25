using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace T0R1.UI
{
    public class Window : Panel
    {
        [SerializeField]
        protected Button _closeButton;

        [SerializeField]
        protected UnityEvent _onClose;

        public UnityEvent OnCloseEvent => _onClose;

        void Awake()
        {
            if(_closeButton != null) _closeButton.onClick.AddListener(OnCloseClick);
            else Debug.LogError("There is no close button set!");
        }

        /// <summary>
        /// 閉じるボタンが押されたときに実行される関数
        /// 登録した関数を実行後、非表示にする
        /// </summary>
        public void OnCloseClick()
        {
            _onClose.Invoke();
            Hide();
            Debug.Log("Closing");
        }
    }

}
