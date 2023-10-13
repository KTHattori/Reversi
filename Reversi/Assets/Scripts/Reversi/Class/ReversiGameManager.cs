using UnityEngine;
using Reversi;
using Unity.VisualScripting;

public class ReversiGameManager
{
    private const int Initiative = 0;
    private const int BlackSide = Initiative;
    private const int WhiteSide = Initiative + 1;
    private IReversiPlayer[] player = new IReversiPlayer[2];
    private int currentPlayer = 0;


    // Start is called before the first frame update
    public ReversiGameManager(bool isInitiative,ReversiAIDifficulty aiDifficulty)
    {
        if(isInitiative)
        {
            player[BlackSide] = new HumanPlayer();
            player[WhiteSide] = new AIPlayer(aiDifficulty);
        }
        else
        {
            player[BlackSide] = new AIPlayer(aiDifficulty);
            player[WhiteSide] = new HumanPlayer();
        }
    }

    public void Act(in Board board)
    {
        player[currentPlayer].OnTurn(board);
    }

    public void SwapTurn()
    {
        currentPlayer = ++currentPlayer % 2;
    }
}
