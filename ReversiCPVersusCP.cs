using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Reversi;


public class ReversiCPVersusCP : ReversiGameManager
{
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

    /// <summary>
    /// プレイヤー0用UIへの参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _UIRef;

    /// <summary>
    /// 定石ファイルへの参照
    /// </summary>
    [SerializeField]
    private AssetReference _bookAssetRef;

    // Private: Non-Serialized

    /// <summary>
    /// 配置可能なマスを保持するリスト
    /// </summary>
    private List<Point> _movablePoints = new List<Point>();

    /// <summary>
    /// プレイヤーの枠
    /// </summary>
    private IReversiPlayer[] _player = new IReversiPlayer[2];

    /// <summary>
    /// UIマネージャ
    /// </summary>
    private ReversiUIManager _ui = new ReversiUIManager();

    /// <summary>
    /// AIの強さを定義したScriptableObject 黒側
    /// </summary>
    private ReversiAIDifficulty _blackAIDifficulty;
    
    /// <summary>
    /// AIの強さを定義したScriptableObject 白側
    /// </summary>
    private ReversiAIDifficulty _whiteAIDifficulty;


    /// <summary>
    /// 現在の手番
    /// </summary>
    private int _currentPlayer = 0;

    /// <summary>
    /// 思考完了フラグ
    /// </summary>
    private bool _completedThinking = false;

    /// <summary>
    /// 選択されたマス
    /// </summary>
    private Point _selectedPoint = null;

    /// <summary>
    /// メインスレッドへの参照
    /// </summary>
    private SynchronizationContext _mainThread;

    /// <summary>
    /// ターン更新フラグ
    /// </summary>
    private bool _turnUpdated = false;

    /// <summary>
    /// タスクキャンセルのトークン元
    /// </summary>
    private CancellationTokenSource _thinkCancelTokenSrc = null;

    /// <summary>
    /// ターンでの経過時間
    /// </summary>
    private float _turnTime = 0.0f;

    /// <summary>
    /// ゲーム終了フラグ
    /// </summary>
    private bool _gameDestroyed = false;

    /// <summary>
    /// 現在の手番を取得するプロパティ
    /// </summary>
    public int CurrentPlayer => _currentPlayer;

    /// <summary>
    /// 現在と反対の手番を取得するプロパティ
    /// </summary>
    public int OppositePlayer => _currentPlayer == BlackSide ? WhiteSide : BlackSide;


    // Start is called before the first frame update

    /// <summary>
    /// インスタンス生成時の処理
    /// </summary>
    public override void OnInitialize()
    {
        _board = new Board();
        _selectedPoint = null;
        _mainThread = SynchronizationContext.Current;
        _gameDestroyed = false;
    }

    /// <summary>
    /// インスタンス破棄時の処理
    /// </summary>
    public override void OnFinalize()
    {
        StopAllCoroutines();
        Destroy(_3dboard);
    }

    /// <summary>
    /// ターンが更新されたのを検知したら次のターン処理
    /// </summary>
    private void Update()
    {
        if(_turnUpdated)
        {
            OnTurn();
            _turnUpdated = false;
            _turnTime = 0.0f;
        }
        else
        {
            if(_turnTime > Constant.TurnTimeOver)
            {
                StopThink();
                return;
            }
            _ui.ThinkAnimation();
            _ui.ThinkAnimation();

            _ui.SetMessageText($"AI Thinking{_ui.ThinkSuffix}");
        }
    }

    public void StopThink()
    {
        _thinkCancelTokenSrc.Cancel();
    }

    /// <summary>
    /// 難易度定義オブジェクトをセットする
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetDifficulty(ReversiAIDifficulty diff,DiscColor color)
    {
        if(color == DiscColor.Black) _blackAIDifficulty = diff;
        if(color == DiscColor.White) _whiteAIDifficulty = diff;
    }

    /// <summary>
    /// 指定したゲームモードを始める
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="isInitiative"></param>
    public void StartMode(PlayMode mode,bool isInitiative)
    {
        _turnUpdated = true;
        CreateGameOnlyAI();
    }
    
    /// <summary>
    /// 現在のゲームモードをやり直す
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="isInitiative"></param>
    public void RestartMode(PlayMode mode,bool isInitiative)
    {
        _turnUpdated = true;
        InitializeGameOnlyAI();
    }

