using System.Collections.Generic;
using UnityEngine;
using Reversi;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

public class ReversiGameManager : MonoSingleton<ReversiGameManager>
{
    /// <summary>
    /// プレイモードの種類
    /// </summary>
    public enum PlayMode
    {
        /// <summary>
        /// AIとのローカル対戦
        /// </summary>
        NonPlayerLocal = 0,
        /// <summary>
        /// プレイヤーとのローカル対戦
        /// </summary>
        PlayerLocal = 1,
        /// <summary>
        /// プレイヤーとのネットワーク対戦
        /// </summary>
        PlayerNetwork = 2,
    }

    // 定数 Constants

    /// <summary>
    /// 先手を表す定数
    /// </summary>
    private const int Initiative = 0;
    /// <summary>
    /// 黒（先手）
    /// </summary>
    private const int BlackSide = Initiative;
    /// <summary>
    /// 白（後手）
    /// </summary>
    private const int WhiteSide = Initiative + 1;

    /// <summary>
    /// ボード内部データ
    /// </summary>
    private static Board _board = null;

    /// <summary>
    /// 3D盤面オブジェクトのマネージャ
    /// </summary>
    [SerializeField]
    private ReversiBoard3D _3dboard;

    [SerializeField]
    private PlayMode _mode = PlayMode.NonPlayerLocal;

    /// <summary>
    /// 先手かどうか
    /// </summary>
    [SerializeField]
    private bool _isInitiative = false;

    /// <summary>
    /// AIの強さを定義したScriptableObject
    /// </summary>
    [SerializeField]
    private ReversiAIDifficulty _aiDifficulty;

    /// <summary>
    /// プレイヤー0用UIへの参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _playerUI0Ref;

    /// <summary>
    /// プレイヤー1用UIへの参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _playerUI1Ref;

    // Private: Non-Serialized

    /// <summary>
    /// 配置可能なマスを保持するリスト
    /// </summary>
    private List<Point> _movablePoints = new List<Point>();

    /// <summary>
    /// プレイヤー操作が可能かどうか
    /// </summary>
    private bool _isPlayerInteractable = false;

    private IReversiPlayer[] _player = new IReversiPlayer[2];
    private ReversiUIManager[] _ui = new ReversiUIManager[2];

    [SerializeField]
    private int _currentPlayer = 0;
    private int _playerSide = -1;
    private bool _completedThinking = false;
    [SerializeField]
    private Point _selectedPoint = null;
    private SynchronizationContext _context;
    private bool _turnUpdated = false;

    public int CurrentPlayer => _currentPlayer;
    public int OppositePlayer => _currentPlayer == BlackSide ? WhiteSide : BlackSide;


    // Start is called before the first frame update

    /// <summary>
    /// インスタンス生成時の処理
    /// </summary>
    protected override void OnInitialize()
    {
        _board = new Board();
        _selectedPoint = null;
        _context = SynchronizationContext.Current;
    }

    /// <summary>
    /// インスタンス破棄時の処理
    /// </summary>
    protected override void OnFinalize()
    {
        StopAllCoroutines();
        Destroy(_3dboard);
    }

    private void Start()
    {
        _ui[BlackSide] = _playerUI0Ref.GetComponent<ReversiUIManager>();
        _ui[WhiteSide] = _playerUI1Ref.GetComponent<ReversiUIManager>();
        _ui[BlackSide].Deactivate();
        _ui[WhiteSide].Deactivate();
        _ui[BlackSide].HideResult();
        _ui[WhiteSide].HideResult();

        StartMode(_mode,_isInitiative);
    }

    private void Update()
    {
        if(_turnUpdated)
        {
            OnTurn();
            _turnUpdated = false;
        }
    }

    /// <summary>
    /// 指定したゲームモードを始める
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="isInitiative"></param>
    public void StartMode(PlayMode mode,bool isInitiative)
    {
        _turnUpdated = true;
        switch(mode)
        {
        case PlayMode.NonPlayerLocal:
            CreateGameWithAI(isInitiative);
            break;
        case PlayMode.PlayerLocal:
            CreateGameWithHuman(false);
            break;
        case PlayMode.PlayerNetwork:
            CreateGameWithHuman(true);
            break;
        }
    }
    
    /// <summary>
    /// 現在のゲームモードをやり直す
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="isInitiative"></param>
    public void RestartMode(PlayMode mode,bool isInitiative)
    {
        switch(mode)
        {
        case PlayMode.NonPlayerLocal:
            InitializeGameWithAI(isInitiative);
            break;
        default:
            InitializeGameWithHuman();
            break;
        } 
    }

