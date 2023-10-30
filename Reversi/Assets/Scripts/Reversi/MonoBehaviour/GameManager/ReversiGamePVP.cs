using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Reversi;
using T0R1;


public class ReversiGamePVP : MonoSingleton<ReversiGamePVP>
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

    [SerializeField]
    private GameSceneController _controller;

    /// <summary>
    /// 3D盤面オブジェクトのマネージャ
    /// </summary>
    [SerializeField]
    private ReversiBoard3D _3dboard;

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
    private GameReversiUI _ui;

    /// <summary>
    /// 現在の手番
    /// </summary>
    private int _currentPlayer = 0;

    /// <summary>
    /// プレイヤー側を表す
    /// </summary>
    private int _clientSide = -1;

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
        InitializeUI();
    }

    /// <summary>
    /// インスタンス破棄時の処理
    /// </summary>
    public override void OnFinalize()
    {
        StopAllCoroutines();
        Destroy(_3dboard);
    }

    public void OnPlayerTurn()
    {

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
                return;
            }
            else if(_board.IsPassable())
            {
                _ui.SetMessageText($"{CurrentPlayer} Thinking");
            }
        }
    }

    public void StopThink()
    {
        _thinkCancelTokenSrc.Cancel();
    }

    /// <summary>
    /// 指定したゲームモードを始める
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="isInitiative"></param>
    public void StartMode(int startTurn)
    {
        _turnUpdated = true;
        CreateGame(startTurn);
    }
    
    /// <summary>
    /// 現在のゲームモードをやり直す
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="isInitiative"></param>
    public void RestartMode(int startTurn)
    {
        _turnUpdated = true;
        InitializeGame(startTurn);
    }

    /// <summary>
    /// 現在のモードをもう一度プレイ
    /// </summary>
    public void RestartCurrent()
    {
        Debug.Log("Restarting...");
        RestartMode(_clientSide);
    }

    /// <summary>
    /// UIの初期化
    /// </summary>
    private void InitializeUI()
    {
        _ui = _controller.SceneUI.ReversiPanel;
    }

    /// <summary>
    /// 対人戦の作成
    /// </summary>
    public void CreateGame(int startTurn)
    {
        // プレイヤーをセット
        {
            _player[BlackSide] = new HumanPlayer();
            _player[WhiteSide] = new HumanPlayer();
        }

        // UI初期化
        _ui.Initialize();

        // 盤面データの初期化
        _board.Init();

        // 3D盤面の作成と初期化
        _3dboard.CreateBoard(_board);
        _3dboard.InitializeBoard(_board);

        _clientSide = startTurn;

        ShowMessage($"ClientSide:{_clientSide}");

        _currentPlayer = BlackSide;

        UpdateUI();
        // スタートメッセージを表示
    }

    /// <summary>
    /// 対人戦の初期化
    /// </summary>
    public void InitializeGame(int startTurn)
    {
        // UI初期化
        _ui.Initialize();

        // 盤面データの初期化
        _board.Init();
        // 3D盤面の初期化
        _3dboard.InitializeBoard(_board);

        _clientSide = startTurn;

        _currentPlayer = BlackSide;

        // UI更新
        UpdateUI();
    }

    /// <summary>
    /// マスを選択し、思考を終了する
    /// </summary>
    /// <param name="point"></param>
    public void SelectPoint(Point point)
    {
        if(_currentPlayer != _clientSide) return;
        point._Log("Selected: ");
        _completedThinking = true;
        _selectedPoint = point;
        _controller.SendSFMessage(point.ToStrCoord());
    }

    /// <summary>
    /// マスを選択し、思考を終了する
    /// </summary>
    /// <param name="point"></param>
    public void ReceivePoint(Point point)
    {
        point._Log("Received: ");
        _completedThinking = true;
        _selectedPoint = point;
    }

    public void OnCallBack()
    {
        _completedThinking = true;
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
        if(_currentPlayer != _clientSide) return;
        SelectPoint(Point.Undone);
    }

    /// <summary>
    /// マスを選ばずに思考を終了する。Pass扱いになる。
    /// </summary>
    public void Pass()
    {
        if(_currentPlayer != _clientSide) return;
        SelectPoint(Point.Passed);
    }

    /// <summary>
    /// メッセージテキストを指定する。
    /// </summary>
    /// <param name="playMode"></param>
    /// <param name="content"></param>
    private void ShowMessage(string content)
    {
        _ui.SetMessageText(content);
    }

    /// <summary>
    /// UIを更新する
    /// </summary>
    /// <param name="playMode"></param>
    private void UpdateUI()
    {
        _ui.SetTurnNumber(_board.GetCurrentTurn());
        _ui.HidePassButton();
    }

    /// <summary>
    /// 各手番での行動
    /// </summary>
    private void OnTurn()
    {
        _movablePoints.Clear();
        _movablePoints = _board.GetMovablePoints();

        if(_currentPlayer == _clientSide)
        {
            if(_board.IsPassable())
            {
                _ui.ShowPassButton();
                _ui.ShowUndoButton();
                StartWait();
            }
            else
            {
                _ui.SetMessageText($"Decide your move.");
                _ui.SetMessageText($"{CurrentPlayer}");
                _ui.ShowUndoButton();
                _3dboard.HighlightMovable(_movablePoints,_board.GetCurrentColor());
                StartWait();
            }
        }
        else
        {
            _3dboard.RemoveHighlight(_movablePoints);
            _ui.SetMessageText($"Your Opponent is thinking.");
            StartWait();
        }

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
            SwapTurn();
            break;

        case IReversiPlayer.ActionResult.Undone:
            _3dboard.UpdateBoardOnUndo(_board.GetUndone());
            await Task.Run(() => WaitAnimationCompleted());
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
        _controller.SendCallBack();
        _completedThinking = false;
        _turnUpdated = true;
        UpdateUI();
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