    /// <summary>
    /// 現在のモードをもう一度プレイ
    /// </summary>
    public void RestartCurrent()
    {
        Debug.Log("Restarting...");
        RestartMode(PlayMode.EvE,true);
    }

    /// <summary>
    /// UIの初期化
    /// </summary>
    private void InitializeUI()
    {
        // コンポーネント
        _ui = _UIRef.GetComponent<ReversiUIManager>();
        _ui.Initialize();

        // UI有効化・更新
        _ui.Activate();
        _ui.HideResult();
        _ui.HidePassButton();
        _ui.HideUndoButton();
    }

    
    /// <summary>
    /// AIvsAIの初期化
    /// </summary>
    public void CreateGameOnlyAI()
    {
        Debug.Log("Created Game AI vs AI");

        // 定石ファイルを読み込み
        BookManager.Instance.LoadBookFile(_bookAssetRef);

        // AIをセット
        _player[BlackSide] = new AIPlayer(_blackAIDifficulty);
        _player[WhiteSide] = new AIPlayer(_whiteAIDifficulty);

        _currentPlayer = BlackSide;

        // UI初期化
        InitializeUI();

        // 盤面データの初期化
        _board.Init();
        _board.SetAIInitiative(!true);

        // 3D盤面の作成と初期化
        _3dboard.CreateBoard(_board);
        _3dboard.InitializeBoard(_board);

        // UI更新
        UpdateUI(PlayMode.EvE);

        // スタートメッセージを表示
        ShowMessage(PlayMode.EvE,$"Game started AI vs AI!");
    }

    /// <summary>
    /// 対AI戦の初期化
    /// </summary>
    /// <param name="_isInitiative">先手かどうか</param>
    public void InitializeGameOnlyAI()
    {
        // UI初期化
        InitializeUI();

        // 盤面データの初期化
        _board.Init();
        _board.SetAIInitiative(!true);

        // 3D盤面の作成と初期化
        _3dboard.CreateBoard(_board);
        _3dboard.InitializeBoard(_board);

        // UI更新
        UpdateUI(PlayMode.EvE);
        // スタートメッセージを表示
        ShowMessage(PlayMode.EvE,$"Game started AI vs AI!");
    }

    /// <summary>
    /// マスを選択し、思考を終了する
    /// </summary>
    /// <param name="point"></param>
    public void SelectPoint(Point point)
    {
        point._Log("Selected: ");
        _completedThinking = true;
        _selectedPoint = point;
    }

    /// <summary>
    /// アニメーション終了待ち
    /// </summary>
    /// <returns></returns>
    public async Task WaitAnimationCompleted()
    {
        while(_3dboard.IsAnimating())
        {
            // アニメーションが終了するのを待つ
            await Task.Delay(10);
        }
    } 

    /// <summary>
    /// Undoとして扱われるマス座標を返す。
    /// </summary>
    public void Undo()
    {
        SelectPoint(Point.Undone);
    }

    /// <summary>
    /// マスを選ばずに思考を終了する。Pass扱いになる。
    /// </summary>
    public void Pass()
    {
        SelectPoint(Point.Passed);
    }

    /// <summary>
    /// メッセージテキストを指定する。
    /// </summary>
    /// <param name="playMode"></param>
    /// <param name="content"></param>
    private void ShowMessage(PlayMode playMode, string content)
    {
        _ui.SetMessageText(content);
    }

    /// <summary>
    /// UIを更新する
    /// </summary>
    /// <param name="playMode"></param>
    private void UpdateUI(PlayMode playMode)
    {
        if(playMode != PlayMode.PvPNetwork)
        {
            _ui.SetTurnNumber(_board.GetCurrentTurn());
            _ui.SetBackgroundColor(_board.GetCurrentColor().ToColor());
            _ui.HidePassButton();
        }
        else
        {
            _ui.SetTurnNumber(_board.GetCurrentTurn());
            _ui.SetBackgroundColor(_board.GetCurrentColor().ToColor());
            _ui.SetTurnNumber(_board.GetCurrentTurn());
            _ui.SetBackgroundColor(_board.GetCurrentColor().ToColor());
            _ui.HidePassButton();
        }
    }

