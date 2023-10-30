using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
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
        protected Image _image;
        [SerializeField]
        protected UnityEvent _onClick;
        public UnityEvent OnClickEvent => _onClick;
        public Button BaseComponent => _button;

        /// <summary>
        /// コンポーネントを取得
        /// </summary>
        protected override void FetchComponents()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        protected override void OnReset()
        {
            
        }

        protected override void OnActivated()
        {
            _button.interactable = true;
        }

        protected override void OnDeactivated()
        {
            _button.interactable = false;
        }

        protected override void OnAwake()
        {
            if(_button != null) _button.onClick.AddListener(OnClick);
            else Debug.LogError("There is no button set!");
        }

        public void SetColor(Color color)
        {
            _image.color = color;
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

        public void RemoveAllListenerOnClick()
        {
            if(_button != null) _button.onClick.RemoveAllListeners();
            else Debug.LogError("There is no button set!");
        }
        
    }
}
