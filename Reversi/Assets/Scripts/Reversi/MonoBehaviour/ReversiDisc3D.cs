using System.Collections.Generic;
using UnityEngine;
using Reversi;
using Interpolation;
using System;
using System.Collections;

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
    private float CurrentFlipAngle{ get { return flipAngle * (int)_disc.discColor;} }
    /// <summary>
    /// プロパティ：現在と逆の石色に応じた角度を取得
    /// </summary>
    private float InvertedFlipAngle{ get { return flipAngle * (int)_disc.discColor.GetInvertedColor();} }

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

    /// <summary>
    /// アニメーションの状態
    /// </summary>
    [SerializeField]
    AnimationState _animState = AnimationState.None;

    /// <summary>
    /// アニメーションの状態キュー
    /// </summary>
    [SerializeField]
    Queue<AnimationState> _animStateQueue = new Queue<AnimationState>();

    /// <summary>
    /// アニメーションの進行度.  範囲は 0.0f ~ 1.0f で表される
    /// </summary>
    [SerializeField]
    private float _animProgress = 0.0f;
    /// <summary>
    /// 現在の時間
    /// </summary>
    private float _currentTime = 0.0f;

    /// <summary>
    /// 初期座標
    /// </summary>
    private Vector3 _initPos;
    /// <summary>
    /// 初期スケール
    /// </summary>
    private Vector3 _initScale;

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
    /// コンポーネントがアタッチされた時にコールされる関数
    /// </summary>
    void Reset()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        _initPos = transform.position;
        _initScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(_animState == AnimationState.None)
        {
            // if(Input.GetKeyDown(KeyCode.Space)) PlayAnimation(AnimationState.Flipping,0.0f); // デバッグ用
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
        // transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
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
        Debug.Log(type);
        FlipAnimation(1.0f);
        PlayAnimation(AnimationState.Placing,delay);
    }

    public void RecallDisc(DiscColor type,float delay)
    {
        _disc.discColor = type;
        PlayAnimation(AnimationState.Recalling,delay);
    }

    /// <summary>
    /// 石の色情報のみを設定する（ひっくり返す）。
    /// </summary>
    /// <param name="type"></param>
    public void FlipDisc(DiscColor type,float delay)
    {
        _disc.discColor = type;
        PlayAnimation(AnimationState.Flipping,delay);
    }

    /// <summary>
    /// 一定時間後にアニメーション状態をセットするコルーチン
    /// </summary>
    /// <param name="state"></param>
    /// <param name="delay"></param>
    public void PlayAnimation(AnimationState state,float delay)
    {
        StartCoroutine(SetAnimationState(state,delay));
    }


    /// <summary>
    /// アニメーションの状態を設定する
    /// </summary>
    /// <param name="state">アニメーションの状態</param>
    private IEnumerator SetAnimationState(AnimationState state,float delay)
    {
        yield return new WaitForSeconds(delay);

        _animStateQueue.Enqueue(state);

        if(_animState == AnimationState.None)
        {
            _animState = _animStateQueue.Dequeue();
            _currentTime = 0.0f;
            _animProgress = 0.0f;
        }
    }

    /// <summary>
    /// アニメーション更新
    /// </summary>
    void UpdateAnimation()
    {
        switch(_animState)
        {
            case AnimationState.Placing:
            PlaceAnimation(Easing.Ease(_animProgress,1.0f,Easing.Curve.EaseInQuint));
            // ScaleAnimation(_animProgress);
            break;
            case AnimationState.Flipping:
            HopAnimation(Easing.Ease(_animProgress,1.0f,_settings.HopEase));
            FlipAnimation(Easing.Ease(_animProgress,1.0f,_settings.FlipEase));
            break;
            case AnimationState.Recalling:
            RecallAnimation(Easing.Ease(_animProgress,1.0f,Easing.Curve.EaseInQuint));
            // ScaleAnimation(1.0f - _animProgress);
            break;
            
        }
        if(_currentTime >= _settings.AnimationTime) { EndAnimation(); return; }
        _currentTime += Time.deltaTime;
        _animProgress = Mathf.Clamp01((_currentTime - 0.0f) / (_settings.AnimationTime - 0.0f));
    }

    /// <summary>
    /// アニメーション終了時
    /// </summary>
    void EndAnimation()
    {
        _animProgress = 1.0f;

        switch(_animState)
        {
            case AnimationState.Recalling:
            gameObject.SetActive(false);
            break;
        }

        _animState = AnimationState.None;

        if(_animStateQueue.TryDequeue(out _animState)) _animProgress = 0.0f;
    }
    
    // Animations
    /// <summary>
    /// 跳ねるアニメーション
    /// </summary>
    /// <param name="progress"></param>
    void HopAnimation(float progress)
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Sin(progress * Mathf.PI) + _initPos.y;
        transform.position = pos;
    }

    /// <summary>
    /// ひっくり返るアニメーション
    /// </summary>
    /// <param name="progress"></param>
    void FlipAnimation(float progress)
    {
        Vector3 angle = transform.localEulerAngles;
        angle.x = MathUtility.Remap(progress,0.0f,1.0f,InvertedFlipAngle,CurrentFlipAngle);
        transform.localEulerAngles = angle;
    }

    /// <summary>
    /// 置かれるアニメーション
    /// </summary>
    /// <param name="progress"></param>
    void PlaceAnimation(float progress)
    {
        Vector3 pos = transform.position;
        pos.y = (1.0f - progress) * 2.0f + _initPos.y;
        transform.position = pos;
    }

    /// <summary>
    /// 大きさを変えるアニメーション
    /// </summary>
    /// <param name="progress"></param>
    void ScaleAnimation(float progress)
    {
        transform.localScale = progress * _initScale;
    }

    /// <summary>
    /// 石を回収するアニメーション
    /// </summary>
    /// <param name="progress"></param>
    void RecallAnimation(float progress)
    {
        Vector3 pos = transform.position;
        pos.y = progress * 2.0f + _initPos.y;
        transform.position = pos;
    }
}
