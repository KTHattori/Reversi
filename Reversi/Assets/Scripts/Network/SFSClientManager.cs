using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using T0R1;

/// <summary>
/// 
/// </summary>
public class SFClientManager : MonoSingleton<SFClientManager>
{
    #region // Private variables
    private SmartFox _sfInstance;
    private string _connLostMsg;
    #endregion

    #region // Public variables
    #endregion

    #region // Private methods, properties
    /// <summary>
    /// 常にサーバーからのイベントを監視する
    /// </summary>
    void Update()
    {
        // sfsイベントキューを処理し続ける
        if(_sfInstance != null)
        {
            _sfInstance.ProcessEvents();
        }
    }

    /// <summary>
    /// 接続が切断されたときに呼ばれる関数
    /// </summary>
    /// <param name="evt">サーバーからのイベント</param>
    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove CONNECTION_LOST listener
        _sfInstance.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        _sfInstance = null;

        // Get disconnection reason
        string connLostReason = (string)evt.Params["reason"];

        Debug.Log("Connection to SmartFoxServer lost; reason is: " + connLostReason);

        if (SceneManager.GetActiveScene().name != "TitleScene")
        {
            if (connLostReason != ClientDisconnectionReason.MANUAL)
            {
                // Save disconnection message, which can be retrieved by the LOGIN scene to display an error message
                _connLostMsg = "エラー：\n";

                if (connLostReason == ClientDisconnectionReason.IDLE)
                    _connLostMsg += "ほうちじょうたいだったためせつぞくがきれました。";
                else if (connLostReason == ClientDisconnectionReason.KICK)
                    _connLostMsg += "KICKされました。";
                else if (connLostReason == ClientDisconnectionReason.BAN)
                    _connLostMsg += "BANされました。";
                else
                    _connLostMsg += "セツダンされました。";
            }

            // Switch to the LOGIN scene
            SceneManager.LoadScene("TitleScene");
        }
    }
    #endregion

    #region // Interface methods : IMonoBehaviourMethods
    public override void OnInitialize()
    {
        DontDestroyOnLoad(this);    // シーン間で破棄されないように
        Application.runInBackground = true;     // バックグラウンドでも処理し続けるように
    }

    public override void OnFinalize()
    {
        if (_sfInstance != null && _sfInstance.IsConnected) // sfsインスタンスがあって、接続状態である
            _sfInstance.Disconnect();  // 切断処理
    }
    #endregion

    #region // Public methods, properties
    /// <summary>
    /// 接続切断メッセージを取得し、元のメッセージは削除
    /// </summary>
    public string PopConnectionLostMsg()
    {
        string m = _connLostMsg;
        _connLostMsg = null;
        return m;
    }

    /// <summary>
    /// SFSクライアントを作成
    /// </summary>
    /// <returns></returns>
    public SmartFox CreateSFClient()
    {
        _sfInstance = new SmartFox();
        _sfInstance.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        return _sfInstance;
    }

    /// <summary>
    /// SFSクライアントを作成(WEBソケット使用)
    /// </summary>
    /// <param name="useWebSocket"></param>
    /// <returns></returns>
    public SmartFox CreateSFClient(UseWebSocket useWebSocket)
    {
        _sfInstance = new SmartFox(useWebSocket);
        _sfInstance.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        return _sfInstance;
    }

    /// <summary>
    /// SFSクライアントを取得
    /// </summary>
    /// <returns></returns>
    public SmartFox GetSFClient()
    {
        return _sfInstance;
    }
    #endregion
}
