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
/// SmartFoxサーバー接続用インタフェース
/// </summary>
public interface ISFConnectable : ISFEventTriggerable
{
    /// <summary>
    /// サーバーへの接続時
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
/// SmartFoxサーバーログイン用インタフェース
/// </summary>
public interface ISFLoginable : ISFEventTriggerable
{
    /// <summary>
    /// ログイン時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFLogin(BaseEvent evt);
    /// <summary>
    /// ログイン失敗時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFLoginError(BaseEvent evt);
}

/// <summary>
/// SmartFoxルーム閲覧用インターフェス
/// </summary>
public interface ISFRoomFetchable : ISFEventTriggerable
{
    /// <summary>
    /// ルーム追加時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFRoomAdded(BaseEvent evt);
    /// <summary>
    /// ルーム削除時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFRoomRemoved(BaseEvent evt);
}

/// <summary>
/// SmartFoxルーム管理用インタフェース
/// </summary>
public interface ISFRoomCreatable : ISFRoomFetchable
{
    /// <summary>
    /// ルーム作成失敗時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFRoomCreationError(BaseEvent evt);
}

/// <summary>
/// SmartFoxルーム参加用インタフェース
/// </summary>
public interface ISFRoomJoinable : ISFRoomFetchable
{
    /// <summary>
    /// ルーム参加時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFRoomJoin(BaseEvent evt);
    /// <summary>
    /// ルーム参加失敗時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFRoomJoinError(BaseEvent evt);
}

/// <summary>
/// SmartFoxルーム参加監視用インタフェース
/// </summary>
public interface ISFRoomAccessWatchable : ISFEventTriggerable
{
    /// <summary>
    /// ルーム参加時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFUserEnterRoom(BaseEvent evt);
    /// <summary>
    /// ルーム退出時
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFUserExitRoom(BaseEvent evt);
}

public interface ISFMessageReceiver : ISFEventTriggerable
{
    /// <summary>
    /// メッセージ受信時（自分のものも含む）
    /// </summary>
    /// <param name="evt"></param>
    public abstract void OnSFMessageReceived(BaseEvent evt);
}