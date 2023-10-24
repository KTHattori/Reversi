using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using UnityEngine;

public class TitleSceneController : SceneController
{

    #region // Private variables
    private SmartFox _sfs;
    #endregion

    #region // Public variables

    #endregion

    #region // Private methods, properties
	private void AddSFSListeners()
	{
		_sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		_sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		_sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
		_sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
	}

    private void OnConnection(BaseEvent evt)
    {

    }

    private void OnConnectionLost(BaseEvent evt)
    {

    }

    private void OnLogin(BaseEvent evt)
    {

    }

    private void OnLoginError(BaseEvent evt)
    {

    }
    #endregion

    #region // Interface methods : IMonoBehaviourMethods
    public override void OnInitialize()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFinalize()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region // Protected methods
	protected override void RemoveSFSListeners()
	{
		if (_sfs != null)
		{
			_sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
			_sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			_sfs.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
			_sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		}
	}
	protected override void HideModals()
	{
		// No modals used by this scene
	}
    #endregion
    
    #region // Public methods, properties
    #endregion


}
