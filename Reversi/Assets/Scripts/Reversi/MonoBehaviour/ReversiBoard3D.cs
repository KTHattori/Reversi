using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Reversi;
using TMPro;

/// <summary>
/// 3Dオブジェクトとしてのリバーシ盤面定義
/// </summary>
public class ReversiBoard3D : MonoBehaviour
{
    /// <summary>
    /// ボード内部データ
    /// </summary>
    private static Board _board = null;
    
    /// <summary>
    /// このクラスのインスタンス保持用
    /// </summary>
    private static ReversiBoard3D _instance = null;

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
    /// UI背景 参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _backgroundObjRef;

    /// <summary>
    /// リザルト表示 参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _resultObjRef;

    /// <summary>
    /// ターン表示 参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _turnObjRef;

    /// <summary>
    /// メッセージ表示 参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _messageObjRef;

    /// <summary>
    /// パス用のボタン 参照
    /// </summary>
    [SerializeField]
    private ObjectReferencer _passButtonObjRef;

    /// <summary>
    /// 配置可能なマスを保持するリスト
    /// </summary>
    [SerializeField,Header("配置可能マス")]
    private List<Point> _movable = null;

    /// <summary>
    /// 背景 Imageコンポーネント参照
    /// </summary>
    private Image _backgroundImageRef;
    /// <summary>
    /// リザルトコンポーネント参照
    /// </summary>
    private ReversiResultObject _resultCompRef;
    /// <summary>
    /// ターン表示Text参照
    /// </summary>
    private TextMeshProUGUI _turnTextRef;
    /// <summary>
    /// メッセージ表示テキスト参照
    /// </summary>
    private TextMeshProUGUI _messageTextRef;


    /// <summary>
    /// オブジェクト生成初回ループにてコール
    /// </summary>
    private void Start()
    {
        if(_instance) {Destroy(_instance.gameObject);}
        _instance = this;

        GetComponentRefs();

        CreateBoard();
        InitializeBoard();

        HighlightMovable();

        UpdateUI();
        SetMessage("Game Start!");
        _resultCompRef.Hide();
    }

    /// <summary>
    /// オブジェクト破棄時
    /// </summary>
    private void OnDestroy()
    {
        _instance.StopAllCoroutines();
        if(_instance == this)
        {
            _instance = null;
            _board = null;
            _discObjBoard = null;
        }
    }


