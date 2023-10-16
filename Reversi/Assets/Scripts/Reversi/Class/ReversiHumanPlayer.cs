using UnityEngine;
using Reversi.Exception;
using System.Collections;
using System.Threading.Tasks;

namespace Reversi
{
    public class HumanPlayer : IReversiPlayer
    {
        private bool _isInteractive = false;
        
        public void Think(in Board board)
        {
            _isInteractive = true;
        }
        
        public IReversiPlayer.ActionResult Act(in Board board, Point point)
        {
            if(point != null)
            {
                board.Move(point);
                return IReversiPlayer.ActionResult.Placed;
            }
            else
            {
                board.Undo();
                return IReversiPlayer.ActionResult.Undone;
            }
        }
    }
}
