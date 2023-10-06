using System.Collections.Generic;
using UnityEngine;
using Reversi;
using Interpolation;

public class ReversiDisc3D : MonoBehaviour
{
    public enum AnimationState
    {
        None = 0,
        Placing = 1,
        Flipping = 2,
    }

    const float flipAngle = 90.0f;
    private float CurrentFlipAngle{ get { return flipAngle * (int)_disc.discColor;} }
    private float InvertedFlipAngle{ get { return flipAngle * (int)_disc.discColor.GetInvertedColor();} }

    // private
    [SerializeField]
    private Disc _disc;

    [SerializeField]
    private ReversiDiscSettings _settings;

    [SerializeField]
    AnimationState _animState = AnimationState.None;

    [SerializeField]
    Queue<AnimationState> _animStateQueue = new Queue<AnimationState>();

    [SerializeField]
    private float _animProgress = 0.0f;
    private float _currentTime = 0.0f;

    private Vector3 _initPos;

    // public 
    public Point Point { get {return _disc;} }

    public DiscType DiscColor { get { return _disc.discColor;} set { _disc.discColor = value;}}

    /// <summary>
    /// ディスク情報をセットする。
    /// </summary>
    /// <param name="disc"></param>
    public void SetDisc(Disc disc)
    {
        _disc = disc;
    }

    /// <summary>
    /// ディスクの色情報をセットする。
    /// </summary>
    /// <param name="discColor"></param>
    public void SetDiscColor(DiscType discColor)
    {
        _disc.discColor = discColor;
    }

    /// <summary>
    /// アニメーションの状態を設定する
    /// </summary>
    /// <param name="state">アニメーションの状態</param>
    public void SetAnimationState(AnimationState state)
    {
        _animStateQueue.Enqueue(state);

        if(_animState == AnimationState.None)
        {
            _animState = _animStateQueue.Dequeue();
            _currentTime = 0.0f;
            _animProgress = 0.0f;
        }
    }

    void UpdateAnimation()
    {
        
        switch(_animState)
        {
            case AnimationState.Placing:
            PlaceAnimation(MathUtility.OneMinus(Easing.Ease(_animProgress,1.0f,Easing.Curve.EaseInQuint)));
            break;
            case AnimationState.Flipping:
            HopAnimation(Easing.Ease(_animProgress,1.0f,_settings.HopEase));
            FlipAnimation(Easing.Ease(_animProgress,1.0f,_settings.FlipEase));
            break;
        }
        if(_currentTime >= _settings.AnimationTime) { EndAnimation(); return; }
        _currentTime += Time.deltaTime;
        _animProgress = Mathf.Clamp01((_currentTime - 0.0f) / (_settings.AnimationTime - 0.0f));
    }

    void EndAnimation()
    {
        _animProgress = 1.0f;
        _animState = AnimationState.None;

        if(_animStateQueue.TryDequeue(out _animState)) _animProgress = 0.0f;
    }
    
    void HopAnimation(float progress)
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Sin(progress * Mathf.PI) + _initPos.y;
        transform.position = pos;
    }

    void FlipAnimation(float progress)
    {
        Vector3 angle = transform.localEulerAngles;
        angle.x = MathUtility.Remap(progress,0.0f,1.0f,InvertedFlipAngle,CurrentFlipAngle);
        Debug.Log(angle.x);
        transform.localEulerAngles = angle;
    }

    void PlaceAnimation(float progress)
    {
        Vector3 pos = transform.position;
        pos.y = progress * 2.0f + _initPos.y;
        transform.position = pos;
    }

    /// <summary>
    /// コンポーネントがアタッチされた時にコールされる関数
    /// </summary>
    void Reset()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        _initPos = transform.position;

        SetAnimationState(AnimationState.Placing);
    }

    // Update is called once per frame
    void Update()
    {
        if(_animState == AnimationState.None)
        {
            return;
        }
        else
        {
            UpdateAnimation();
        }

    }
}
