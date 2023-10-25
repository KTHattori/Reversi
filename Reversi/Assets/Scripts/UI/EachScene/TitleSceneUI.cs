using TMPro;
using UnityEngine;
using UnityEngine.UI;
using T0R1.UI;

public class TitleSceneUI : SceneUIBase
{
    #region // Serialized properties
    [SerializeField]
    private TitleSettingWindow _settingWindow;
    [SerializeField]
    private TextMeshProUGUI _playerNameText;
    [SerializeField]
    private TextMeshProUGUI _errorText;

    [SerializeField]
    private Button _playLocalButton;
    [SerializeField]
    private Button _playOnlineButton;
    [SerializeField]
    private Button _playerSettingButton;
    
    [SerializeField]
    private ErrorWindow _errorWindow;
    #endregion

    #region // Public properties
    public string PlayerNameText => _playerNameText.text;
    public Button PlayLocalButton => _playLocalButton;
    public Button PlayOnlineButton => _playOnlineButton;
    public Button PlayerSettingButton => _playerSettingButton;
    #endregion

    #region // Unity methods
    void Start()
    {
        _settingWindow.SetBaseUI(this);
        _settingWindow.Hide();
        TryBindActionToButton(_playLocalButton,OnPlayLocalClicked);
        TryBindActionToButton(_playOnlineButton,OnPlayOnlineClicked);
        TryBindActionToButton(_playerSettingButton,OnSettingClicked);
    }
    #endregion
    
    #region // Private methods
    void OnPlayLocalClicked()
    {
        Debug.Log("PLAY LOCAL");
        // SceneManager.LoadScene("GameScene");
    }

    void OnPlayOnlineClicked()
    {
        Debug.Log("PLAY ONLINE");
    }

    void OnSettingClicked()
    {
        Debug.Log("OPEN SETTING");
        _settingWindow.Show();
    }
    #endregion

    #region // Protected override methods
    public override void HideModals()
    {
        _settingWindow.Hide();
    }
    #endregion

    #region // Public methods
    public void SetErrorText(string text)
    {
        _errorText.SetText(text);
    }

    public void SetPlayerNameText(string text)
    {
        _playerNameText.SetText(text);
    }

    public override void Enable()
    {
        _playLocalButton.interactable = true;
        _playOnlineButton.interactable = true;
        _playerSettingButton.interactable = true;
    }

    public override void Disable()
    {
        _playLocalButton.interactable = false;
        _playOnlineButton.interactable = false;
        _playerSettingButton.interactable = false;
    }
    #endregion
}
