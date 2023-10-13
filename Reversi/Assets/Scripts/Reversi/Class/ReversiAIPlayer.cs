using UnityEngine;
namespace Reversi
{
    public class AIPlayer : IReversiPlayer
    {
        private AI _ai = null;

        public AIPlayer()
        {
            _ai = new AlphaBetaAI();
        }

        public AIPlayer(ReversiAIDifficulty difficultySO)
        {
            _ai = new AlphaBetaAI();
            SetDifficulty(difficultySO);
        }

        public void OnTurn(in Board board)
        {
            Debug.Log("AI's turn");
            _ai.Move(board);
        }

        public void SetDifficulty(ReversiAIDifficulty difficultySO)
        {
            _ai.SetDifficulty(difficultySO);
        }
    }
}