    /// <summary>
    /// 各手番での行動
    /// </summary>
    private void OnTurn()
    {    
        _movablePoints.Clear();
        _movablePoints = _board.GetMovablePoints();

        // メッセージリセット
        _ui.SetMessageText("");
        _ui.SetMessageText("");
        _ui.HideUndoButton();
        _ui.HideUndoButton();

        // メッセージ設定
        _ui.SetMessageText($"AI Thinking{_ui.ThinkSuffix}");
        StartWait();
    }

    /// <summary>
    /// 思考待機開始
    /// </summary>
    private async void StartWait()
    {
        // 非同期処理をCancelするためのTokenを取得
        _thinkCancelTokenSrc = new CancellationTokenSource();
        var thinkCancelToken = _thinkCancelTokenSrc.Token;

        // 別スレッドに移行
        await Task.Run(
        () => 
        {
            thinkCancelToken.ThrowIfCancellationRequested();    // キャンセル判定すると例外スロー
            _player[_currentPlayer].Think(_board,thinkCancelToken,_mainThread);
        }
        ,thinkCancelToken);

        // オブジェクト破棄後にコルーチン実行しないように
        if(!thinkCancelToken.IsCancellationRequested) StartCoroutine(WaitForSelect());
        else if(!_gameDestroyed) ImmediateAct();
    }

    /// <summary>
    /// プレイヤー行動待機
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForSelect()
    {
        yield return new WaitUntil(() => _completedThinking);
        Act(new Point(_selectedPoint));
    }

    /// <summary>
    /// 即時行動
    /// </summary>
    private void ImmediateAct()
    {
        Act(new Point(_selectedPoint));
    }

    /// <summary>
    /// 実際の行動と、行動に基づいた更新内容
    /// </summary>
    /// <param name="point"></param>
    private async void Act(Point point)
    {
        await Task.Run(() => WaitAnimationCompleted());

        IReversiPlayer.ActionResult result = _player[_currentPlayer].Act(_board,point);

        switch(result)
        {
        case IReversiPlayer.ActionResult.Placed:
            _3dboard.UpdateBoardOnPlace(_board.GetUpdate());
            SwapTurn();
            break;

        case IReversiPlayer.ActionResult.Passed:
            _ui.SetMessageText("Passed!");
            _ui.SetMessageText("Passed!");
            SwapTurn();
            break;

        case IReversiPlayer.ActionResult.Undone:
            _3dboard.UpdateBoardOnUndo(_board.GetUndone());
            
            await Task.Run(() => WaitAnimationCompleted());
            _player[_currentPlayer].Act(_board,point);
            _3dboard.UpdateBoardOnUndo(_board.GetUndone());
            SwapTurn();

            SwapTurn();
            break;

        case IReversiPlayer.ActionResult.Failed:
            Debug.Log("FAIL");
            EndTurn();
            break;
        }
    }

    /// <summary>
    /// 手番の終了
    /// </summary>
    private void EndTurn()
    {
        _completedThinking = false;
        _turnUpdated = true;
        UpdateUI(PlayMode.EvE);
        _3dboard.RemoveHighlight(_movablePoints);
        IsGameOver();
    }

    /// <summary>
    /// 手番入れ替え
    /// </summary>
    private void SwapTurn()
    {
        _currentPlayer = 1 - _currentPlayer;
        EndTurn();
    }

    /// <summary>
    /// 手番入れ替え時の処理
    /// </summary>
    private void IsGameOver()
    {
        if(!_board.IsGameOver()) return;

        _turnUpdated = false;
        _ui.ShowResult(_board);
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

    /// <summary>
    /// 終了処理
    /// </summary>
    private void Finalization()
    {
        if(_thinkCancelTokenSrc != null) _thinkCancelTokenSrc.Cancel();
        _gameDestroyed = true;
        StopAllCoroutines();
    }

    /// <summary>
    /// オブジェクト破棄時
    /// </summary>
    private void OnDestroy()
    {
        Finalization();
    }

    /// <summary>
    /// アプリケーション終了時
    /// </summary>
    private void OnApplicationQuit()
    {
        Finalization();
    }
}
