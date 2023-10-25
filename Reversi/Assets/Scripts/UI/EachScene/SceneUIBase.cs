using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace T0R1.UI
{
    public abstract class SceneUIBase : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            // Escapeキーが押されたら、モーダルなUIを非表示にする
            if (Input.GetKeyDown(KeyCode.Escape))
                HideModals();
        }

        /// <summary>
        /// モーダルなUIを非表示にする
        /// </summary>
        public abstract void HideModals();

        /// <summary>
        /// 有効化する
        /// </summary>
        public abstract void Enable();

        /// <summary>
        /// 無効化する
        /// </summary>
        public abstract void Disable();

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
    }
}
