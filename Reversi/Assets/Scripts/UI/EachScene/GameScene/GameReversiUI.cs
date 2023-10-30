using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using T0R1.UI;
using TMPro;

namespace Reversi
{
    public class GameReversiUI : ModalPanel
    {
        /// <summary>
        /// ターン表示Text参照
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _turnTextRef;
        /// <summary>
        /// メッセージ表示テキスト参照
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _messageTextRef;

        [SerializeField]
        private ButtonTextEdit _passButton;

        [SerializeField]
        private ButtonTextEdit _undoButton;

        /// <summary>
        /// 最初のUpdate直前にコール
        /// 各UIコンポーネントを取得する
        /// </summary>
        public void Initialize()
        {
            HidePassButton();
            HideUndoButton();
            Deactivate();
        }

        /// <summary>
        /// 有効化
        /// </summary>
        protected override void OnActivated()
        {
            _passButton.Activate();
            _undoButton.Activate();
        }

        /// <summary>
        /// 無効化
        /// </summary>
        protected override void OnDeactivated()
        {
            _passButton.Deactivate();
            _undoButton.Deactivate();
        }

        /// <summary>
        /// ターン数を設定する
        /// </summary>
        /// <param name="turn"></param>
        public void SetTurnNumber(int turnNum)
        {
            _turnTextRef.SetText(turnNum.ToString());
        }

        /// <summary>
        /// 画面下部に指定したメッセージを表示する
        /// </summary>
        /// <param name="msg">メッセージ内容</param>
        public void SetMessageText(string msg)
        {
            _messageTextRef.SetText(msg);
        }

        /// <summary>
        /// パスボタンを表示
        /// </summary>
        public void ShowPassButton()
        {
            _passButton.Show();
        }

        /// <summary>
        /// パスボタンを非表示
        /// </summary>
        public void HidePassButton()
        {
            _passButton.Hide();
        }

        /// <summary>
        /// やりなおしボタンを表示
        /// </summary>
        public void ShowUndoButton()
        {
            _undoButton.Show();
        }

        /// <summary>
        /// やりなおしボタンを非表示
        /// </summary>
        public void HideUndoButton()
        {
            _undoButton.Hide();
        }

        /// <summary>
        /// 回転を設定
        /// </summary>
        /// <param name="angle"></param>
        public void SetRotation(float angle)
        {
            Vector3 ang = transform.localEulerAngles;
            ang.z = angle;
            transform.localEulerAngles = ang;
        }
    }
}