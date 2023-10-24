using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// 非同期処理を管理しやすくするためのクラス
/// </summary>
public class AsyncOperation
{
    /// <summary>
    /// 現在進行中の処理を保持するディクショナリ
    /// </summary>
    private static Dictionary<string,AsyncOperation> _onGoingOperations = new Dictionary<string, AsyncOperation>();

    /// <summary>
    /// 今まで非同期処理が作成された回数
    /// </summary>
    private static int _totalCreationAmount = 0;

    /// <summary>
    /// 進行中処理管理ID
    /// </summary>
    private int _operationID;
    public int OperationID => _operationID;
    /// <summary>
    /// 進行中処理管理名
    /// </summary>
    private string _operationName;
    public string Name => _operationName;
    /// <summary>
    /// タスク
    /// </summary>
    private Task _task;
    public Task Task => _task;
    /// <summary>
    /// 進行度
    /// </summary>
    private float _progress;
    private float Progress => _progress;
    /// <summary>
    /// 進行度の保持と管理用をするProgress
    /// </summary>
    private IProgress<float> _progressHolder;
    public IProgress<float> ProgressHolder => _progressHolder;
    /// <summary>
    /// キャンセルトークン
    /// </summary>
    private CancellationTokenSource _cancelTokenSrc;
    public CancellationTokenSource CancellationTokenSource => _cancelTokenSrc;

    /// <summary>
    /// 進捗更新時に実行されるAction
    /// </summary>
    private Action<float> _onProgressChanged;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancelTokenSrc"></param>
    /// <param name="progressHolder"></param>
    public AsyncOperation(int id,string name,Task task = null,CancellationTokenSource cancelTokenSrc = null,IProgress<float> progressHolder = null)
    {
        _operationID = id;
        _operationName = name;
        _task = task;
        _progress = 0.0f;
        _progressHolder = progressHolder;
        _cancelTokenSrc = cancelTokenSrc;
        Application.quitting += OnApplicationQuit;
    }

    /// <summary>
    /// 処理の進捗状況が通知されるたびに登録されたActionを実行する
    /// </summary>
    /// <param name="progress"></param>
    public void OnProgressChanged(float progress)
    {
        _progress = progress;
        _onProgressChanged.Invoke(progress);
    }

    /// <summary>
    /// 進捗状況更新時に実行されるActionを登録する
    /// </summary>
    /// <param name="action"></param>
    public void AddActionOnProgressChanged(Action<float> action)
    {
        _onProgressChanged += action;
    }

    /// <summary>
    /// 処理をキャンセルする
    /// </summary>
    public void Cancel()
    {
        _cancelTokenSrc.Cancel();
    }

    /// <summary>
    /// アプリケーション終了時に進行中タスクを破棄
    /// </summary>
    public void OnApplicationQuit()
    {
        Cancel();
    }
    
    /// <summary>
    /// 指定した処理の進行度を0.0f ~ 1.0fで返す、存在しない場合は-1.0f
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static float GetProgress(string key)
    {
        if (_onGoingOperations.ContainsKey(key))
        {
            return _onGoingOperations[key].Progress;
        }
        else
        {
            return -1.0f;
        }
    }

    /// <summary>
    /// 指定した処理の進行度を管理するIProgressを返す、存在しない場合はnull
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static IProgress<float> GetProgressHolder(string key)
    {
        if (_onGoingOperations.ContainsKey(key))
        {
            return _onGoingOperations[key].ProgressHolder;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 指定した処理に名前をつけて実行開始
    /// 既に同名の処理が実行されていた場合はなにもせずに処理を抜ける
    /// </summary>
    /// <param name="key"></param>
    /// <param name="asyncOperation"></param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    public static async Task ExcuteOperation(string key, Func<IProgress<float>,CancellationToken,Task> asyncOperation, Action onCompleted = null)
    {
        if(_onGoingOperations.ContainsKey(key))
        {
            // 既に実行中のタスクがある
            return;
        }

        // インスタンス作成
        AsyncOperation op = new AsyncOperation(_totalCreationAmount++,key);

        // Progressの割り当て
        var progress = new Progress<float>(op.OnProgressChanged);
        // キャンセルトークン生成
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        // タスクの作成
        Task task = asyncOperation(progress,cancellationToken);

        // インスタンスに代入
        op._progressHolder = progress;
        op._cancelTokenSrc = cancellationTokenSource;
        op._task = task;

        // ディクショナリに追加
        _onGoingOperations.Add(key, op);

        // 実行
        try
        {
            // 終了待機
            await task;
        }
        catch (Exception ex)
        {
            // 例外ハンドリング
            Debug.LogError($"Error in async operation: {ex.Message}");
        }
        finally
        {
            // 終了
            _onGoingOperations.Remove(key);
            onCompleted?.Invoke();
        }
    }

    /// <summary>
    /// 指定した処理の進捗更新時関数にActionを追加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void AddActionOnProgressChanged(string key,Action<float> action)
    {
        _onGoingOperations[key].AddActionOnProgressChanged(action);
    }

    /// <summary>
    /// 指定の処理を中断
    /// </summary>
    /// <param name="key"></param>
    public static void CancelOperation(string key)
    {
        if (_onGoingOperations.TryGetValue(key, out var operation))
        {
            operation.Cancel();
        }
    }

    /// <summary>
    /// すべての処理を中断
    /// </summary>
    public static void CancelAllOperation()
    {
        foreach(KeyValuePair<string,AsyncOperation> operationPair in _onGoingOperations)
        {
            operationPair.Value.Cancel();
        }
    }
}
