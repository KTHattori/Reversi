/// <summary>
/// MonoSingleton用のメソッド群
/// </summary>
public interface IMonoSingletonMethod
{
    /// <summary>
    /// 初期化時にコールされる関数
    /// </summary>
    public abstract void OnInitialize();
    /// <summary>
    /// 終了時（破棄時）にコールされる関数
    /// </summary>
    public abstract void OnFinalize();
}