    /// <summary>
    /// 対人戦の作成
    /// </summary>
    /// <param name="_isNetwork">ネットワークかどうか</param>
    public void CreateGameWithHuman(bool _isNetwork)
    {
         // モードをセット
        if(_isNetwork) _mode = PlayMode.PlayerNetwork;
        else _mode = PlayMode.PlayerLocal;

        // プレイヤーをセット
        {
            _player[BlackSide] = new HumanPlayer();
            _player[WhiteSide] = new HumanPlayer();
        }

        // 盤面データの初期化
        _board.Init();

        // 3D盤面の作成と初期化
        _3dboard.CreateBoard(_board);
        _3dboard.InitializeBoard(_board);

        // UI有効化・更新
        _ui[BlackSide].Activate();
        _ui[WhiteSide].Activate();
        _ui[BlackSide].HideResult();
        _ui[WhiteSide].HideResult();
        UpdateUI(_mode);
        // スタートメッセージを表示
        ShowMessage(_mode,"Game Start!");
    }

    /// <summary>
    /// 対人戦の初期化
    /// </summary>
    public void InitializeGameWithHuman()
    {
        // 盤面データの初期化
        _board.Init();
        // 3D盤面の初期化
        _3dboard.InitializeBoard(_board);

        // UI有効化・更新
        _ui[BlackSide].Activate();
        _ui[WhiteSide].Activate();
        _ui[BlackSide].HideResult();
        _ui[WhiteSide].HideResult();
        UpdateUI(_mode);
        // スタートメッセージを表示
        ShowMessage(_mode,"Game Start!");
    }

    /// <summary>
    /// 対AI戦の初期化
    /// </summary>
    /// <param name="_isInitiative">先手かどうか</param>
    public void CreateGameWithAI(bool _isInitiative)
    {
        Debug.Log("Created Game With AI");

        // モードをセット
        _mode = PlayMode.NonPlayerLocal;

        // プレイヤーをセット
        if(_isInitiative)
        {
            _player[BlackSide] = new HumanPlayer();
            _player[WhiteSide] = new AIPlayer(_aiDifficulty);
            _playerSide = BlackSide;
        }
        else
        {
            _player[BlackSide] = new AIPlayer(_aiDifficulty);
            _player[WhiteSide] = new HumanPlayer();
            _playerSide = WhiteSide;
        }

        // 盤面データの初期化
        _board.Init();

        // 3D盤面の作成と初期化
        _3dboard.CreateBoard(_board);
        _3dboard.InitializeBoard(_board);

        // UI有効化・更新
        _ui[_playerSide].Activate();
        _ui[_playerSide].HideResult();
        UpdateUI(_mode);
        // スタートメッセージを表示
        ShowMessage(_mode,"Game Start!");
    }

    /// <summary>
    /// 対AI戦の初期化
    /// </summary>
    /// <param name="_isInitiative"></param>
    public void InitializeGameWithAI(bool _isInitiative)
    {
        // 盤面データの初期化
        _board.Init();

        // 3D盤面の初期化
        _3dboard.InitializeBoard(_board);

        // UI有効化・更新
        _ui[_playerSide].Activate();
        _ui[_playerSide].HideResult();
        UpdateUI(_mode);
        // スタートメッセージを表示
        ShowMessage(_mode,"Game Start!");
    }

    /// <summary>
    /// マスを選択し、思考を終了する
    /// </summary>
    /// <param name="point"></param>
    public void SelectPoint(Point point)
    {
        _completedThinking = true;
        _selectedPoint = point;
    }

    /// <summary>
    /// マスを選ばずに思考を終了する。Undo扱いになる。
    /// </summary>
    public void Undo()
    {
        _completedThinking = true;
        _selectedPoint = null;
    }

    /// <summary>
    /// メッセージテキストを指定する。
    /// </summary>
    /// <param name="playMode"></param>
    /// <param name="content"></param>
    private void ShowMessage(PlayMode playMode, string content)
    {
        if(playMode == PlayMode.NonPlayerLocal)
        {
            _ui[_playerSide].SetMessageText(content);
        }
        else
        {
            _ui[BlackSide].SetMessageText(content);
            _ui[WhiteSide].SetMessageText(content);
        }
    }

