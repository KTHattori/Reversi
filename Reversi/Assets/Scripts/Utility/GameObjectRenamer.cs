using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectRenamer : MonoBehaviour
{
    [SerializeField]
    private string prefix;

    [SerializeField]
    private string suffix;

    [SerializeField]
    private string rename;

    [SerializeField]
    private List<Transform> targetObjects = new List<Transform>();

    void Reset()
    {
        FetchObjects();
    }

    [ContextMenu("Fetch Objects in Hierarchy Tree")]
    public void FetchObjects()
    {
        targetObjects.Clear();
        targetObjects.AddRange(GetComponentsInChildren<Transform>());
    }

    [ContextMenu("Add Prefix")]
    public void AddPrefix()
    {
        int count = 0;
        foreach(Transform transform in targetObjects)
        {
            transform.gameObject.name = prefix + transform.gameObject.name;
            count++;
        }

        Debug.Log($"Added prefix to {count} object(s).");
    }

    [ContextMenu("Add Suffix")]
    public void AddSuffix()
    {
        int count = 0;
        foreach(Transform transform in targetObjects)
        {
            transform.gameObject.name = transform.gameObject.name + suffix;
            count++;
        }

        Debug.Log($"Added suffix to {count} object(s).");
    }

    [ContextMenu("Rename")]
    public void Rename()
    {
        int count = 0;
        foreach(Transform transform in targetObjects)
        {
            transform.gameObject.Rename(rename);
            count++;
        }

        Debug.Log($"Renamed {count} object(s).");
    }
}
