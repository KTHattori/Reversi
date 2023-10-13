using UnityEngine;

/// <summary>
/// リバーシのAI難易度設定
/// </summary>
[CreateAssetMenu(fileName = "New Reversi AI Difficulty",menuName = "ScriptableObjects/Reversi/AI Difficulty",order = 2)]
public class ReversiAIDifficulty : ScriptableObject
{
    /// <summary>
    /// 難易度の識別名
    /// </summary>
    [field:SerializeField]
    public string DifficultyName{ get; private set; } = "Diffuculty";
    /// <summary>
    /// 事前探索を行う際の先読みする手数
    /// </summary>
    [field:SerializeField]
    public int PresearchDepth{ get; private set; } = 3;

    /// <summary>
    /// 序盤・中盤の先読み手数
    /// </summary>
    [field:SerializeField]
    public int NormalDepth{ get; private set; } = 5;

    /// <summary>
    /// 終盤での必勝読みを始める残り手数
    /// </summary>
    [field:SerializeField]
    public int WLDDepth{ get; private set; } = 15;

    /// <summary>
    /// 終盤において完全読みを始める残り手数
    /// </summary>
    [field:SerializeField]
    public int PerfectDepth{ get; private set; } = 13;

}
