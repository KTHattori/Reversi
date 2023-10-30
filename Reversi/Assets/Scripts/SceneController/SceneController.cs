using UnityEngine;
using T0R1;
using T0R1.UI;
using UnityEngine.SceneManagement;

public abstract class SceneController : SealableMonoBehaviour,ISFEventTriggerable
{
    protected SFClientManager _sfManager;
    
    protected void TransitScene(string sceneName)
    {
        LoadScreen.Show();
        SceneManager.LoadScene(sceneName);
    }

    protected void OnSceneLoaded()
    {
        LoadScreen.Hide();
    }

    protected sealed override void Reset()
    {
        OnReset();
    }
    
    protected sealed override void Awake()
    {
        _sfManager = SFClientManager.Instance;
        OnAwake();
    }

    protected sealed override void Start()
    {
        OnSceneLoaded();
        OnStart();
    }

    protected sealed override void Update()
    {
        OnUpdate();
    }

    protected virtual void OnReset()
    {

    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnUpdate()
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
