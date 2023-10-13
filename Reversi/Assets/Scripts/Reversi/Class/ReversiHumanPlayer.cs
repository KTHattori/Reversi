using UnityEngine;
using Reversi.Exception;

namespace Reversi
{
    public class HumanPlayer : IReversiPlayer
    {
        public void OnTurn(in Board board)
        {
            Debug.Log("Player's turn");
            ReversiBoard3D.EnablePlayerInteract();
        }
    }
}
