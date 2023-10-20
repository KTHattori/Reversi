using System.Collections.Generic;
using UnityEngine;
using Reversi;

/// <summary>
/// 3Dオブジェクトとしてのリバーシ盤面定義
/// </summary>
public class ReversiBoard3D : MonoBehaviour
{
    /// <summary>
    /// 石オブジェクトクラスの配列, ボード内部データと同じ構造
    /// </summary>
    private static ReversiDisc3D[,] _discObjBoard = null;   // 石

    /// <summary>
    /// ScriptableObjectで定義された各設定事項
    /// </summary>
    [SerializeField]
    private ReversiBoardSettings _settings;

    /// <summary>
    /// 石オブジェクトのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject _discPrefab;

    /// <summary>
    /// マス選択ボタンのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject _selectorPrefab;

    /// <summary>
    /// 石生成時の親となるオブジェクト
    /// </summary>
    [SerializeField]
    private ObjectReferencer _discParentObjRef;

    /// <summary>
    /// マス選択ボタン生成時の親となるオブジェクト
    /// </summary>
    [SerializeField]
    private ObjectReferencer _selectorParentObjRef;

    /// <summary>
    /// アニメーション中の石リスト
    /// </summary>
    //[SerializeField]
    private List<ReversiDisc3D>_onGoingAnimation = new List<ReversiDisc3D>();

    private bool _isAnimating = false;

    /// <summary>
    /// オブジェクト破棄時
    /// </summary>
    private void OnDestroy()
    {
        _discObjBoard = null;
    }

    /// <summary>
    /// ボードの作成（初回のみ）
    /// </summary>
    public void CreateBoard(in Board board)
    {
        _isAnimating = false;
        _discObjBoard = new ReversiDisc3D[Constant.BoardSize + 2, Constant.BoardSize + 2];
        for(int x = 1;x < Constant.BoardSize + 1;x++)
        {
            for(int y = 1; y < Constant.BoardSize + 1; y++)
            {
                // 石の生成・整列
                GameObject discObj = Instantiate(_discPrefab,_discParentObjRef.transform);
                discObj.name = $"Disc({x},{y})";
                Vector3 pos = discObj.transform.localPosition;
                pos.x = 1.25f * y;
                pos.z = 1.25f * x;
                pos += _settings.PositionOrigin;
                discObj.transform.localPosition = pos;

                // 選択用オブジェクトの生成・整列
                GameObject selectorObj = Instantiate(_selectorPrefab,_selectorParentObjRef.transform);
                selectorObj.name = $"Selector({x},{y})";
                Vector3 selectorPos = selectorObj.transform.position;
                selectorPos.x = discObj.transform.position.x;
                selectorPos.z = discObj.transform.position.z;
                selectorObj.transform.position = selectorPos;   // 石と同じxz座標

                ReversiPointSelector selectorComp = selectorObj.GetComponent<ReversiPointSelector>();
                selectorComp.SetPoint(new Point(x,y));

                // 石のコンポーネント取得と設定
                ReversiDisc3D disc3D = discObj.GetComponent<ReversiDisc3D>();  // 取得
                disc3D.PointSelector = selectorComp;
                _discObjBoard[x,y] = disc3D;   // 配列にコンポーネント保存
            }
        }
    }

    /// <summary>
    /// ボードの初期化
    /// </summary>
    public void InitializeBoard(in Board board)
    {
        _onGoingAnimation.Clear();
        int order = 0;
        for(int x = 1;x < Constant.BoardSize + 1;x++)
        {
            for(int y = 1; y < Constant.BoardSize + 1; y++)
            {
                // 初期状態にセット・石情報をセット
                DiscColor disctype = board.GetColorAt(x,y);
                _discObjBoard[x,y].Initialize();    // 初期化
                _discObjBoard[x,y].SetDisc(new Disc(x,y,disctype));   // 値を流し込む

                // --- 初期で配置されている場所ならこの先を実行 ---
                if(disctype != DiscColor.White && disctype != DiscColor.Black) { continue; }
                _discObjBoard[x,y].PlaceDisc(disctype,order * _settings.AnimationDelay);  // 配置処理
                _onGoingAnimation.Add(_discObjBoard[x,y]);
                order++;
            }
        }
    }


    /// <summary>
    /// アニメーション中の石が存在するかどうか
    /// </summary>
    public bool IsAnimating()
    {
        if(_isAnimating)
        {
            Debug.Log("Check");
            _onGoingAnimation.RemoveAll(anim => !anim.IsAnimating);
            if(_onGoingAnimation.Count >= 1) return true;
            else {_isAnimating = false; Debug.Log("EndAnim");}
        }
        return false;
    } 

    /// <summary>
    /// 石の配置
    /// </summary>
    /// <param name="point"></param>
    public void UpdateBoardOnPlace(in List<Disc> updatedList)
    {      
        int order = 0;
        _isAnimating = true;
        foreach(Disc updated in updatedList)
        {
            // 新しく配置されたもののみ配置処理
            if(order == 0)
            {
                _discObjBoard[updated.x,updated.y].PlaceDisc(updated.discColor,order);
            }
            else
            {
                _discObjBoard[updated.x,updated.y].FlipDisc(updated.discColor,order * _settings.AnimationDelay);
            }

            _onGoingAnimation.Add(_discObjBoard[updated.x,updated.y]);
            order++;
        }
    }

    /// <summary>
    /// 一手戻し
    /// </summary>
    public void UpdateBoardOnUndo(List<Disc> undoneList)
    {
        int order = 0;
        _isAnimating = true;
        foreach(Disc undone in undoneList)
        {
            // 前の手で配置された石のみ回収処理
            if(order == 0) _discObjBoard[undone.x,undone.y].RecallDisc(undone.discColor,order);
            else _discObjBoard[undone.x,undone.y].FlipDisc(undone.discColor,order * _settings.AnimationDelay);

            _onGoingAnimation.Add(_discObjBoard[undone.x,undone.y]);
            order++;
        }
    }

    /// <summary>
    /// 配置可能マスをハイライトする
    /// </summary>
    public void HighlightMovable(in List<Point> targets,DiscColor side)
    {
        // current movable
        foreach(Point point in targets)
        {
            _discObjBoard[point.x,point.y].SetMovable(true,side);
        }
    }

    /// <summary>
    /// 配置可能ハイライトを削除
    /// </summary>
    public void RemoveHighlight(in List<Point> targets)
    {
        foreach(Point point in targets)
        {
            _discObjBoard[point.x,point.y].SetMovable(false);
        }
    }

    /// <summary>
    /// 指定したPointの評価値を表示する
    /// </summary>
    /// <param name="point"></param>
    /// <param name="score"></param>
    public void DisplayEvalScore(Point point,int score)
    {
        _discObjBoard[point.x,point.y].SetDisplayText(score.ToString());
    }
}
