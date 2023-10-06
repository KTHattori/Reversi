using UnityEngine;
using Interpolation;
using System;

[CreateAssetMenu(fileName = "New Reversi Board Settings",menuName = "ScriptableObjects/Reversi/Board Settings",order = 1)]
public class ReversiBoardSettings : ScriptableObject
{
    [field:SerializeField] public Vector3 PositionOrigin{ get; private set; } = new Vector3(-5.0f,0.0f,-5.0f);
    [field: SerializeField] public float AnimationDelay{ get; private set; } = 0.1f;

}
