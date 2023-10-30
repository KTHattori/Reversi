using System.Collections.Generic;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
public class GameSceneController : SceneController,ISFConnectable,ISFRoomAccessWatchable,ISFMessageReceiver
{
    #region // Constants
	public static string MSG_RECV_CALLBACK = "MSG_RCV";
	#endregion

    #region // Private variables
    private SmartFox _server;
    private ReversiGamePVP _reversi;
	private string _manualDCReason = "";
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

        _reversi = ReversiGamePVP.Instance;

        if(_server != null && _server.IsConnected)
        {
            _sceneUI.StartPanel.AddActionOnHidden(ReadyToStart);
            _sceneUI.MenuWindow.DisconnectButton.AddActionOnHidden(ExitGame);

            AddSFListeners();
        }
        else
        {
            _sceneUI.StartPanel.Hide();
            _sceneUI.MenuWindow.DisconnectButton.AddActionOnHidden(ExitGame);
            _sceneUI.MenuWindow.DisconnectButton.SetLabel("Exit Game");
        }

    }
    #endregion


    #region // Public methods
    public void SendSFMessage(string msg)
    {
        // Send public message to Room
		_server.Send(new PublicMessageRequest(msg));
    }
    public void SendCallBack()
    {
        SendSFMessage(MSG_RECV_CALLBACK);
    }
    #endregion

    #region // Private methods
    private void ReadyToStart()
    {
        var turn = _server.MySelf.GetVariable("turn");
        _reversi.gameObject.SetActive(true);
        _reversi.StartMode(turn.GetIntValue());
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
        // ReversiGamePVP.Instance.SelectPoint(new Reversi.Point(msg));
    }
    private void OnReceiveMessage(string msg)
    {
        Debug.Log($"Received: {msg}");
        if(msg == MSG_RECV_CALLBACK)
        {
            OnCallBackReceived();
        }
        else
        {
            ReversiGamePVP.Instance.ReceivePoint(new Reversi.Point(msg));
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
    }
    public override void RemoveSFListeners()
    {
        if(_server == null) return;
        _server.RemoveEventListener(SFSEvent.CONNECTION,OnSFConnection);
        _server.RemoveEventListener(SFSEvent.CONNECTION_LOST,OnSFConnectionLost);
        _server.RemoveEventListener(SFSEvent.USER_ENTER_ROOM,OnSFUserEnterRoom);
        _server.RemoveEventListener(SFSEvent.USER_EXIT_ROOM,OnSFUserExitRoom);

        _server.RemoveEventListener(SFSEvent.PUBLIC_MESSAGE,OnSFMessageReceived);
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

		// 一旦ウィンドウをすべて閉じる
		_sceneUI.HideModals();
    }

    public void OnSFUserEnterRoom(BaseEvent evt)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSFUserExitRoom(BaseEvent evt)
    {
        // User user = (User)evt.Params["user"];

        // if()

        // if(user != _server.MySelf)
        // {
        //     _sceneUI.ShowError("You have been disconnected.");
        // }
        // else
        // {
        //     _sceneUI.ShowError("Opponent has been disconnected.");
        // }

        // _server.Disconnect();
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
}
