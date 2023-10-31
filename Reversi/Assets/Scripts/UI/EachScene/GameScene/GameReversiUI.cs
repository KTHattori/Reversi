using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using T0R1.UI;
using TMPro;
using UnityEngine.UI;
using Interpolation;

namespace Reversi
{
    public class GameReversiUI : ModalPanel
    {
        /// <summary>
        /// ターン表示Text参照
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _turnTextRef;
        [SerializeField]
        private TextMeshProUGUI _timeTextRef;
        /// <summary>
        /// メッセージ表示テキスト参照
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _messageTextRef;
        [SerializeField]
        private TextMeshProUGUI _clientSideText;

        [SerializeField]
        private Image _backgroundImg;
        [SerializeField]
        private Image _clientSideImg;


        [SerializeField]
        private ButtonTextEdit _menuButton;

        [SerializeField]
        private ButtonTextEdit _passButton;

        [SerializeField]
        private ButtonTextEdit _undoButton;

        [SerializeField]
        private Color _blackSideColor = Color.black;

        [SerializeField]
        private Color _whiteSideColor = Color.white;
        
        public ButtonTextEdit MenuButton { get => _menuButton; }
        public ButtonTextEdit PassButton { get => _passButton; }
        public ButtonTextEdit UndoButton { get => _undoButton; }

        private DiscColor _currentBGColorState = DiscColor.Black;
        private Color currentColor = Color.black;

        private float _colorChangeCount = 0.0f;

        /// <summary>
        /// 最初のUpdate直前にコール
        /// 各UIコンポーネントを取得する
        /// </summary>
        public void Initialize()
        {
            HidePassButton();
            HideUndoButton();
            Deactivate();
            currentColor = _blackSideColor;
        }

        protected override void OnUpdate()
        {
            if(_colorChangeCount > 1.0f) return;

            if(_currentBGColorState == DiscColor.Black)
            {
                _backgroundImg.color = Easing.Ease(currentColor,_blackSideColor,_colorChangeCount,1.0f,Easing.Curve.EaseOutCirc);
            }
            else
            {
                _backgroundImg.color = Easing.Ease(currentColor,_whiteSideColor,_colorChangeCount,1.0f,Easing.Curve.EaseOutCirc);
            }

            _colorChangeCount += Time.deltaTime;
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
        /// 制限時間カウントを更新
        /// </summary>
        /// <param name="count"></param>
        public void SetTimeCount(int count)
        {
            _timeTextRef.SetText(count.ToString());
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
        /// 背景色
        /// </summary>
        /// <param name="color"></param>
        public void TurnBackgroundColor(DiscColor color)
        {
            _currentBGColorState = color;
            _colorChangeCount = 1.0f - Mathf.Clamp01(_colorChangeCount);
            currentColor = _backgroundImg.color;
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

        public void SetClientSide(DiscColor color)
        {
            if(color == DiscColor.Black)
            {
                _clientSideImg.color = _blackSideColor;
                _clientSideText.color = _whiteSideColor;
                _clientSideText.SetText("YOUR SIDE: BLACK");
            }
            else
            {
                _clientSideImg.color = _whiteSideColor;
                _clientSideText.color = _blackSideColor;
                _clientSideText.SetText("YOUR SIDE: WHITE");
            }
        }
    }
}