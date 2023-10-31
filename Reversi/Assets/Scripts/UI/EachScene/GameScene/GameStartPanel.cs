using T0R1.UI;
using TMPro;
using UnityEngine;

public class GameStartPanel : ModalPanel
{
    [SerializeField]
    private TextMeshProUGUI _turnNotifyText;
    public TextMeshProUGUI TurnNotify => _turnNotifyText;

    [SerializeField]
    private ButtonTextEdit _confirmButton;

    [SerializeField]
    private float _disappearTimer = 5.0f;
    private int _oldSecondInt = 0;

    private bool _onGoingCount = false;

    protected override void OnAwake()
    {
        _onGoingCount = true;
        _oldSecondInt = Mathf.RoundToInt(_disappearTimer);
    }

    protected override void OnStart()
    {
        _confirmButton.OnClickEvent.AddListener(OnStartClicked);
    }

    protected override void OnUpdate()
    {
        if(_onGoingCount)
        {
            _disappearTimer -= Time.deltaTime;
            int secondInt = Mathf.RoundToInt(_disappearTimer);
            if(secondInt != _oldSecondInt)
            {
                if(secondInt < 0)
                {
                    _onGoingCount = false;
                    Hide();
                    return;
                }
                _confirmButton.SetLabel($"READY...{Mathf.RoundToInt(_disappearTimer)}");
                _oldSecondInt = secondInt;
            }
        }
    }

    public void SetInitiative(bool initiative)
    {
        if(initiative) _turnNotifyText.SetText("センコウ");
        else _turnNotifyText.SetText("コウコウ");
    }

    public void OnStartClicked()
    {
        _disappearTimer = -1.0f;
    }


}
