using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using T0R1;
using T0R1.UI;

/// <summary>
/// 
/// </summary>
public class SFClientManager : MonoSingleton<SFClientManager>
{
    #region // Constants
	public static string DEFAULT_ROOM = "LOBBY";
	public static string GAME_ROOMS_GROUP_NAME = "games";
	#endregion

    #region // Private variables
    private SmartFox _sfInstance = null;
    private string _connLostMsg;
    #endregion

    #region // Serialized variables
    [Tooltip("IP address or domain name of the SmartFoxServer instance")]
	public string host = "sfs2x.m-craft.com";

	[Tooltip("TCP listening port of the SmartFoxServer instance, used for TCP socket connection in all builds except WebGL")]
	public int tcpPort = 9933;

	[Tooltip("HTTP listening port of the SmartFoxServer instance, used for WebSocket (WS) connections in WebGL build")]
	public int httpPort = 8080;

	[Tooltip("Name of the SmartFoxServer Zone to join")]
	public string zone = "Reversi";

	[Tooltip("Display SmartFoxServer client debug messages")]
	public bool debug = false;
    #endregion

    #region // Public properties
    public static ConfigData TCPConfigData
    {
        get
        {
            ConfigData cfg = new ConfigData();
            cfg.Host = Instance.host;
            cfg.Port = Instance.tcpPort;
            cfg.Zone = Instance.zone;
            cfg.Debug = Instance.debug;

            return cfg;
        }
    }
    public static ConfigData HTTPConfigData
    {
        get
        {
            ConfigData cfg = new ConfigData();
            cfg.Host = Instance.host;
            cfg.Port = Instance.httpPort;
            cfg.Zone = Instance.zone;
            cfg.Debug = Instance.debug;

            return cfg;
        }
    }
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
            LoadScreen.Show();
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
        if(_sfInstance != null) return _sfInstance;
        return null;
    }
    #endregion

    #region // Unity Callbacks
    void OnApplicationQuit()
    {
        if(_sfInstance != null && _sfInstance.IsConnected) _sfInstance.Disconnect();
    }
    #endregion
}
