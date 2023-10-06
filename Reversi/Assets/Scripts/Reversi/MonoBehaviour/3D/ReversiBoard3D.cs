using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Reversi;
using TMPro;
using Unity.VisualScripting;

public class ReversiBoard3D : MonoBehaviour
{
    private static Board _board = null;
    
    private static ReversiBoard3D _instance = null;

    private static ReversiDisc3D[,] _objBoard = null;
    [SerializeField]
    ReversiDiscSettings _settings;

    [SerializeField]
    private Transform _boardObject;

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

    [SerializeField,Header("配置可能マスカラー")]
    private Color _movableColor = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        if(_instance) {GameObject.Destroy(_instance.gameObject);}
        _instance = this;

        _objBoard = new ReversiDisc3D[Constant.BoardSize + 2, Constant.BoardSize + 2];
        _board = new Board();
        
        for(int x = 0;x < Constant.BoardSize + 2; x++)
        {
            for(int y = 0; y < Constant.BoardSize + 2; y++)
            {
                DiscType disctype = _board.GetColor(x,y);
                if(disctype != DiscType.White && disctype != DiscType.Black) { _objBoard[x,y] = null; return; }

                GameObject obj = Instantiate(_discPrefab,_boardObject);
                ReversiDisc3D discObj = obj.GetComponent<ReversiDisc3D>();
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
                _objBoard[updated.x,updated.y].SetDiscColor(updated.discColor);
            }
            HighlightMovable();

            UpdateUI();
            SetMessage("");
        }
        else
        {
            Debug.Log($"Cannot place disc at: {disc.x}, {disc.y}");
            SetMessage("Cannot place here!");
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
            for(int x = 0;x < Constant.BoardSize + 2; x++)
            {
                for(int y = 0; y < Constant.BoardSize + 2; y++)
                {
                    _objBoard[x,y].SetDiscColor(_board.GetColor(x,y));
                }
            }

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
            //_objBoard[point.x,point.y].SetImageColor(_objBoard[point.x,point.y].DiscColor.ToColor());
        }

        // get
        _instance._movable = _board.GetMovablePoint();

        // current movable
        foreach(Point point in _instance._movable)
        {
            //_objBoard[point.x,point.y].SetImageColor(_instance._movableColor);
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
