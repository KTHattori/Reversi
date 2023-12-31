using System.Collections.Generic;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Match;
using Sfs2X.Requests;
using Sfs2X.Requests.Game;
using Sfs2X.Util;
using Sfs2X.Entities.Variables;



public class TitleSceneController : SceneController,ISFConnectable,ISFRoomCreatable,ISFRoomJoinable,ISFRoomAccessWatcher,ISFUserVariableWatcher
{
    #region // Private variables
	private static readonly int VAR_TURN_INIT = -1;
    private SmartFox _server;
	private string _manualDCReason = "";
	private bool _lobbyConnected = false;
    #endregion

    #region // Serialized variables
    [SerializeField]
    TitleSceneUI _sceneUI;
    #endregion

    #region // Public properties
	public string UserName { get; set; }
    #endregion

    #region // Unity callback methods
    protected override void OnStart()
	{
        // ボタンに関数を割り当て
		_sceneUI.PlayLocalButton.onClick.AddListener(OnSelectedLocal);
		_sceneUI.PlayOnlineButton.onClick.AddListener(OnSelectedOnline);

		// オンラインプレイウィンドウが閉じたときに切断する関数を割り当て
		_sceneUI.OnlinePlayWindow.AddActionOnHidden(OnCloseOnlineWindow);

		// 検索開始関数を割り当て
		_sceneUI.OnlinePlayWindow.MatchMakingButton.AddListenerOnClick(StartMatchMaking);

        // 接続失敗メッセージがあれば表示
		string connLostMsg = _sfManager.PopConnectionLostMsg();
		if (connLostMsg != null)
			_sceneUI.SetErrorText(connLostMsg);

		UserName = $"Player#{Mathf.RoundToInt(System.DateTime.Now.Millisecond)}";
		_sceneUI.SetPlayerNameText(UserName);
		_sceneUI.SettingWindow.NameInputField.SetTextWithoutNotify(UserName);

		_manualDCReason = "";
		_lobbyConnected = false;
	}
    #endregion

    #region // Private methods, properties
	private void OnSelectedLocal()
	{
		_sceneUI.Deactivate();
		TransitScene("GameLocalScene");
	}
	private void OnSelectedOnline()
	{
		ConnectServer();
		SetMatchButtonLabel("connecting...");
		DisableMatchButton();
		_sceneUI.OnlinePlayWindow.Show();
	}

    private void ConnectServer()
    {
		// ユーザーネーム更新
		UserName = _sceneUI.PlayerNameText;

		// UIを無効か
		_sceneUI.Deactivate();

		// エラーメッセージをクリア
		_sceneUI.SetErrorText("");

		// 接続情報を設定
		ConfigData cfg = SFClientManager.TCPConfigData;

#if UNITY_WEBGL
		// 接続情報を設定
		ConfigData cfg = SFClientManager.HTTPConfigData;
#endif

		// クライアントを初期化
#if !UNITY_WEBGL
		_server = _sfManager.CreateSFClient();
#else
		_server = _sfManager.CreateSFClient(UseWebSocket.WS_BIN)
#endif

		// Loggerの設定
		_server.Logger.EnableConsoleTrace = cfg.Debug;

		// イベント割り当て
		AddSFListeners();

		// SmartFoxServerに接続
		_server.Connect(cfg);
    }

	/// <summary>
	/// オンライン状態が確認できたとき
	/// </summary>
	private void EnableMatchButton()
	{
		_sceneUI.OnlinePlayWindow.MatchMakingButton.BaseComponent.interactable = true;
	}

	/// <summary>
	/// オンライン状態が確認できなかったとき
	/// </summary>
	private void DisableMatchButton()
	{
		_sceneUI.OnlinePlayWindow.MatchMakingButton.BaseComponent.interactable = false;
	}

