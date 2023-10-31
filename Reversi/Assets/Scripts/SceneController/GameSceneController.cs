using System.Collections.Generic;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Entities.Variables;
public class GameSceneController : SceneController,ISFConnectable,ISFRoomAccessWatcher,ISFMessageReceiver,ISFUserVariableWatcher
{
    #region // Constants
	public static string MSG_RECV_CALLBACK = "MSG_RCV";
    public static string MSG_TIMEOVER = "MSG_TIMEOVER";
    private static readonly int VAR_TURN_BLACKSIDE = 0;
	private static readonly int VAR_TURN_WHITESIDE = 1;
	#endregion

    #region // Private variables
    private SmartFox _server;
    private ReversiGameNetwork _reversi;
	private string _manualDCReason = "";
    private int _side = -1;
    private bool clientReady = false;
    private bool opponentReady = false;
    private bool started = false;
    #endregion

    #region // Serialized variables
    [SerializeField]
    GameSceneUI _sceneUI;
    #endregion

    public GameSceneUI SceneUI { get {return _sceneUI; } }

    #region // Override Unity Methods
    protected override void OnStart()
    {
       	// 取得
		_server = _sfManager.GetSFClient();

        clientReady = false;
        opponentReady = false;
        started = false;



        if(_server != null && _server.IsConnected)
        {
            _reversi = ReversiGameNetwork.Instance;
            _sceneUI.StartPanel.AddActionOnHidden(ReadyToStart);
            _sceneUI.MenuWindow.DisconnectButton.AddActionOnHidden(ExitGame);

            AddSFListeners();

            _side = _server.MySelf.GetVariable("turn").GetIntValue();
            _sceneUI.StartPanel.SetInitiative(_side == VAR_TURN_BLACKSIDE);
            
            _sceneUI.MenuWindow.DisconnectButton.AddListenerOnClick(ExitGame);
            _sceneUI.ResultPanel.BackTitleButton.AddListenerOnClick(ExitGame);
            _sceneUI.ResultPanel.BackTitleButton.Hide();
        }
        else
        {
            _sceneUI.StartPanel.Hide();
            _sceneUI.MenuWindow.DisconnectButton.AddActionOnHidden(ExitGame);
            _sceneUI.MenuWindow.DisconnectButton.SetLabel("Exit Game");
        }

    }

    protected override void OnUpdate()
    {
        if(clientReady && opponentReady && !started)
        {
            StartGame();
            started = true;
        }
    }
    #endregion


    #region // Public methods
    public void SendSFMessage(string msg)
    {
        if(!_server.IsConnected) return;
        // Send public message to Room
		_server.Send(new PublicMessageRequest(msg));
    }
    public void SendCallBack()
    {
        SendSFMessage(MSG_RECV_CALLBACK);
    }

    public void SetThinkingState(bool flag)
    {
        UserVariable variable = new SFSUserVariable("thinking",flag);
		_server.MySelf.SetVariable(variable);

		List<UserVariable> vars = new List<UserVariable>();
		vars.Add(variable);

		// Set User Variables
		_server.Send(new SetUserVariablesRequest(vars));
    }
    #endregion

