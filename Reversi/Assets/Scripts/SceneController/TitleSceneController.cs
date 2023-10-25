using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : SceneController,ISFConnectable,ISFLoginable
{

    #region // Private variables
    private SmartFox _server;
    #endregion

    #region // Serialized variables
    [SerializeField]
    TitleSceneUI _sceneUI;
    [Tooltip("IP address or domain name of the SmartFoxServer instance")]
	public string host = "127.0.0.1";

	[Tooltip("TCP listening port of the SmartFoxServer instance, used for TCP socket connection in all builds except WebGL")]
	public int tcpPort = 9933;

	[Tooltip("HTTP listening port of the SmartFoxServer instance, used for WebSocket (WS) connections in WebGL build")]
	public int httpPort = 8080;

	[Tooltip("Name of the SmartFoxServer Zone to join")]
	public string zone = "BasicExamples";

	[Tooltip("Display SmartFoxServer client debug messages")]
	public bool debug = false;
    #endregion

    #region // Public variables

    #endregion

    #region // Unity callback methods
    private void Start()
	{
        // ボタンにサーバー接続関数を割り当て
		_sceneUI.PlayOnlineButton.onClick.AddListener(ConnectServer);

        // 接続失敗メッセージがあれば表示
		string connLostMsg = _sfManager.PopConnectionLostMsg();
		if (connLostMsg != null)
			_sceneUI.SetErrorText(connLostMsg);
	}
    #endregion

    #region // Private methods, properties
    private void ConnectServer()
    {
		// UIを無効か
		_sceneUI.Disable();

		// エラーメッセージをクリア
		_sceneUI.SetErrorText("");

		// 接続情報を設定
		ConfigData cfg = new ConfigData();
		cfg.Host = host;
		cfg.Port = tcpPort;
		cfg.Zone = zone;
		cfg.Debug = debug;

#if UNITY_WEBGL
		cfg.Port = httpPort;
#endif

		// クライアントを初期化
#if !UNITY_WEBGL
		_server = _sfManager.CreateSFClient();
#else
		_server = _sfManager.CreateSFClient(UseWebSocket.WS_BIN)
#endif

		// Loggerの設定
		_server.Logger.EnableConsoleTrace = debug;

		// イベント割り当て
		AddSFListeners();

		// SmartFoxServerに接続
		_server.Connect(cfg);
    }
    #endregion

    #region // Interface methods : ISFConnectable
	public override void AddSFListeners()
	{
		_server.AddEventListener(SFSEvent.CONNECTION, OnSFConnection);
		_server.AddEventListener(SFSEvent.CONNECTION_LOST, OnSFConnectionLost);
		_server.AddEventListener(SFSEvent.LOGIN, OnSFLogin);
		_server.AddEventListener(SFSEvent.LOGIN_ERROR, OnSFLoginError);
	}

    public override void RemoveSFListeners()
    {
		if (_server != null)
		{
			_server.RemoveEventListener(SFSEvent.CONNECTION, OnSFConnection);
			_server.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnSFConnectionLost);
			_server.RemoveEventListener(SFSEvent.LOGIN, OnSFLogin);
			_server.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnSFLoginError);
		} 
    }

    // 接続成功
    public void OnSFConnection(BaseEvent evt)
    {
        // 接続できているかチェック
		if ((bool)evt.Params["success"])
		{
			Debug.Log("SFS2X API version: " + _server.Version);
			Debug.Log("Connection mode is: " + _server.ConnectionMode);

			// ログイン
			_server.Send(new LoginRequest(_sceneUI.PlayerNameText));
		}
		else
		{
			// エラー表示
			_sceneUI.SetErrorText("CONNECTION FAILED: Check if the server is running.");

			// UI有効化
			_sceneUI.Enable();
		}
    }

    // 接続失敗
    public void OnSFConnectionLost(BaseEvent evt)
    {
		// イベントリスナーを削除
		RemoveSFListeners();

		// エラー取得、表示
		string reason = (string)evt.Params["reason"];
		
		if (reason != ClientDisconnectionReason.MANUAL)
			_sceneUI.SetErrorText("Connection lost; reason is: " + reason);

		// UI有効化
		_sceneUI.Enable();
    }

    // ログイン成功
    public void OnSFLogin(BaseEvent evt)
    {
		// Load lobby scene
		SceneManager.LoadScene("GameScene");
    }

    // ログイン失敗
    public void OnSFLoginError(BaseEvent evt)
    {
		// 切断する。OnConnectionLostもコールされる。
		_server.Disconnect();

		// エラー表示
		_sceneUI.SetErrorText("Login failed due to the following error:\n" + (string)evt.Params["errorMessage"]);

		// UI有効化
		_sceneUI.Enable();
    }
    #endregion

    #region // Interface methods : IMonoSingletonMethods
    public override void OnInitialize()
    {

    }

    public override void OnFinalize()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region // Protected methods
    #endregion
    
    #region // Public methods, properties
    #endregion


}
