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
                {   // 見つからない場合
                    GameObject obj = new GameObject("Test");
                    instance = obj.AddComponent<TestMonoSingleton>();
                }
                else
                {
                    Initialize(find);
                }

                return instance;
            }
        }
    }

    private static void Initialize(TestMonoSingleton check)
    {
        if(instance == null)
        {
            instance = check;
        }
        else if(check != instance)
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
