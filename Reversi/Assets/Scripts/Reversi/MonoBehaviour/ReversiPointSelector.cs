using UnityEngine;
using Reversi;
using UnityEngine.EventSystems;

/// <summary>
/// リバーシのマス選択用オブジェクト
/// </summary>
[RequireComponent(typeof(EventTrigger))]
public class ReversiPointSelector : MonoBehaviour
{
    /// <summary>
    /// 割り当てられたマス座標
    /// </summary>
    private Point _point;

    /// <summary>
    /// 選択可能かどうか
    /// </summary>
    private bool _isSelectable;


    /// <summary>
    /// Update前にコールされる関数
    /// フラグ初期化とこのスクリプト無効化
    /// </summary>
    private void Start()
    {
        _isSelectable = false;
    }

    /// <summary>
    /// _isSelectableが変わったときに呼ばれる関数
    /// </summary>
    /// <param name="flag">変更結果</param>
    private void OnChangeSelectable(bool flag)
    {
        gameObject.SetActive(flag);
    }

    /// <summary>
    /// マス座標を代入
    /// </summary>
    /// <param name="point"></param>
    public void SetPoint(Point point)
    {
        _point = point;
    }

    /// <summary>
    /// このオブジェクトが押されたときにコールされる関数
    /// </summary>
    public void OnPressNetwork()
    {
        ReversiGameNetwork.Instance.SelectPoint(_point);
    }

    /// <summary>
    /// このオブジェクトが押されたときにコールされる関数
    /// </summary>
    public void OnPressLocal()
    {
        ReversiGameManager.Instance.SelectPoint(_point);
    }

    /// <summary>
    /// 選択可能フラグを設定する
    /// </summary>
    /// <param name="flag"></param>
    public void SetSelectable(bool flag)
    {
        _isSelectable = flag;
        OnChangeSelectable(flag);
    }
}
