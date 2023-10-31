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

    [System.Serializable]
    public class SideSettings
    {
        [SerializeField]
        public ObjectReferencer _diffObjRef;
        [SerializeField]
        public ObjectReferencer _playerObjRef;
        
        private Slider _difficultySlider;
        private Slider _playerSlider;
        private TextMeshProUGUI _diffText;

        private bool _isAI = true;
        private Difficulty _difficulty = Difficulty.Normal;

        public bool IsAI => _isAI;
        public Difficulty Difficulty => _difficulty;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            _difficultySlider = _diffObjRef.gameObject.GetComponentInChildren<Slider>();
            _playerSlider = _playerObjRef.gameObject.GetComponentInChildren<Slider>();
            _diffText = _diffObjRef.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            _playerSlider.onValueChanged.AddListener( delegate { OnPlayerChanged();} );
            _difficultySlider.onValueChanged.AddListener( delegate { OnDifficultyChanged();} );

            _playerSlider.value = 0;
            _difficultySlider.value = 1;

            OnPlayerChanged();
            OnDifficultyChanged();
        }

        /// <summary>
        /// プレイヤー/AI選択スライダー変更時に実行する関数
        /// </summary>
        public void OnPlayerChanged()
        {
            if(_playerSlider.value == 1) _isAI = true;
            else _isAI = false;

            SetDiffInteractable(_isAI);
        }

        /// <summary>
        /// 難易度スライダーが変わったときに実行する関数
        /// </summary>
        public void OnDifficultyChanged()
        {
            _difficulty = (Difficulty)_difficultySlider.value;
            _diffText.SetText(_difficulty.ToString());
        }

        /// <summary>
        /// 難易度調整スライダーを有効化 / 無効か
        /// </summary>
        /// <param name="flag"></param>
        public void SetDiffInteractable(bool flag)
        {
            _difficultySlider.interactable = flag;
            if(!flag) _diffText.SetText("HUMAN");
            else _diffText.SetText(_difficulty.ToString());
        }
    }

    [SerializeField]
    ObjectReferencer _inGameObjRef;

    [SerializeField]
    ReversiAIDifficulty _easyDiff;
    [SerializeField]
    ReversiAIDifficulty _normalDiff;
    [SerializeField]
    ReversiAIDifficulty _hardDiff;

    /// <summary>
    /// 黒側の設定
    /// </summary>
    [SerializeField]
    SideSettings _blackside;

    /// <summary>
    /// 白側の設定
    /// </summary>
    [SerializeField]
    SideSettings _whiteside;

    /// <summary>
    /// スタートモードを取得
    /// </summary>
    public ReversiGameManager.PlayMode StartingMode
    {
        get
        {
            if(_blackside.IsAI && _whiteside.IsAI) return ReversiGameManager.PlayMode.EvE;
            else if (_blackside.IsAI || _whiteside.IsAI) return ReversiGameManager.PlayMode.PvE;
            else return ReversiGameManager.PlayMode.PvPLocal;
        }
    }

    /// <summary>
    /// 対応する難易度オブジェクトを取得
    /// </summary>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    public ReversiAIDifficulty GetDifficultyObj(Difficulty difficulty)
    {
        switch(difficulty)
        {
        case Difficulty.Easy:
            return _easyDiff;
        case Difficulty.Normal:
            return _normalDiff;
        case Difficulty.Hard:
            return _hardDiff;
        default:
            return _normalDiff;        
        }
    }

    /// <summary>
    /// 先手かどうか判別
    /// </summary>
    public bool IsInitiative
    {
        get
        {
            if(StartingMode == ReversiGameManager.PlayMode.PvE)
            {
                if(_blackside.IsAI) return false;
                else return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        _blackside.Initialize();
        _whiteside.Initialize();
        _inGameObjRef.DeactivateObject();
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void StartGame()
    {
        _inGameObjRef.ActivateObject();
        ReversiGameManager manager = _inGameObjRef.GetComponent<ReversiGameManager>();

        manager.SetDifficulty(GetDifficultyObj(_blackside.Difficulty),Reversi.DiscColor.Black);
        manager.SetDifficulty(GetDifficultyObj(_whiteside.Difficulty),Reversi.DiscColor.White);

        manager.StartMode(StartingMode,IsInitiative);

        gameObject.SetActive(false);
    }

}
