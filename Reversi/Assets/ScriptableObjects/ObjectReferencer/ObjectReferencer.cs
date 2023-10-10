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
}
