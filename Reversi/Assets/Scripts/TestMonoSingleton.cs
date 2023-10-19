using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonoSingleton : MonoBehaviour
{
    private static TestMonoSingleton instance;
    [SerializeField]
    private int num = 0;
    public static TestMonoSingleton Instance
    {
        get
        {
            if(instance != null)    // インスタンスが存在する
            {
                return instance;
            }
            else
            {
                // ほかのインスタンスが存在するかどうか
                TestMonoSingleton find = FindFirstObjectByType(typeof(TestMonoSingleton),FindObjectsInactive.Include) as TestMonoSingleton;
                
                if( find == null )
                {   // 見つからない場合、新しく生成
                    GameObject obj = new GameObject("Test");
                    instance = obj.AddComponent<TestMonoSingleton>();
                }
                else
                {   // みつかったので、チェック
                    Initialize(find);
                }

                return instance;
            }
        }
    }

    private static void Initialize(TestMonoSingleton check)
    {
        if(instance == null)        // まだ変数がセットされたなかった場合
        {
            instance = check;
        }
        else if(check != instance)  // 重複があった場合
        {
            DestroyImmediate( check.gameObject );
        }
    }

    private void Awake()
    {
        Initialize(this);
    }

    public static TestMonoSingleton GetInstance()
    {
        return instance;
    }

    public int GetNum()
    {
        return num;
    }

    public void AddNum()
    {
        num++;
    }
}
