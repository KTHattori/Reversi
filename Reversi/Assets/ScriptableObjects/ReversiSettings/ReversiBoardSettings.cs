using UnityEngine;

/// <summary>
/// リバーシのボード共通設定
/// </summary>
[CreateAssetMenu(fileName = "New Reversi Board Settings",menuName = "ScriptableObjects/Reversi/Board Settings",order = 1)]
public class ReversiBoardSettings : ScriptableObject
{
    /// <summary>
    /// 石生成時の座標原点
    /// </summary>
    [field:SerializeField]
    public Vector3 PositionOrigin{ get; private set; } = new Vector3(-5.0f,0.0f,-5.0f);

    /// <summary>
    /// アニメーション時の石ごとの遅延間隔
    /// </summary>
    [field: SerializeField]
    public float AnimationDelay{ get; private set; } = 0.1f;

}
