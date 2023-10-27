using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace T0R1.UI
{
    public abstract class SceneUIBase : SealableMonoBehaviour
    {
        // モーダルな要素を格納するリスト
        protected List<IModal> _modalElements = new List<IModal>();

        [SerializeField]
        protected ErrorWindow _errorWindow;

        protected sealed override void Reset()
        {
            OnReset();
        }

        protected sealed override void Awake()
        {
            OnAwake();
        }

        protected sealed override void Start()
        {
            _errorWindow.SetBaseUI(this);
            _errorWindow.Hide();
            MarkAsModal(_errorWindow);

            OnStart();
        }

        protected sealed override void Update()
        {
            // Escapeキーが押されたら、モーダルなUIを非表示にする
            if (Input.GetKeyDown(KeyCode.Escape))
                HideModals();
            OnUpdate();
        }

        protected virtual void OnReset()
        {

        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 有効化する
        /// </summary>
        public abstract void Enable();

        /// <summary>
        /// 無効化する
        /// </summary>
        public abstract void Disable();

        /// <summary>
        /// モーダルなUIを非表示にする
        /// </summary>
        public void HideModals()
        {
            foreach(IModal modal in _modalElements)
            {
                modal.HideModal();
            }
            _errorWindow.Hide();
        }

        public void ShowError(string error)
        {
            _errorWindow.Show(error);
        }

        public void HideError()
        {
            _errorWindow.Hide();
        }

        /// <summary>
        /// 指定のボタンクリック時関数に関数を追加する
        /// ボタンが見つからなければエラーログを流す
        /// </summary>
        /// <param name="target"></param>
        /// <param name="call"></param>
        protected static void TryBindActionToButton(Button target,UnityAction call)
        {
            if(target != null) target.onClick.AddListener(call);
            else Debug.LogError("No button found!");
        }

        /// <summary>
        /// 指定したモーダル要素を保持する
        /// </summary>
        /// <param name="modal"></param>
        protected void MarkAsModal(IModal modal)
        {
            _modalElements.Add(modal);
        }
    }
}