	private void SetMatchButtonCancellable(bool cancellable)
	{
		if(cancellable)
		{
			_sceneUI.OnlinePlayWindow.MatchMakingButton.RemoveAllListenerOnClick();
			_sceneUI.OnlinePlayWindow.MatchMakingButton.AddListenerOnClick(CancelMatchMaking);
		}
		else
		{
			_sceneUI.OnlinePlayWindow.MatchMakingButton.RemoveAllListenerOnClick();
			_sceneUI.OnlinePlayWindow.MatchMakingButton.AddListenerOnClick(StartMatchMaking);
		}
	}

	/// <summary>
	/// マッチ開始ボタンのテキストを変更
	/// </summary>
	/// <param name="text"></param>
	private void SetMatchButtonLabel(string text)
	{
		_sceneUI.OnlinePlayWindow.MatchMakingButton.SetLabel(text);
	}

	/// <summary>
	/// ログイン試行
	/// </summary>
	private void StartMatchMaking()
	{
		// UI無効
		_sceneUI.Deactivate();
		SetMatchButtonLabel("Matching...");
		DisableMatchButton();
		StartSearchingRooms();
	}

	/// <summary>
	/// マッチングをキャンセル
	/// </summary>
	private void CancelMatchMaking()
	{
		ManuallyDisconnect("Matching Cancelled.");
	}

	private void OnCloseOnlineWindow()
	{
		ManuallyDisconnect("");
	}

	private void JoinLobby()
	{
		_server.Send(new JoinRoomRequest(SFClientManager.DEFAULT_ROOM));
		Debug.Log("Joining Lobby...");
	}
	private void ManuallyDisconnect(string reason)
	{
		if(_server == null) return;
		if(!_server.IsConnected) return;

		// UI無効か
		_sceneUI.Deactivate();
		// 切断する。OnConnectionLostもコールされる。
		_server.Disconnect();
		// 切断理由をセット
		_manualDCReason = reason;

		// エラー表示
		_sceneUI.SetErrorText(_manualDCReason);
		_sceneUI.ShowError(_manualDCReason);

		// 切断理由をリセット
		_manualDCReason = "";
	}

	/// <summary>
	/// ルーム検索、参加
	/// </summary>
	private void StartSearchingRooms()
	{
		_sceneUI.Deactivate();
		TryConnectRoom();
		_sceneUI.OnlinePlayWindow.Message.SetText("Searching rooms...");
	}

	private void TryConnectRoom()
	{
		// ルーム作成情報
		string roomName = _server.MySelf.Name + "'s game";

		SFSGameSettings settings = new SFSGameSettings(roomName);
		settings.GroupId = SFClientManager.GAME_ROOMS_GROUP_NAME;
		settings.MaxUsers = 2;
		settings.MaxSpectators = 10;
		settings.MinPlayersToStartGame = 2;
		settings.IsPublic = true;
		settings.LeaveLastJoinedRoom = true;
		settings.NotifyGameStarted = false;
		settings.IsGame = true;

		MatchExpression exp = new MatchExpression(RoomProperties.IS_GAME, BoolMatch.EQUALS, true)
        	.And(RoomProperties.HAS_FREE_PLAYER_SLOTS, BoolMatch.EQUALS, true);
		
		_server.Send(new QuickJoinOrCreateRoomRequest(matchExpression: exp, new List<string>(){ SFClientManager.GAME_ROOMS_GROUP_NAME },settings,_server.LastJoinedRoom));
	}

	private void CreatePrivateRoom()
	{
		// Configure Room
		string roomName = _server.MySelf.Name + "'s game";

		SFSGameSettings settings = new SFSGameSettings(roomName);
		settings.GroupId = SFClientManager.GAME_ROOMS_GROUP_NAME;
		settings.MaxUsers = 2;
		settings.MaxSpectators = 10;
		settings.MinPlayersToStartGame = 2;
		settings.IsPublic = false;
		settings.LeaveLastJoinedRoom = true;
		settings.NotifyGameStarted = false;

		// Request Room creation to server
		_server.Send(new CreateSFSGameRequest(settings));
		_sceneUI.OnlinePlayWindow.Message.SetText("Creating a room...");
	}

	private void StartMatch()
	{
		_sceneUI.Deactivate();
		TransitScene("GameOnlineScene");
	}

