using System.Collections;
using System.Collections.Generic;
using Reversi;
using T0R1.UI;
using UnityEngine;

public class GameSceneUI : SceneUIBase
{
    [SerializeField]
    GameStartPanel _startPanel;

    [SerializeField]
    GameReversiUI _reversiUI;

    [SerializeField]
    GameMenuWindow _menuWindow;

    [SerializeField]
    GameResultPanel _resultPanel;

    public GameStartPanel StartPanel { get { return _startPanel; }}
    public GameReversiUI ReversiPanel { get{ return _reversiUI; }}
    public GameMenuWindow MenuWindow { get{ return _menuWindow; }}
    public GameResultPanel ResultPanel { get { return _resultPanel; }}

    public override void Deactivate()
    {
        ReversiPanel.Deactivate();
    }

    public override void Activate()
    {
        ReversiPanel.Activate();
    }

    #region // Unity methods
    protected override void OnStart()
    {
        _menuWindow.SetBaseUI(this);
        _menuWindow.Hide();
        MarkAsModal(_menuWindow);

        _reversiUI.SetBaseUI(this);
        _reversiUI.Hide();

        _startPanel.SetBaseUI(this);
        _startPanel.Show();
        MarkAsModal(_startPanel);

        _resultPanel.SetBaseUI(this);
        _resultPanel.Hide();
        MarkAsModal(_resultPanel);

        ReversiPanel.MenuButton.AddListenerOnClick(ShowMenu);

    }
    #endregion

    private void ShowMenu()
    {
        MenuWindow.Show();
    }
}
