using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Reversi
{
    public class ReversiUIManager : MonoBehaviour
    {
        /// <summary>
        /// プレイヤー視点（カメラ）参照
        /// </summary>
        [SerializeField]
        private ObjectReferencer _playerView;
        /// <summary>
        /// UI背景 参照
        /// </summary>
        [SerializeField]
        private ObjectReferencer _backgroundObjRef;

        /// <summary>
        /// リザルト表示 参照
        /// </summary>
        [SerializeField]
        private ObjectReferencer _resultObjRef;

        /// <summary>
        /// ターン表示 参照
        /// </summary>
        [SerializeField]
        private ObjectReferencer _turnObjRef;

        /// <summary>
        /// メッセージ表示 参照
        /// </summary>
        [SerializeField]
        private ObjectReferencer _messageObjRef;

        /// <summary>
        /// パス用のボタン 参照
        /// </summary>
        [SerializeField]
        private ObjectReferencer _passButtonObjRef;

        /// <summary>
        /// 背景 Imageコンポーネント参照
        /// </summary>
        private Image _backgroundImageRef;
        /// <summary>
        /// リザルトコンポーネント参照
        /// </summary>
        private ReversiResultObject _resultCompRef;
        /// <summary>
        /// ターン表示Text参照
        /// </summary>
        private TextMeshProUGUI _turnTextRef;
        /// <summary>
        /// メッセージ表示テキスト参照
        /// </summary>
        private TextMeshProUGUI _messageTextRef;


        /// <summary>
        /// 最初のUpdate直前にコール
        /// 各UIコンポーネントを取得する
        /// </summary>
        public void Initialize()
        {
            // コンポーネントを取得・変数に格納
            ObjectReferencer.GetComponentReference<Image>(_backgroundObjRef,ref _backgroundImageRef);
            ObjectReferencer.GetComponentReference<ReversiResultObject>(_resultObjRef,ref _resultCompRef);
            ObjectReferencer.GetComponentReference<TextMeshProUGUI>(_turnObjRef,ref _turnTextRef);
            ObjectReferencer.GetComponentReference<TextMeshProUGUI>(_messageObjRef,ref _messageTextRef);

            HidePassButton();
            HideResult();
            Deactivate();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            _playerView.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            _playerView.gameObject.SetActive(false);
        }

        /// <summary>
        /// 背景色を設定する
        /// </summary>
        /// <param name="color"></param>
        public void SetBackgroundColor(Color color)
        {
            _backgroundImageRef.color = color;
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
        /// リザルトを表示する
        /// </summary>
        public void ShowResult(in Board board)
        {
            _resultCompRef.Show(board);
        }

        /// <summary>
        /// リザルトを非表示にする
        /// </summary>
        public void HideResult()
        {
            _resultCompRef.Hide();
        }

        /// <summary>
        /// パスボタンを表示
        /// </summary>
        public void ShowPassButton()
        {
            _passButtonObjRef.ActivateObject();
        }

        /// <summary>
        /// パスボタンを非表示
        /// </summary>
        public void HidePassButton()
        {
            _passButtonObjRef.DeactivateObject();
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