	private void ReadyToMatchMaking()
	{
		SetMatchButtonCancellable(false);
		EnableMatchButton();
		SetMatchButtonLabel("Start MatchMaking");
	}

	private bool CheckGameStart(Room room)
	{
		return room.IsGame && room.UserCount == room.MaxUsers;
	}

	private void SetTurnVariable(int turn)
	{
		UserVariable variable = new SFSUserVariable("turn",turn);
		_server.MySelf.SetVariable(variable);

		List<UserVariable> vars = new List<UserVariable>();
		vars.Add(variable);

		// Set User Variables
		_server.Send(new SetUserVariablesRequest(vars));
	}
    #endregion

    #region // Interface methods : ISF
	public override void AddSFListeners()
	{
		_server.AddEventListener(SFSEvent.CONNECTION, OnSFConnection);
		_server.AddEventListener(SFSEvent.CONNECTION_LOST, OnSFConnectionLost);
		_server.AddEventListener(SFSEvent.LOGIN, OnSFLogin);
		_server.AddEventListener(SFSEvent.LOGIN_ERROR, OnSFLoginError);

		_server.AddEventListener(SFSEvent.ROOM_ADD, OnSFRoomAdded);
		_server.AddEventListener(SFSEvent.ROOM_REMOVE, OnSFRoomRemoved);
		_server.AddEventListener(SFSEvent.ROOM_JOIN, OnSFRoomJoin);
		_server.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnSFRoomJoinError);
		_server.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnSFRoomCreationError);

		_server.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnSFUserEnterRoom);
		_server.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnSFUserExitRoom);

		_server.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE,OnSFUserVariableChanged);
	}

    public override void RemoveSFListeners()
    {
		if (_server != null)
		{
			_server.RemoveEventListener(SFSEvent.CONNECTION, OnSFConnection);
			_server.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnSFConnectionLost);
			_server.RemoveEventListener(SFSEvent.LOGIN, OnSFLogin);
			_server.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnSFLoginError);
			
			_server.RemoveEventListener(SFSEvent.ROOM_ADD, OnSFRoomAdded);
			_server.RemoveEventListener(SFSEvent.ROOM_REMOVE, OnSFRoomRemoved);
			_server.RemoveEventListener(SFSEvent.ROOM_JOIN, OnSFRoomJoin);
			_server.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnSFRoomJoinError);
			_server.RemoveEventListener(SFSEvent.ROOM_CREATION_ERROR, OnSFRoomCreationError);

			_server.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnSFUserEnterRoom);
			_server.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnSFUserExitRoom);

			_server.RemoveEventListener(SFSEvent.USER_VARIABLES_UPDATE,OnSFUserVariableChanged);
		} 
    }

    // 接続時
    public void OnSFConnection(BaseEvent evt)
    {
		Debug.Log("Connection");
        // 接続できているかチェック
		if ((bool)evt.Params["success"])
		{
			Debug.Log("SFS2X API version: " + _server.Version);
			Debug.Log("Connection mode is: " + _server.ConnectionMode);

			_sceneUI.OnlinePlayWindow.MatchMakingButton.SetLabel("Start Matchmaking");

			// ログイン
			_server.Send(new LoginRequest(UserName));
			Debug.Log(UserName);
		}
		else
		{
			// エラー表示
			string error = "CONNECTION FAILED: Check if the server is running.";
			_sceneUI.SetErrorText(error);
			_sceneUI.ShowError(error);

			_sceneUI.OnlinePlayWindow.Hide();
			DisableMatchButton();

			// UI有効化
			_sceneUI.Activate();
		}
    }

    // 接続切断時
    public void OnSFConnectionLost(BaseEvent evt)
    {
		Debug.Log("Connection Lost");
		// イベントリスナーを削除
		RemoveSFListeners();

		// 一旦ウィンドウをすべて閉じる
		_sceneUI.HideModals();
		
		// エラー取得、表示
		string reason = (string)evt.Params["reason"];
		
		if (reason != ClientDisconnectionReason.MANUAL)
		{
			_sceneUI.SetErrorText("Connection lost: " + reason);
			_sceneUI.ShowError("Connection lost: " + reason);
		}
		else
		{
			
		}

		ReadyToMatchMaking();
		_sceneUI.OnlinePlayWindow.Message.SetText("");

		_lobbyConnected = false;

		// UI有効化
		_sceneUI.Activate();
    }

    // ログイン成功
    public void OnSFLogin(BaseEvent evt)
    {
		Debug.Log("Login");
		_server.MySelf.SetVariable(new SFSUserVariable("turn", 0));
		JoinLobby();
		_sceneUI.Activate();
    }

    // ログイン失敗
    public void OnSFLoginError(BaseEvent evt)
    {
		Debug.Log("Login Error");
		// 切断する。OnConnectionLostもコールされる。
		_server.Disconnect();

		// エラー表示
		string error = "Login failed due to the following error:\n" + (string)evt.Params["errorMessage"];
		_sceneUI.SetErrorText(error);
		_sceneUI.ShowError(error);

		// UI有効化
		_sceneUI.Activate();
    }
	
    public void OnSFRoomCreationError(BaseEvent evt)
    {
		Debug.Log("Room Creation Error");
        // エラー表示
		string error = "Room creation failed: " + (string)evt.Params["errorMessage"];
		_sceneUI.SetErrorText(error);
		_sceneUI.ShowError(error);

		_sceneUI.OnlinePlayWindow.Message.SetText("");

		ReadyToMatchMaking();

		_sceneUI.Activate();
    }

    public void OnSFRoomAdded(BaseEvent evt)
    {
		Debug.Log("Room Added");
    }

    public void OnSFRoomRemoved(BaseEvent evt)
    {
        Debug.Log("Room Remove");
    }

    public void OnSFRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
    	Debug.Log("Room joined: " + room.Name);

		// If a game Room was joined, go to the Game scene, otherwise ignore this event
		if (_lobbyConnected && room.IsGame)
		{
			SetMatchButtonLabel("Cancel");
			EnableMatchButton();
			SetMatchButtonCancellable(true);
			_sceneUI.OnlinePlayWindow.Message.SetText("Waiting for other player...");

			SetTurnVariable(_server.MySelf.GetPlayerId(room) - 1);
			if(CheckGameStart(room)) Invoke("StartMatch",1.0f);
		}
		else
		{
			ReadyToMatchMaking();
			SetTurnVariable(VAR_TURN_INIT);
			_lobbyConnected = true;
		}
    }

    public void OnSFRoomJoinError(BaseEvent evt)
    {
		Debug.Log("Room Join Error");
		// エラー表示
		string error = $"Failed to join room : " + (string)evt.Params["errorMessage"];
		_sceneUI.SetErrorText(error);
		_sceneUI.ShowError(error);

		_sceneUI.OnlinePlayWindow.Message.SetText("");
		ReadyToMatchMaking();

		_sceneUI.Activate();
    }

    public void OnSFUserEnterRoom(BaseEvent evt)
    {
		Debug.Log("User Enter Room");
		Room room = (Room)evt.Params["room"];

		// Display system message
		if(room.IsGame) _sceneUI.OnlinePlayWindow.Message.SetText("Match found!");

		if(CheckGameStart(room)) Invoke("StartMatch",1.0f);

    }

    public void OnSFUserExitRoom(BaseEvent evt)
    {
		Debug.Log("User Exit Room");
    }

    public void OnSFUserVariableChanged(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        List<string> changedVars = (List<string>)evt.Params["changedVars"];
		if(user.IsItMe)
		{
        	Debug.Log("My Var Changed!");
			foreach(string variable in changedVars)
			{
                Debug.Log($"My {variable} : {user.GetVariable(variable)}");
			}
		}
		else
		{
			Debug.Log("Their Var Changed!");
            foreach(string variable in changedVars)
			{
                Debug.Log($"Their {variable} : {user.GetVariable(variable)}");
			}
		}

    }
    #endregion

    #region // Protected methods
    #endregion

    #region // Public methods, properties
    #endregion


}
