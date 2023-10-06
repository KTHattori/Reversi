using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Reversi;
using TMPro;
using Unity.VisualScripting;

public class ReversiBoardObject : MonoBehaviour
{
    private static Board _board = null;
    
    private static ReversiBoardObject _instance = null;

    private static ReversiDiscObject[,] _objBoard = null;
    [SerializeField]
    ReversiDiscSettings _settings;

    [SerializeField]
    private Transform _parentUI;

    [SerializeField]
    private GameObject _discPrefab;

    [SerializeField]
    private Image _background;

    [SerializeField]
    private ReversiResultObject _result;

    [SerializeField]
    private TextMeshProUGUI _turnText;

    [SerializeField]
    private TextMeshProUGUI _messageText;

    [SerializeField,Header("配置可能マス")]
    private List<Point> _movable = null;

    public List<Disc> undones = null;

    [SerializeField,Header("配置可能マスカラー")]
    private Color _movableColor = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        if(_instance) {GameObject.Destroy(_instance.gameObject);}
        _instance = this;

        _objBoard = new ReversiDiscObject[Constant.BoardSize + 2, Constant.BoardSize + 2];
        _board = new Board();
        
        for(int x = 0;x < Constant.BoardSize + 2; x++)
        {
            for(int y = 0; y < Constant.BoardSize + 2; y++)
            {
                GameObject obj = Instantiate(_discPrefab,_parentUI);
                ReversiDiscObject discObj = obj.GetComponent<ReversiDiscObject>();
                Disc disc = new Disc(x,y,_board.GetColor(x,y));
                
                discObj.SetDisc(disc);
                _objBoard[x,y] = discObj;
            }
        }

        HighlightMovable();

        UpdateUI();
        SetMessage("Game Start!");
        _result.Hide();
    }

    static public void PlaceDisc(Disc disc)
    {
        if(_board.Move(disc))
        {
            Debug.Log("Disc placed at: " + disc.x + ", " + disc.y);
            
            List<Disc> updatedList = _board.GetUpdate();
            foreach(Disc updated in updatedList)
            {
                Debug.Log($"Flipped disc at: {updated.x}, {updated.y}");
                _objBoard[updated.x,updated.y].SetDiscColor(updated.discType);
            }
            HighlightMovable();

            UpdateUI();
            SetMessage("");
        }
        else
        {
            Debug.Log($"Cannot place disc at: {disc.x}, {disc.y}");
            SetMessage("Cannot place here!");

            return;
        }

        if(_board.IsGameOver())
        {
            _instance._result.Show();
            _instance._result.SetResult(_board.CountDisc(DiscType.Black),_board.CountDisc(DiscType.White));
            SetMessage("");
        }
    }

    static public void Pass()
    {
        if(_board.Pass())
        {
            SetMessage("Passed!");
            HighlightMovable();
            UpdateUI();
        }
        else
        {
            SetMessage("Cannot pass now!");
        }
    }

    static public void Undo()
    {
        if(_board.Undo())
        {
            List<Disc> undoneList = _board.GetUndone();
            foreach(Disc undone in undoneList)
            {
                Debug.Log($"Undone disc at: {undone.x}, {undone.y} to {undone.discType}");
                _objBoard[undone.x,undone.y].SetDiscColor(undone.discType);
            }

            _instance.undones = undoneList;

            HighlightMovable();

            SetMessage("Undone!");
            UpdateUI();
        }
        else
        {
            SetMessage("Cannot undo now!");
        }
    }

    static public void Restart()
    {
        _board = new Board();
        for(int x = 0;x < Constant.BoardSize + 2; x++)
        {
            for(int y = 0; y < Constant.BoardSize + 2; y++)
            {
                Disc disc = new Disc(x,y,_board.GetColor(x,y));
                _objBoard[x,y].SetDisc(disc);
            }
        }

        HighlightMovable();

        UpdateUI();
        SetMessage("Game Start!");
        _instance._result.Hide();
    }

    /// <summary>
    /// 配置可能マスをハイライトする
    /// </summary>
    static public void HighlightMovable()
    {
        // previous movable
        foreach(Point point in _instance._movable)
        {
            _objBoard[point.x,point.y].SetImageColor(_objBoard[point.x,point.y].DiscColor.ToColor());
        }

        // get
        _instance._movable = _board.GetMovablePoint();

        // current movable
        foreach(Point point in _instance._movable)
        {
            _objBoard[point.x,point.y].SetImageColor(_instance._movableColor);
        }
    }

    /// <summary>
    /// UI更新
    /// </summary>
    static public void UpdateUI()
    {
        _instance._turnText.SetText(_board.GetCurrentTurn().ToString());
        if(_instance._background) _instance._background.color = _board.GetCurrentColor().ToColor();
    }

    /// <summary>
    /// 画面下部に指定したメッセージを表示する
    /// </summary>
    /// <param name="msg"></param>
    static public void SetMessage(string msg)
    {
        _instance._messageText.SetText(msg);
    }

    void OnDestroy()
    {
        _instance.StopAllCoroutines();
        if(_instance == this)
        {
            _instance = null;
            _board = null;
            _objBoard = null;
        }
    }
}
