using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading;

public class AsyncOperation
{
    private static Dictionary<string,AsyncOperation> _onGoingOperations = new Dictionary<string, AsyncOperation>();

    private Task _task;
    public Task Task => _task;
    private IProgress<float> _progress;
    public IProgress<float> Progress;
    private CancellationTokenSource _cancelTokenSrc;
    public CancellationTokenSource CancellationTokenSource => _cancelTokenSrc;

    public event Action<float> OnProgressUpdated;
    
    public AsyncOperation(Task task,CancellationTokenSource cancelTokenSrc,IProgress<float> progressHolder)
    {
        _task = task;
        _progress = progressHolder;
        _cancelTokenSrc = cancelTokenSrc;
    }

    /// <summary>
    /// 指定した処理の進行度を取得. 存在しない場合はnullが返る
    /// </summary>
    /// <param name="key"></param>
    /// <param name="onProgressChanged"></param>
    /// <returns></returns>
    public static IProgress<float> GetProgress(string key,Action onProgressChanged = null)
    {
        if (_onGoingOperations.ContainsKey(key))
        {
            return _onGoingOperations[key].Progress;
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
    /// <param name="acceptDuplicate"></param>
    /// <returns></returns>
    public static async Task ExcuteOperation(string key, Func<IProgress<float>,CancellationToken,Task> asyncOperation, Action onCompleted = null)
    {
        if(_onGoingOperations.ContainsKey(key))
        {
            // 既に実行中のタスクがある
            return;
        }

        var progress = new Progress<float>();
        progress.ProgressChanged += (sender, value) =>
        {
            // 進行度が変化したときにイベントを発火し、外部から登録された関数を呼び出す
            _onGoingOperations[key].OnProgressUpdated?.Invoke(value);
        };
        _onGoingOperations[key].Progress = progress;
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        Task task = asyncOperation(progress,cancellationToken);

        _onGoingOperations.Add(key, new AsyncOperation(task,cancellationTokenSource,progress));

        try
        {
            await task;
        }
        catch (Exception ex)
        {
            // 非同期処理の例外ハンドリング
            Debug.LogError($"Error in async operation: {ex.Message}");
        }
        finally
        {
            _onGoingOperations.Remove(key);
            onCompleted?.Invoke();
        }
    }

    /// <summary>
    /// 指定の処理を中断
    /// </summary>
    /// <param name="key"></param>
    public static void CancelOperation(string key)
    {
        if (_onGoingOperations.TryGetValue(key, out var operation))
        {
            operation.CancellationTokenSource.Cancel();
        }
    }
}
