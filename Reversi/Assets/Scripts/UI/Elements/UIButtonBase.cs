using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace T0R1.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonBase : Element
    {
        [SerializeField]
        protected Button _button;
        [SerializeField]
        protected UnityEvent _onClick;
        public UnityEvent OnClickEvent => _onClick;
        public Button BaseComponent => _button;

        /// <summary>
        /// ボタンコンポーネントを取得
        /// </summary>
        protected override void OnReset()
        {
            _button = GetComponent<Button>();
        }

        protected override void OnAwake()
        {
            if(_button != null) _button.onClick.AddListener(OnClick);
            else Debug.LogError("There is no button set!");
        }
    
        /// <summary>
        /// ボタンが押されたときに実行される関数
        /// </summary>
        private void OnClick()
        {
            OnClickContent();
            _onClick.Invoke();
        }

        /// <summary>
        /// クリック時関数 クラス内定義
        /// </summary>
        protected virtual void OnClickContent()
        {

        }

        public void AddListenerOnClick(UnityAction call)
        {
            if(_button != null) _button.onClick.AddListener(call);
            else Debug.LogError("There is no button set!");
        }

        public void RemoveListenerOnClick(UnityAction call)
        {
            if(_button != null) _button.onClick.RemoveListener(call);
            else Debug.LogError("There is no button set!");
        }
        
    }
}
