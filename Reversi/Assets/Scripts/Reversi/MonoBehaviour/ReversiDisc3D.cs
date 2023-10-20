using System.Collections.Generic;
using UnityEngine;
using Reversi;
using Interpolation;
using System;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// 3Dオブジェクトとしてのリバーシ石定義
/// </summary>
public class ReversiDisc3D : MonoBehaviour
{
    public enum AnimationState
    {
        /// <summary>
        /// アニメーションなし
        /// </summary>
        None = 0,
        /// <summary>
        /// 配置
        /// </summary>
        Placing = 1,
        /// <summary>
        /// ひっくり返す
        /// </summary>
        Flipping = 2,
        /// <summary>
        /// 回収
        /// </summary>
        Recalling = 3,
    }

    /// <summary>
    /// 定数：ひっくり返す際の角度オフセット
    /// </summary>
    const float flipAngle = 90.0f;
    /// <summary>
    /// プロパティ：現在の石色に応じた角度を取得
    /// </summary>
    public float CurrentFlipAngle{ get { return flipAngle * (int)_disc.discColor;} }
    /// <summary>
    /// プロパティ：現在と逆の石色に応じた角度を取得
    /// </summary>
    public float InvertedFlipAngle{ get { return flipAngle * (int)_disc.discColor.GetInvertedColor();} }

    // private
    /// <summary>
    /// リバーシ石内部クラス
    /// </summary>
    [SerializeField]
    private Disc _disc;

    /// <summary>
    /// リバーシ石に関する設定 ScriptableObjectで定義
    /// </summary>
    [SerializeField]
    private ReversiDiscSettings _settings;
    public ReversiDiscSettings Settings { get { return _settings; }}

    /// <summary>
    /// アニメーションの状態
    /// </summary>
    [SerializeField]
    private AnimationState _animState = AnimationState.None;

    /// <summary>
    /// アニメーションの状態キュー
    /// </summary>
    private Queue<AnimationState> _animStateQueue = new Queue<AnimationState>();

    /// <summary>
    /// アニメーションの進行度.  範囲は 0.0f ~ 1.0f で表される
    /// </summary>
    private float AnimProgress { get { return Mathf.Clamp01((_currentTime - 0.0f) / (_settings.AnimationTime - 0.0f));}}
    /// <summary>
    /// 現在の時間
    /// </summary>
    private float _currentTime = 0.0f;

    /// <summary>
    /// 初期座標
    /// </summary>
    private Vector3 _initPos;
    public Vector3 InitPos { get { return _initPos;} }
    /// <summary>
    /// 初期スケール
    /// </summary>
    private Vector3 _initScale;
    public Vector3 InitScale { get { return _initScale;} }

    /// <summary>
    /// 位置指定用ボタンへの参照
    /// </summary>
    private ReversiPointSelector _pointSelector;

    /// <summary>
    /// 現在セットされているアニメーション
    /// </summary>
    private Reversi.DiscAnimation _animation = null;

    [SerializeField]
    private bool _isAnimating = false;
    public bool IsAnimating{ get { return _isAnimating; }}

    /// <summary>
    /// 位置指定用オブジェクトへの参照セット用プロパティ
    /// </summary>
    public ReversiPointSelector PointSelector { set{ _pointSelector = value; }}

    // public 
    /// <summary>
    /// プロパティ：自身の保持するリバーシ石をPoint型で返す
    /// </summary>
    public Point Point { get {return _disc;} }

    /// <summary>
    /// プロパティ：自身の保持するリバーシ石の色を返す
    /// </summary>
    public DiscColor DiscColor { get { return _disc.discColor;} set { _disc.discColor = value;}}

    /// <summary>
    /// 開始時にコール
    /// </summary>
    private void Start()
    {
        _initPos = transform.position;
        _initScale = transform.localScale;
        _animation = null;
    }

    /// <summary>
    /// Update毎に呼ばれる
    /// </summary>
    private void Update()
    {
        if(_animation == null)
        {
            return;
        }
        else
        {
            UpdateAnimation();
        }

    }

    /// <summary>
    /// 石を初期状態にする。
    /// </summary>
    public void Initialize()
    {
        gameObject.SetActive(false);
        SetMovable(false);
        _animation = null;
        _isAnimating = false;
    }

    /// <summary>
    /// 石の情報をセットする。
    /// </summary>
    /// <param name="disc"></param>
    public void SetDisc(Disc disc)
    {
        _disc = disc;
    }

    /// <summary>
    /// 石を配置する。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="delay"></param>
    public void PlaceDisc(DiscColor type,float delay)
    {
        gameObject.SetActive(true);
        _disc.discColor = type;
        SetAnimationState(AnimationState.Placing,delay);
    }

    /// <summary>
    /// 配置された石を戻す。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="delay"></param>
    public void RecallDisc(DiscColor type,float delay)
    {
        _disc.discColor = type;
        SetAnimationState(AnimationState.Recalling,delay);
    }

    /// <summary>
    /// 石をひっくり返す。
    /// </summary>
    /// <param name="type"></param>
    public void FlipDisc(DiscColor type,float delay)
    {
        _disc.discColor = type;
        SetAnimationState(AnimationState.Flipping,delay);
    }

    /// <summary>
    /// 配置可能状態にする。
    /// </summary>
    /// <param name="color"></param>
    public void SetMovable(bool flag,DiscColor color = DiscColor.Empty)
    {
        _pointSelector.SetSelectable(flag);
    }
    
    /// <summary>
    /// 表示テキストを変更する
    /// </summary>
    /// <param name="text"></param>
    public void SetDisplayText(string text)
    {
        _pointSelector.SetDisplayText(text);
    }

    /// <summary>
    /// アニメーションの状態を設定する
    /// </summary>
    /// <param name="state">アニメーションの状態</param>
    private void SetAnimationState(AnimationState state,float delay)
    {
        _animState = state;
        _currentTime = 0.0f;

        switch(_animState)
        {
        case AnimationState.None:
            _animation = null;
            _isAnimating = false;
            break;
        case AnimationState.Placing:
            _animation = new PlaceAnimation(this);
            break;
        case AnimationState.Flipping:
            _animation = new FlipAnimation(this);
            break;
        case AnimationState.Recalling:
            _animation = new RecallAnimation(this);
            break;
        }

        if(_animation != null) StartAnimation(delay);
    }

    /// <summary>
    /// アニメーション開始
    /// </summary>
    private async void StartAnimation(float delay)
    {
        await Task.Delay((int)(delay * 1000.0f + 1));

        switch(_animState)
        {
        case AnimationState.None:
            _animation = null;
            Debug.Log($"{_disc.x},{_disc.y}");
            break;
        case AnimationState.Placing:
            _animation = new PlaceAnimation(this);
            break;
        case AnimationState.Flipping:
            _animation = new FlipAnimation(this);
            break;
        case AnimationState.Recalling:
            _animation = new RecallAnimation(this);
            break;
        }

        if (_animation != null)
        {
            _animation.Start();
            _isAnimating = true;
        }
        else
        {
            Debug.Log("Animation is null, cannot start.");
        }
    }

    /// <summary>
    /// アニメーション更新
    /// </summary>
    private void UpdateAnimation()
    {
        _animation.Update(AnimProgress);

        if(_currentTime >= _settings.AnimationTime) EndAnimation();
        else _currentTime += Time.deltaTime;
    }

    /// <summary>
    /// アニメーション終了時
    /// </summary>
    private void EndAnimation()
    {
        _animation.End();
        SetAnimationState(AnimationState.None,0.0f);
    }
}
