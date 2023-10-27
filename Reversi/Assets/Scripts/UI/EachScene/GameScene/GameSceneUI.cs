using System.Collections;
using System.Collections.Generic;
using T0R1.UI;
using UnityEngine;

public class GameSceneUI : SceneUIBase
{
    [SerializeField]
    GameStartPanel _startPanel;

    [SerializeField]
    GameMenuWindow _menuWindow;

    public override void Disable()
    {
        throw new System.NotImplementedException();
    }

    public override void Enable()
    {
        throw new System.NotImplementedException();
    }

    #region // Unity methods
    protected override void OnStart()
    {
        _menuWindow.SetBaseUI(this);
        _menuWindow.Hide();
        MarkAsModal(_menuWindow);

        _startPanel.SetBaseUI(this);
        MarkAsModal(_startPanel);
    }
    #endregion
}