    #region // Private methods
    private void StartGame()
    {
        _sceneUI.ReversiPanel.Show();
        _reversi.gameObject.SetActive(true);
        _reversi.StartMode(_side);
    }
    private void ReadyToStart()
    {
        if(!started && _server != null && _server.IsConnected)
        {
            UserVariable variable = new SFSUserVariable("ready",true);
            _server.MySelf.SetVariable(variable);

            List<UserVariable> vars = new List<UserVariable>();
            vars.Add(variable);

            // Set User Variables
            _server.Send(new SetUserVariablesRequest(vars));
        }
    }
    private void ExitGame()
    {
        if(_server.IsConnected)
        {
            _server.Send(new LeaveRoomRequest());
        }
        else
        {
            _sceneUI.Deactivate();
            TransitScene("TitleScene");
        }
    }
    private void OnSendMessage(string msg)
    {
        Debug.Log($"Sent: {msg}");

        if(msg == MSG_TIMEOVER)
        {
            ExitGame();
        }
        // ReversiGamePVP.Instance.SelectPoint(new Reversi.Point(msg));
    }
    private void OnReceiveMessage(string msg)
    {
        Debug.Log($"Received: {msg}");
        if(msg == MSG_RECV_CALLBACK)
        {
            OnCallBackReceived();
        }
        else if(msg == MSG_TIMEOVER)
        {
            ExitGame();
        }
        else
        {
            ReversiGameNetwork.Instance.ReceivePoint(new Reversi.Point(msg));
        }
    }
    private void OnCallBackReceived()
    {
        // ReversiGamePVP.Instance.CompleteThink();
    }
    #endregion

    public override void AddSFListeners()
    {
        _server.AddEventListener(SFSEvent.CONNECTION,OnSFConnection);
        _server.AddEventListener(SFSEvent.CONNECTION_LOST,OnSFConnectionLost);
        _server.AddEventListener(SFSEvent.USER_ENTER_ROOM,OnSFUserEnterRoom);
        _server.AddEventListener(SFSEvent.USER_EXIT_ROOM,OnSFUserExitRoom);

        _server.AddEventListener(SFSEvent.PUBLIC_MESSAGE,OnSFMessageReceived);
        _server.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE,OnSFUserVariableChanged);
    }
    public override void RemoveSFListeners()
    {
        if(_server == null) return;
        _server.RemoveEventListener(SFSEvent.CONNECTION,OnSFConnection);
        _server.RemoveEventListener(SFSEvent.CONNECTION_LOST,OnSFConnectionLost);
        _server.RemoveEventListener(SFSEvent.USER_ENTER_ROOM,OnSFUserEnterRoom);
        _server.RemoveEventListener(SFSEvent.USER_EXIT_ROOM,OnSFUserExitRoom);

        _server.RemoveEventListener(SFSEvent.PUBLIC_MESSAGE,OnSFMessageReceived);
        _server.RemoveEventListener(SFSEvent.USER_VARIABLES_UPDATE,OnSFUserVariableChanged);
    }

    public void OnSFConnection(BaseEvent evt)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSFConnectionLost(BaseEvent evt)
    {
        Debug.Log("Connection Lost");
		// イベントリスナーを削除
		if(_server.IsConnected) RemoveSFListeners();

        clientReady = false;
        opponentReady = false;
        started = false;

		// 一旦ウィンドウをすべて閉じる
		_sceneUI.HideModals();

        TransitScene("TitleScene");
    }

    public void OnSFUserEnterRoom(BaseEvent evt)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSFUserExitRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        if(user.IsItMe)
        {
            _sceneUI.ShowError("You have been disconnected.");
            _server.Disconnect();
        }
        else
        {
            _server.Send(new LeaveRoomRequest());
            _sceneUI.ShowError("Opponent has been disconnected.");
        }
    }

    public void OnSFMessageReceived(BaseEvent evt)
    {
        User sender = (User)evt.Params["sender"];
		string message = (string)evt.Params["message"];

        Debug.Log("Message");

        if(sender != _server.MySelf)
        {
            OnReceiveMessage(message);
        }
        else
        {
            OnSendMessage(message);
        }
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

                if(variable == "ready" && user.GetVariable(variable).GetBoolValue())
                {
                    clientReady = true;
                }
			}
		}
		else
		{
			Debug.Log("Their Var Changed!");
            foreach(string variable in changedVars)
			{
                Debug.Log($"Their {variable} : {user.GetVariable(variable)}");

                if(variable == "ready" && user.GetVariable(variable).GetBoolValue())
                {
                    opponentReady = true;
                    _reversi.SetUserNames(_server.MySelf.Name,user.Name);
                }
			}
            
            
		}
    }
}
