using Sfs2X.Core;

/// <summary>
/// イベントをトリガーできるインタフェース クラス
/// </summary>
public interface ISFEventTriggerable
{
    /// <summary>
    /// イベントを割り当てる
    /// </summary>
    public abstract void AddSFListeners();
    /// <summary>
    /// 割り当てたイベントを解除する
    /// </summary>
    public abstract void RemoveSFListeners();
}

/// <summary>
/// 
/// </summary>
public interface ISFConnectable : ISFEventTriggerable
{
    /// <summary>
    /// サーバーへの接続成功時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFConnection(BaseEvent evt);
    /// <summary>
    /// サーバーへの接続失敗時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFConnectionLost(BaseEvent evt);
}

/// <summary>
/// 
/// </summary>
public interface ISFLoginable : ISFEventTriggerable
{
    /// <summary>
    /// ログイン成功時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFLogin(BaseEvent evt);
    /// <summary>
    /// ログイン失敗時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFLoginError(BaseEvent evt);
}