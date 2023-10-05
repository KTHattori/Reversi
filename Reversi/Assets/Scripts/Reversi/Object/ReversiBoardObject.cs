using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Reversi;
using TMPro;

public class ReversiBoardObject : MonoBehaviour
{
    static Board _board = null;
    
    static ReversiBoardObject instance = null;

    static ReversiDiscObject[,] objBoard = null;

    [SerializeField]
    Transform parentUI;

    [SerializeField]
    GameObject discPrefab;

    [SerializeField]
    Image background;

    [SerializeField]
    ReversiResultObject result;

    [SerializeField]
    TextMeshProUGUI turnText;

    [SerializeField]
    TextMeshProUGUI messageText;

    [SerializeField,Header("配置可能マス")]
    List<Point> movable = null;

    [SerializeField,Header("配置可能マスカラー")]
    Color movableColor = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        if(instance) {GameObject.Destroy(instance.gameObject);}
        instance = this;
        objBoard = new ReversiDiscObject[Constant.BoardSize + 2, Constant.BoardSize + 2];
        _board = new Board();
        for(int x = 0;x < Constant.BoardSize + 2; x++)
        {
            for(int y = 0; y < Constant.BoardSize + 2; y++)
            {
                GameObject obj = Instantiate(discPrefab,parentUI);
                ReversiDiscObject discObj = obj.GetComponent<ReversiDiscObject>();
                Disc disc = new Disc(x,y,_board.GetColor(x,y));
                
                discObj.Init(disc);
                objBoard[x,y] = discObj;
            }
        }

        HighlightMovable();

        SetTurn();
        SetMessage("Game Start!");
        result.Hide();
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
                objBoard[updated.x,updated.y].SetDiscColor(updated.discColor);
            }
            HighlightMovable();

            SetTurn();
            SetMessage("");
        }
        else
        {
            Debug.Log($"Cannot place disc at: {disc.x}, {disc.y}");
            SetMessage("Cannot place here!");
        }

        if(_board.IsGameOver())
        {
            instance.result.Show();
            instance.result.SetResult(_board.CountDisc(DiscType.Black),_board.CountDisc(DiscType.White));
            SetMessage("");
        }
    }

    static public void Pass()
    {
        if(_board.Pass())
        {
            SetMessage("Passed!");
            HighlightMovable();
            SetTurn();
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
                    objBoard[x,y].SetDiscColor(_board.GetColor(x,y));
                }
            }

            HighlightMovable();

            SetMessage("Undone!");
            SetTurn();
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
                objBoard[x,y].Init(disc);
            }
        }

        HighlightMovable();

        SetTurn();
        SetMessage("Game Start!");
        instance.result.Hide();
    }

    static public void HighlightMovable()
    {
        // previous movable
        foreach(Point point in instance.movable)
        {
            objBoard[point.x,point.y].SetImageColor(objBoard[point.x,point.y].DiscColor.ToColor());
        }

        // get
        instance.movable = _board.GetMovablePoint();

        // current movable
        foreach(Point point in instance.movable)
        {
            objBoard[point.x,point.y].SetImageColor(instance.movableColor);
        }
    }

    static public void SetTurn()
    {
        instance.turnText.SetText(_board.GetCurrentTurn().ToString());
        if(instance.background) instance.background.color = _board.GetCurrentColor().ToColor();
        instance.movable = _board.GetMovablePoint();
    }

    static public void SetMessage(string msg)
    {
        instance.messageText.SetText(msg);
    }
}
