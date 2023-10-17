using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ReversiOutGameUI : MonoBehaviour
{
    public enum Difficulty
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        MAX_AMOUNT
    }

    [SerializeField]
    ObjectReferencer _diffObjRef;
    [SerializeField]
    ObjectReferencer _turnObjRef;
    [SerializeField]
    ObjectReferencer _inGameObjRef;

    [SerializeField]
    ReversiAIDifficulty _easyDiff;
    [SerializeField]
    ReversiAIDifficulty _normalDiff;
    [SerializeField]
    ReversiAIDifficulty _hardDiff;

    Slider _diffSlider;
    Slider _turnSlider;
    TextMeshProUGUI _diffText;
    TextMeshProUGUI _turnText;

    private void Start()
    {
        _diffSlider = _diffObjRef.gameObject.GetComponentInChildren<Slider>();
        _turnSlider = _turnObjRef.gameObject.GetComponentInChildren<Slider>();
        _diffText = _diffObjRef.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _turnText = _turnObjRef.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _inGameObjRef.DeactivateObject();
        OnDiffChanged();
        OnTurnChanged();
    }

    public void OnDiffChanged()
    {

        _diffText.SetText(((Difficulty)_diffSlider.value).ToString());
    }

    public void OnTurnChanged()
    {
        if(_turnSlider.value == 1)
        {
            _turnText.SetText("First");
        }else
        {
            _turnText.SetText("Second");
        }
    }

    public void StartGame()
    {
        _inGameObjRef.ActivateObject();
        ReversiGameManager manager = _inGameObjRef.GetComponent<ReversiGameManager>();
        switch((Difficulty)_diffSlider.value)
        {
        case Difficulty.Easy:
            manager.SetDifficulty(_easyDiff);
            break;
        case Difficulty.Normal:
            manager.SetDifficulty(_normalDiff);
            break;
        case Difficulty.Hard:
            manager.SetDifficulty(_hardDiff);
            break;
        }
        manager.StartMode(ReversiGameManager.PlayMode.NonPlayerLocal,_turnSlider.value == 1 ? true : false);
        gameObject.SetActive(false);
    }

}
