using T0R1.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleOnlinePlayWindow : ModalWindow
{
    [SerializeField]
    private TitleSceneUI _baseSceneUI;
    [SerializeField]
    private ButtonTextEdit _buttonTextEdit;
    [SerializeField]
    private TextMeshProUGUI _messageText;

    [SerializeField]
    private Image _loaderImage;

    [SerializeField]
    private Sprite _loadingSprite;

    [SerializeField]
    private Sprite _nonLoadingSprite;

    public ButtonTextEdit MatchMakingButton => _buttonTextEdit;
    public TextMeshProUGUI Message => _messageText;

    protected override void OnStart()
    {
        if(_baseSceneUI == null) _baseSceneUI = GetBaseUI<TitleSceneUI>();
    }
}
