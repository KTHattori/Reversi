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
        protected CloseButton _closeButton;

        [SerializeField]
        protected UnityEvent _onClose;

        public UnityEvent OnCloseEvent => _onClose;

        protected override void OnAwake()
        {
            _closeButton.SetParent(this);
        }

        protected override void FetchComponents()
        {
            _closeButton = GetComponentInChildren<CloseButton>();
        }

        protected override void OnActivated()
        {
            EnableClose();
        }

        protected override void OnDeactivated()
        {
            DisableClose();
        }

        /// <summary>
        /// 閉じるボタンが押されたときに実行される関数
        /// 登録した関数を実行後、非表示にする
        /// </summary>
        protected void OnCloseClick()
        {
            _onClose.Invoke();
            Hide();
            Debug.Log("Closing");
        }

        public void EnableClose()
        {
            _closeButton.Activate();
        }

        public void DisableClose()
        {
            _closeButton.Deactivate();
        }
    }

}
