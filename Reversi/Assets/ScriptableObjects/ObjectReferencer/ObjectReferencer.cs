using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RegisterObject.RegisterToReferencer)), CreateAssetMenu(fileName = "Ref_ObjectName", menuName = "ScriptableObjects/Object Referencer")]
public class ObjectReferencer : ScriptableObject
{
    /// <summary>
    /// 登録されたゲームオブジェクト
    /// </summary>
    [field: SerializeField]
    public GameObject gameObject{get;set;}

    /// <summary>
    /// オブジェクトをアクティブにする
    /// </summary>
    public void ActivateObject()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// オブジェクトを非アクティブにする
    /// </summary>
    public void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// gameObject.transformを取得する
    /// </summary>
    public Transform transform{ get {return gameObject.transform;} }

    /// <summary>
    /// gameObject.GetComponent<T>()を実行する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetComponent<T>()
    {
        return gameObject.GetComponent<T>();
    }

    /// <summary>
    /// 任意のリファレンサから指定したクラスのコンポーネントの取得を試み、targetVarへ格納する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="referencer"></param>
    /// <param name="targetVar"></param>
    static public void GetComponentReference<T>(ObjectReferencer referencer,ref T targetVar)
    {
        if(!referencer) Debug.LogError($"Referencer object not set for variable: {nameof(referencer)!}");
        if(!referencer.gameObject.TryGetComponent<T>(out targetVar))
        {
            // 型を取得
            string typeName = nameof(T);
            Debug.LogError($"Failed to get component: {typeName}!");
        }
    }
}
