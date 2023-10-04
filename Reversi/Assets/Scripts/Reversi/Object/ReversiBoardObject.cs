using UnityEngine;
using Reversi;

public class ReversiBoardObject : MonoBehaviour
{
    [SerializeField]
    Board _board;

    [SerializeField]
    Transform parentUI;

    [SerializeField]
    GameObject discPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0;x < Constant.BoardSize + 2; x++)
        {
            for(int y = 0; y < Constant.BoardSize + 2; y++)
            {
                GameObject disc = Instantiate(discPrefab,parentUI);
                disc.GetComponent<ReversiDiscObject>().Set(new Disc(x,y,_board.GetColor(x,y)));
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
