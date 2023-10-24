using UnityEngine;

public abstract class SceneController : MonoSingleton<SceneController>
{
    private SFSClientManager sfsClient;

    /// <summary>
    /// インスタンスをセット
    /// </summary>
    void Awake()
    {
        sfsClient = SFSClientManager.Instance;
    }

    protected virtual void Update()
	{
		// Escapeキーが押されたら、モーダルなUIを非表示にする
		if (Input.GetKeyDown(KeyCode.Escape))
			HideModals();
	}

    /// <summary>
    /// シーン変更時にSFSに登録したイベントリスナーを破棄
    /// </summary>
	protected virtual void OnDestroy()
	{
		// Remove SFS2X listeners
		RemoveSFSListeners();
	}

	/// <summary>
    /// SFSに登録したイベントリスナーを破棄
    /// </summary>
	protected abstract void RemoveSFSListeners();

    /// <summary>
    /// モーダルなUIを非表示にする
    /// </summary>
	protected abstract void HideModals();
}