    /// <summary>
    /// UIを更新する
    /// </summary>
    /// <param name="playMode"></param>
    private void UpdateUI(PlayMode playMode)
    {
        if(playMode == PlayMode.NonPlayerLocal)
        {
            _ui[_playerSide].SetTurnNumber(_board.GetCurrentTurn());
            _ui[_playerSide].SetBackgroundColor(_board.GetCurrentColor().ToColor());
        }
        else
        {
            _ui[BlackSide].SetTurnNumber(_board.GetCurrentTurn());
            _ui[BlackSide].SetBackgroundColor(_board.GetCurrentColor().ToColor());
            _ui[WhiteSide].SetTurnNumber(_board.GetCurrentTurn());
            _ui[WhiteSide].SetBackgroundColor(_board.GetCurrentColor().ToColor());
        }
    }

    /// <summary>
    /// 各手番での行動
    /// </summary>
    private void OnTurn()
    {    
        // 初めにハイライトを削除
        _3dboard.RemoveHighlight(_movablePoints);
        _movablePoints = _board.GetMovablePoints();

        // メッセージリセット
        _ui[CurrentPlayer].SetMessageText("");
        _ui[OppositePlayer].SetMessageText("");

        if( _mode == PlayMode.NonPlayerLocal && _currentPlayer != _playerSide)
        {
            _ui[OppositePlayer].SetMessageText("Thinking...");
            StartWait();
            return;
        }
        else if(_board.IsPassable())
        {
            _ui[CurrentPlayer].SetMessageText("Passed!");
            _ui[OppositePlayer].SetMessageText("Passed!");
            _board.Pass();
            SwapTurn();
        }
        else
        {
            _ui[OppositePlayer].SetMessageText("Thinking...");
            _3dboard.HighlightMovable(_movablePoints,_board.GetCurrentColor());
            StartWait();
        }
    }

    /// <summary>
    /// 思考待機開始
    /// </summary>
    private async void StartWait()
    {
        await Task.Run(() => _player[_currentPlayer].Think(_board));
        StartCoroutine(WaitForSelect());
    }

    /// <summary>
    /// プレイヤー行動待機
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForSelect()
    {
        yield return new WaitUntil(() => _completedThinking);
        Act(_selectedPoint);
        _selectedPoint = null;
        _completedThinking = false;
        SwapTurn();
    }

    /// <summary>
    /// 実際の行動と、行動に基づいた更新内容
    /// </summary>
    /// <param name="point"></param>
    private void Act(Point point)
    {
        IReversiPlayer.ActionResult result = _player[_currentPlayer].Act(_board,point);

        switch(result)
        {
        case IReversiPlayer.ActionResult.Placed:
            _3dboard.UpdateBoardOnPlace(_board.GetUpdate());
            break;
        case IReversiPlayer.ActionResult.Undone:
            _3dboard.UpdateBoardOnUndo(_board.GetUndone());
            if(_currentPlayer == _playerSide)
            {   // AI戦で、プレイヤーの手番ならもう一度Undo
                _board.Undo();
                _3dboard.UpdateBoardOnUndo(_board.GetUndone());
                SwapTurn();
            }
            break;
        }
    }

    /// <summary>
    /// 手番の入れ替え
    /// </summary>
    private void SwapTurn()
    {
        _currentPlayer = ++_currentPlayer % 2;
        _turnUpdated = true;
        OnSwapTurn();
    }

    private void OnSwapTurn()
    {
        UpdateUI(_mode);
        if(!_board.IsGameOver()) return;

        _turnUpdated = false;

        switch(_mode)
        {
        case PlayMode.NonPlayerLocal:
            _ui[_playerSide].SetResultContent(_board.CountDisc(DiscColor.Black),_board.CountDisc(DiscColor.White));
            _ui[_playerSide].ShowResult();
            break;
        case PlayMode.PlayerLocal:
            _ui[_playerSide].SetResultContent(_board.CountDisc(DiscColor.Black),_board.CountDisc(DiscColor.White));
            _ui[_playerSide].ShowResult();
            break;
        case PlayMode.PlayerNetwork:
            _ui[BlackSide].SetResultContent(_board.CountDisc(DiscColor.Black),_board.CountDisc(DiscColor.White));
            _ui[WhiteSide].SetResultContent(_board.CountDisc(DiscColor.Black),_board.CountDisc(DiscColor.White));
            _ui[BlackSide].ShowResult();
            _ui[WhiteSide].ShowResult();
            break;
        }
    }

    /// <summary>
    /// 評価値を指定したマスに表示
    /// </summary>
    /// <param name="point"></param>
    /// <param name="score"></param>
    public void DisplayEvalScore(Point point,int score)
    {
        _3dboard.DisplayEvalScore(point,score);
    }
}
