/// MonoSingleton.cs
using UnityEngine;
using System.Threading;

/**
<summary>
シングルトン化した MonoBehavior 抽象クラス
dontDestroyOnLoadの部分と、各コメント以外は下記サイトを引用
参考：https://caitsithware.com/wordpress/archives/118
</summary>
*/
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    /// <summary>
    /// インスタンス
    /// </summary>
    static T m_Instance = null;

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
            if( m_Instance != null )
            {   // インスタンスが存在していれば取得
                return m_Instance;
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
            return m_Instance;
        }
    }
 
    /// <summary>
    /// 初期化関数
    /// </summary>
    /// <param name="instance"></param>
    static void Initialize(T instance)
    {
        if( m_Instance == null )
        {   // メンバインスタンスが空なら代入し、インスタンス初期化時関数をコール
            m_Instance = instance;

            if(m_Instance.dontDestroyOnLoad)
            {   // dontDestroyOnLoadが有効
                DontDestroyOnLoad(m_Instance);
            }

            m_Instance.OnInitialize();  // インスタンス初期化時関数
        }
        else if( m_Instance != instance )
        {
            // オブジェクトが見つかったが重複している場合、破棄
            DestroyImmediate( instance.gameObject );
        }
    }
 
    /// <summary>
    /// 破棄時の処理
    /// </summary>
    /// <param name="instance"></param>
    static void Destroyed(T instance)
    {
        if( m_Instance == instance )
        {   // 正しいインスタンスであればインスタンス破棄時関数をコール
            m_Instance.OnFinalize();
 
            m_Instance = null;
        }
    }
 
    /// <summary>
    /// インスタンス初期化時にコールされる関数
    /// </summary>
    protected virtual void OnInitialize() {}

    /// <summary>
    /// インスタンス破棄時にコールされる関数
    /// </summary>
    protected virtual void OnFinalize() {}
 
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