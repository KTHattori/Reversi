using UnityEngine;
using T0R1;

public abstract class SceneController : MonoSingleton<SceneController>,ISFEventTriggerable
{
    protected SFClientManager _sfManager;

    /// <summary>
    /// インスタンスをセット
    /// </summary>
    void Awake()
    {
        _sfManager = SFClientManager.Instance;
    }

    protected virtual void Update()
	{

	}

    /// <summary>
    /// シーン変更時にSFSに登録したイベントリスナーを破棄
    /// </summary>
	protected virtual void OnDestroy()
	{
		// Remove SFS2X listeners
		RemoveSFListeners();
	}

    /// <summary>
    /// イベントを割り当てる
    /// </summary>
    public abstract void AddSFListeners();

    /// <summary>
    /// 割り当てたイベントを解除する
    /// </summary>
    public abstract void RemoveSFListeners();
}
