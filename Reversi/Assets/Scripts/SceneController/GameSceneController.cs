using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Match;
using Sfs2X.Requests;
using Sfs2X.Requests.Game;
using Sfs2X.Util;

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

    #region // Override Unity Methods
    protected override void OnStart()
    {
       	// 取得
		_server = _sfManager.GetSFClient();
        AddSFListeners();

        var turn = _server.MySelf.GetVariable("turn");
        _reversi = ReversiGamePVP.Instance;
        _reversi.gameObject.SetActive(true);
        _reversi.StartMode(turn.GetIntValue());
    }

    protected override void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            SendSFMessage("A");
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            SendSFMessage("B");
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            SendSFMessage("C");
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
    private void OnSendMessage(string msg)
    {
        // ReversiGamePVP.Instance.SelectPoint(new Reversi.Point(msg));
    }
    private void OnReceiveMessage(string msg)
    {
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
        //throw new System.NotImplementedException();
    }

    public void OnSFUserEnterRoom(BaseEvent evt)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSFUserExitRoom(BaseEvent evt)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSFMessageReceived(BaseEvent evt)
    {
        User sender = (User)evt.Params["sender"];
		string message = (string)evt.Params["message"];

        MessageViewer.Instance.SetText(message);

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
