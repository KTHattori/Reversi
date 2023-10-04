using System.Collections.Generic;
using UnityEngine;
using Reversi;
using System.Data.Common;

public class ReversiBoardObject : MonoBehaviour
{
    [SerializeField]
    static Board _board;

    [SerializeField]
    Transform parentUI;

    [SerializeField]
    GameObject discPrefab;

    static ReversiDiscObject[,] objBoard = new ReversiDiscObject[Constant.BoardSize + 2, Constant.BoardSize + 2];

    // Start is called before the first frame update
    void Start()
    {
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
    }

    static public void PlaceDisc(Disc disc)
    {
        if(_board.Move(disc))
        {
            Debug.Log("placed at: " + disc.x + ", " + disc.y);
            List<Disc> updatedList = _board.GetUpdate();
            foreach(Disc updated in updatedList)
            {
                Debug.Log("update at: " + disc.x + ", " + disc.y);
                objBoard[updated.x,updated.y].Set(updated);
            }
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {

    }
}
