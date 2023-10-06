using UnityEngine;
using Interpolation;
using System;

[CreateAssetMenu(fileName = "New Reversi Disc Settings",menuName = "ScriptableObjects/Reversi/Disc Settings",order = 0)]
public class ReversiDiscSettings : ScriptableObject
{
    [System.Serializable]
    public class DiscFlip
    {
        [field:SerializeField] public float AnimationTime{ get; private set; } = 1.0f;
        [field:SerializeField] public Easing.Curve HopEase{ get; private set; } = Easing.Curve.EaseInOutCirc;
        [field:SerializeField] public Easing.Curve FlipEase{ get; private set; } = Easing.Curve.EaseOutCirc;
    }

    [field:SerializeField] public DiscFlip DiscFlipSettings{ get; private set;} = new DiscFlip();
    public float AnimationTime { get {return DiscFlipSettings.AnimationTime;} }
    public Easing.Curve HopEase { get {return DiscFlipSettings.HopEase;} }
    public Easing.Curve FlipEase{ get {return DiscFlipSettings.FlipEase;} }

}
