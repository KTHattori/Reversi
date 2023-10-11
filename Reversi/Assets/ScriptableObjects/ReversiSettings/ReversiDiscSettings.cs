using UnityEngine;
using Interpolation;

/// <summary>
/// リバーシの石共通設定
/// </summary>
[CreateAssetMenu(fileName = "New Reversi Disc Settings",menuName = "ScriptableObjects/Reversi/Disc Settings",order = 0)]
public class ReversiDiscSettings : ScriptableObject
{
    /// <summary>
    /// 石をひっくり返すときに関係した設定項目
    /// </summary>
    [System.Serializable]
    public class DiscFlip
    {
        /// <summary>
        /// アニメーション時間
        /// </summary>
        [field:SerializeField]
        public float AnimationTime{ get; private set; } = 1.0f;

        /// <summary>
        /// 跳ねるアニメーションのイージングカーブ
        /// </summary>
        [field:SerializeField]
        public Easing.Curve HopEase{ get; private set; } = Easing.Curve.EaseInOutCirc;

        /// <summary>
        /// ひっくり返るアニメーションのイージングカーブ
        /// </summary>
        [field:SerializeField]
        public Easing.Curve FlipEase{ get; private set; } = Easing.Curve.EaseOutCirc;
    }

    /// <summary>
    /// ひっくり返る際の設定
    /// </summary>
    [field:SerializeField]
    public DiscFlip DiscFlipSettings{ get; private set;} = new DiscFlip();

    /// <summary>
    /// プロパティ：アニメーションにかかる時間
    /// </summary>
    public float AnimationTime { get {return DiscFlipSettings.AnimationTime;} }

    /// <summary>
    /// プロパティ：跳ねるアニメーションのイージングカーブ
    /// </summary>
    public Easing.Curve HopEase { get {return DiscFlipSettings.HopEase;} }

    /// <summary>
    /// プロパティ：ひっくり返るアニメーションのイージングカーブ
    /// </summary>
    public Easing.Curve FlipEase{ get {return DiscFlipSettings.FlipEase;} }

}
