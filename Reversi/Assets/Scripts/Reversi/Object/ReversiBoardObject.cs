using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Reversi;
using TMPro;

public class ReversiBoardObject : MonoBehaviour
{
    [SerializeField]
    static Board _board;

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

    [SerializeField]
    List<Point> movable = null;

    static ReversiBoardObject instance = null;

    static ReversiDiscObject[,] objBoard = new ReversiDiscObject[Constant.BoardSize + 2, Constant.BoardSize + 2];

    static void DebugInfo()
    {
        instance.movable = _board.CurrentMovablePoints;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(instance) {GameObject.Destroy(instance.gameObject);}
        instance = this;
        _board = new Board();
        for(int x = 0;x < Constant.BoardSize + 2; x++)
        {
            for(int y = 0; y < Constant.BoardSize + 2; y++)
            {
                GameObject obj = Instantiate(discPrefab,parentUI);
                ReversiDiscObject discObj = obj.GetComponent<ReversiDiscObject>();
                Disc disc = new Disc(x,y,_board.GetColor(x,y));
                
                discObj.Set(disc);
                objBoard[x,y] = discObj;
            }
        }

        SetTurn();
        SetMessage("Game Start!");

        DebugInfo();
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
                objBoard[updated.x,updated.y].Set(updated);
            }
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
            instance.result.SetResult(_board.CountDisc(DiscColor.Black),_board.CountDisc(DiscColor.White));
            SetMessage("");
        }

        SetTurn();
    }

    static public void Pass()
    {
        if(_board.Pass())
        {
            SetMessage("Passed!");
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
                    objBoard[x,y].Set(new Disc(x,y,_board.GetColor(x,y)));
                }
            }

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
        
    }

    static public void SetTurn()
    {
        instance.turnText.SetText(_board.GetCurrentTurn().ToString());
        if(instance.background) instance.background.color = _board.GetCurrentColor().ToColor();
        DebugInfo();
    }

    static public void SetMessage(string msg)
    {
        instance.messageText.SetText(msg);
    }
}
