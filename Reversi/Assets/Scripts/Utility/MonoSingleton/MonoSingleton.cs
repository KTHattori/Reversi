/// MonoSingleton.cs
using UnityEngine;

/**
<summary>
シングルトン化した MonoBehavior 抽象クラス
dontDestroyOnLoadの部分と、各コメント以外は下記サイトを引用
参考：https://caitsithware.com/wordpress/archives/118
</summary>
*/
public abstract class MonoSingleton<T> : MonoBehaviour, IMonoSingletonMethod where T : MonoSingleton<T>
{
    /// <summary>
    /// インスタンス
    /// </summary>
    static T _instance = null;

    /// <summary>
    /// シーンロード時に破棄しないようにする
    /// </summary>
    [SerializeField]
    protected bool dontDestroyOnLoad;
 
    /// <summary>
    /// インスタンスを取得するプロパティ
    /// 存在しなければ新たに生成
    /// </summary>
    public static T Instance
    {
        // ゲッター
        get
        {
            if( _instance != null )
            {   // インスタンスが存在していれば取得
                return _instance;
            }

            // 型を取得
            System.Type type = typeof(T);

            // T型のMonoBehaviorスクリプトがアタッチされたオブジェクトが存在するかチェック
            T instance = FindFirstObjectByType(type,FindObjectsInactive.Include) as T;
            if( instance == null )
            {   // 見つからない場合
                string typeName = type.ToString();

                // 新たにこのスクリプトがアタッチされたオブジェクトを生成
                GameObject gameObject = new GameObject( typeName, type );
                instance = gameObject.GetComponent<T>();

                if( instance == null )
                {   // それでも失敗した場合エラーログを残す
                    Debug.LogError(typeName + " インスタンスの生成に失敗しました。\nProblem during the creation of " + typeName,gameObject );
                }
            }
            else
            {   // 初期化を行う
                Initialize(instance);
            }
            return _instance;
        }
    }
 
    /// <summary>
    /// 初期化関数
    /// </summary>
    /// <param name="instance"></param>
    static void Initialize(T instance)
    {
        if( _instance == null )
        {   // メンバインスタンスが空なら代入し、インスタンス初期化時関数をコール
            _instance = instance;

            if(_instance.dontDestroyOnLoad)
            {   // dontDestroyOnLoadが有効
                DontDestroyOnLoad(_instance);
            }

            _instance.OnInitialize();  // インスタンス初期化時関数
        }
        else if( _instance != instance )
        {
            // オブジェクトが見つかったが重複している場合、破棄
            DestroyImmediate( instance.gameObject );
        }
    }

    /// <summary>
    /// インスタンスを破棄する
    /// 破棄はループ最後に行われる。
    /// </summary>
    static public void ReleaseInstance()
    {
        Destroy(_instance.gameObject);
    }
 
    /// <summary>
    /// 破棄時の処理
    /// </summary>
    /// <param name="instance"></param>
    static void Destroyed(T instance)
    {
        if( _instance == instance )
        {   // 正しいインスタンスであればインスタンス破棄時関数をコール
            _instance.OnFinalize();
 
            _instance = null;
        }
    }

    // interface methods
    public abstract void OnInitialize();
    public abstract void OnFinalize();

 
    /// <summary>
    /// ループ初期にコール
    /// </summary>
    private void Awake()
    {
        // 初期化
        Initialize( this as T );
    }
 
    /// <summary>
    /// オブジェクト破棄時
    /// </summary>
    private void OnDestroy()
    {
        // オブジェクトが破棄された場合の処理
        Destroyed( this as T );
    }
 
    /// <summary>
    /// アプリケーション終了時
    /// </summary>
    private void OnApplicationQuit()
    {
        // アプリケーションが終了した場合も破棄時の処理
        Destroyed( this as T );
    }
}