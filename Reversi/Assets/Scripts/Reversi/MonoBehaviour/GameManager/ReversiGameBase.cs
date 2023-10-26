using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Reversi;
using T0R1;


public abstract class ReversiGameBase : MonoSingleton<ReversiGameBase>
{
    // 定数 Constants

    /// <summary>
    /// 先手を表す定数
    /// </summary>
    protected const int Initiative = 0;
    /// <summary>
    /// 黒（先手）
    /// </summary>
    protected const int BlackSide = Initiative;
    /// <summary>
    /// 白（後手）
    /// </summary>
    protected const int WhiteSide = Initiative + 1;

    /// <summary>
    /// ボード内部データ
    /// </summary>
    protected static Board _board = null;

    /// <summary>
    /// 3D盤面オブジェクトのマネージャ
    /// </summary>
    [SerializeField]
    protected ReversiBoard3D _3dboard;

    /// <summary>
    /// プレイヤー0用UIへの参照
    /// </summary>
    [SerializeField]
    protected ObjectReferencer _playerUIRef;

    /// <summary>
    /// 定石ファイルへの参照
    /// </summary>
    [SerializeField]
    protected AssetReference _bookAssetRef;


    // Private: Non-Serialized

    /// <summary>
    /// 配置可能なマスを保持するリスト
    /// </summary>
    protected List<Point> _movablePoints = new List<Point>();

    /// <summary>
    /// プレイヤーの枠
    /// </summary>
    protected IReversiPlayer[] _player = new IReversiPlayer[2];

    /// <summary>
    /// UIマネージャ
    /// </summary>
    protected ReversiUIManager _ui = new ReversiUIManager();

    /// <summary>
    /// 現在の手番
    /// </summary>
    protected int _currentPlayer = 0;

    /// <summary>
    /// プレイヤー側を表す
    /// </summary>
    protected int _playerSide = -1;

    /// <summary>
    /// 思考完了フラグ
    /// </summary>
    protected bool _completedThinking = false;

    /// <summary>
    /// 選択されたマス
    /// </summary>
    protected Point _selectedPoint = null;

    /// <summary>
    /// メインスレッドへの参照
    /// </summary>
    protected SynchronizationContext _mainThread;

    /// <summary>
    /// ターン更新フラグ
    /// </summary>
    protected bool _turnUpdated = false;

    /// <summary>
    /// タスクキャンセルのトークン元
    /// </summary>
    protected CancellationTokenSource _thinkCancelTokenSrc = null;

    /// <summary>
    /// ターンでの経過時間
    /// </summary>
    protected float _turnTime = 0.0f;

    /// <summary>
    /// ゲーム終了フラグ
    /// </summary>
    protected bool _gameDestroyed = false;

    /// <summary>
    /// 現在の手番を取得するプロパティ
    /// </summary>
    public int CurrentPlayer => _currentPlayer;

    /// <summary>
    /// 現在と反対の手番を取得するプロパティ
    /// </summary>
    public int OppositePlayer => _currentPlayer == BlackSide ? WhiteSide : BlackSide;

    protected void Initialization()
    {
        _board = new Board();
        _selectedPoint = null;
        _mainThread = SynchronizationContext.Current;
        _gameDestroyed = false;
    }

    protected void Finalization()
    {
        if(_thinkCancelTokenSrc != null) _thinkCancelTokenSrc.Cancel();
        _gameDestroyed = true;
        StopAllCoroutines();
        Destroy(_3dboard);
    }

    /// <summary>
    /// インスタンス生成時の処理
    /// </summary>
    public override void OnInitialize()
    {
        Initialization();
    }

    /// <summary>
    /// インスタンス破棄時の処理
    /// </summary>
    public override void OnFinalize()
    {
        Finalization();
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
            UpdateContent();
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
    public abstract void StartMode();
    
    /// <summary>
    /// 現在のゲームモードをやり直す
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="isInitiative"></param>
    public abstract void RestartMode();

    /// <summary>
    /// Updateで実行される内容
    /// </summary>
    protected abstract void UpdateContent();

    
    protected abstract void OnPlaced();
    protected abstract void OnPassed();
    protected abstract void OnUndone();
    protected abstract void OnFailed();
    protected abstract void OnGameOver();

    /// <summary>
    /// UIの初期化
    /// </summary>
    private void InitializeUI()
    {
        _ui = _playerUIRef.GetComponent<ReversiUIManager>();
        _ui.Initialize();
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
    protected void ShowMessage(string content)
    {
        _ui.SetMessageText(content);
    }

    /// <summary>
    /// UIを更新する
    /// </summary>
    protected abstract void UpdateUI();

    protected abstract void OnTurnContent();

    /// <summary>
    /// 各手番での行動
    /// </summary>
    private void OnTurn()
    {    
        _movablePoints.Clear();
        _movablePoints = _board.GetMovablePoints();

        // メッセージリセット
        _ui.SetMessageText("");
        _ui.HideUndoButton();

        OnTurnContent();
    }

    /// <summary>
    /// 思考待機開始
    /// </summary>
    protected async void StartWait()
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
    protected IEnumerator WaitForSelect()
    {
        yield return new WaitUntil(() => _completedThinking);
        Act(new Point(_selectedPoint));
    }

    /// <summary>
    /// 即時行動
    /// </summary>
    protected void ImmediateAct()
    {
        Act(new Point(_selectedPoint));
    }

    /// <summary>
    /// 実際の行動と、行動に基づいた更新内容
    /// </summary>
    /// <param name="point"></param>
    protected async void Act(Point point)
    {
        await Task.Run(() => WaitAnimationCompleted());

        IReversiPlayer.ActionResult result = _player[_currentPlayer].Act(_board,point);

        switch(result)
        {
        case IReversiPlayer.ActionResult.Placed:
            OnPlaced();
            break;

        case IReversiPlayer.ActionResult.Passed:
            OnPassed();
            break;

        case IReversiPlayer.ActionResult.Undone:
            OnUndone();
            break;

        case IReversiPlayer.ActionResult.Failed:
            OnFailed();
            break;
        }
    }

    /// <summary>
    /// 手番の終了
    /// </summary>
    protected void EndTurn()
    {
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
    /// 評価値を指定したマスに表示
    /// </summary>
    /// <param name="point"></param>
    /// <param name="score"></param>
    public void DisplayEvalScore(Point point,int score)
    {
        // _3dboard.DisplayEvalScore(point,score);
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