    /// <summary>
    /// 各コンポーネント参照を取得
    /// </summary>
    private void GetComponentRefs()
    {
        if(!_backgroundObjRef) Debug.LogError("Background Ref Not Set!");
        if(!_resultObjRef) Debug.LogError("Result Ref Not Set!");
        if(!_turnObjRef) Debug.LogError("Turn Text Ref Not Set!");
        if(!_messageObjRef) Debug.LogError("Message Text Ref Not Set!");
        if(!_passButtonObjRef) Debug.LogError("Pass Button Ref Not Set!");

        _backgroundImageRef = _backgroundObjRef.GetComponent<Image>();
        _resultCompRef = _resultObjRef.GetComponent<ReversiResultObject>();
        _turnTextRef = _turnObjRef.GetComponent<TextMeshProUGUI>();
        _messageTextRef = _messageObjRef.GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// ボードの作成（初回のみ）
    /// </summary>
    private void CreateBoard()
    {
        _discObjBoard = new ReversiDisc3D[Constant.BoardSize + 2, Constant.BoardSize + 2];
        _board = new Board();
        for(int x = 1;x < Constant.BoardSize + 1;x++)
        {
            for(int y = 1; y < Constant.BoardSize + 1; y++)
            {
                // 石の生成・整列
                GameObject discObj = Instantiate(_discPrefab,_discParentObjRef.transform);
                discObj.name = $"Disc({x},{y})";
                Vector3 pos = discObj.transform.localPosition;
                pos.x = 1.25f * x;
                pos.z = 1.25f * y;
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
    private void InitializeBoard()
    {
        _board.Init();

        int order = 0;
        for(int x = 1;x < Constant.BoardSize + 1;x++)
        {
            for(int y = 1; y < Constant.BoardSize + 1; y++)
            {
                // 初期状態にセット・石情報をセット
                DiscColor disctype = _board.GetColor(x,y);
                _discObjBoard[x,y].Initialize();    // 初期化
                _discObjBoard[x,y].SetDisc(new Disc(x,y,disctype));   // 値を流し込む

                // --- 初期で配置されている場所ならこの先を実行 ---
                if(disctype != DiscColor.White && disctype != DiscColor.Black) { continue; }
                _discObjBoard[x,y].PlaceDisc(disctype,order * _settings.AnimationDelay);  // 配置処理
                order++;
            }
        }
    }

    /// <summary>
    /// マスを選択する（置ける場合は置く）
    /// </summary>
    /// <param name="point"></param>
    static public void SelectPoint(Point point)
    {
        if(_board.Move(point))
        {         
            List<Disc> updatedList = _board.GetUpdate();
            int order = 0;
            foreach(Disc updated in updatedList)
            {
                // 新しく配置されたもののみ配置処理
                if(order == 0)
                {
                    _discObjBoard[updated.x,updated.y].PlaceDisc(updated.discColor,order);
                    Debug.Log($"Placed {updated.discColor} disc at: {point.x}, {point.y}");
                }
                else
                {
                    _discObjBoard[updated.x,updated.y].FlipDisc(updated.discColor,order * _instance._settings.AnimationDelay);
                    Debug.Log($"Flipped disc to {updated.discColor} at: {updated.x}, {updated.y}");
                }
                order++;
            }
            _instance.HighlightMovable();

            _instance.UpdateUI();
            _instance.SetMessage("");
        }
        else
        {
            Debug.Log($"Cannot place disc at: {point.x}, {point.y}");
            _instance.SetMessage("Cannot place here!");
        }

        if(_board.IsGameOver())
        {
            _instance._resultCompRef.Show();
            _instance._resultCompRef.SetResult(_board.CountDisc(DiscColor.Black),_board.CountDisc(DiscColor.White));
            _instance.SetMessage("");
        }

        Debug.Log($"Black: {_board.CountDisc(DiscColor.Black)}, White: {_board.CountDisc(DiscColor.White)}");
    }

    /// <summary>
    /// パスを試みる
    /// </summary>
    static public void Pass()
    {
        if(_board.Pass())
        {
            _instance.SetMessage("Passed!");
            _instance.HighlightMovable();
            _instance.UpdateUI();
        }
        else
        {
            _instance.SetMessage("Cannot pass now!");
        }
    }

    /// <summary>
    /// 一手戻しを試みる
    /// </summary>
    static public void Undo()
    {
        if(_board.Undo())
        {
            List<Disc> undoneList = _board.GetUndone();
            int order = 0;
            foreach(Disc undone in undoneList)
            {
                Debug.Log($"Undone disc at: {undone.x}, {undone.y} to {undone.discColor}");

                // 前の手で配置された石のみ回収処理
                if(order == 0) _discObjBoard[undone.x,undone.y].RecallDisc(undone.discColor,order);
                else _discObjBoard[undone.x,undone.y].FlipDisc(undone.discColor,order * _instance._settings.AnimationDelay);
                order++;
            }

            _instance.HighlightMovable();

            _instance.SetMessage("Undone!");
            _instance.UpdateUI();
        }
        else
        {
            _instance.SetMessage("Cannot undo now!");
        }
    }

    /// <summary>
    /// ゲームをリスタートする
    /// </summary>
    static public void Restart()
    {
        _instance.InitializeBoard();

        _instance.HighlightMovable();

        _instance.UpdateUI();
        _instance.SetMessage("Game Start!");
        _instance._resultCompRef.Hide();
    }

    /// <summary>
    /// 配置可能マスをハイライトする, 現在無効
    /// </summary>
    private void HighlightMovable()
    {
        // previous movable
        foreach(Point point in _movable)
        {
            _discObjBoard[point.x,point.y].SetMovable(false);
        }

        // get
        _movable = _board.GetMovablePoints();
        DiscColor current = _board.GetCurrentColor();

        // current movable
        foreach(Point point in _movable)
        {
            _discObjBoard[point.x,point.y].SetMovable(true,current);
        }
    }

    /// <summary>
    /// UI更新
    /// </summary>
    private void UpdateUI()
    {
        _turnTextRef.SetText(_board.GetCurrentTurn().ToString());
        _instance._backgroundImageRef.color = _board.GetCurrentColor().ToColor();
        
        if(_board.IsPassable()) _passButtonObjRef.ActivateObject();
        else _passButtonObjRef.DeactivateObject();
    }

    /// <summary>
    /// 画面下部に指定したメッセージを表示する
    /// </summary>
    /// <param name="msg"></param>
    private void SetMessage(string msg)
    {
        _messageTextRef.SetText(msg);
    }
}
