using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RegisterObject.RegisterToReferencer)), CreateAssetMenu(fileName = "Ref_ObjectName", menuName = "ScriptableObjects/Object Referencer")]
public class ObjectReferencer : ScriptableObject
{
    [field: SerializeField]
    public GameObject gameObject{get;set;}

    public void ActivateObject()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    public Transform transform{ get {return gameObject.transform;} }

    public T GetComponent<T>()
    {
        return gameObject.GetComponent<T>();
    }
}
