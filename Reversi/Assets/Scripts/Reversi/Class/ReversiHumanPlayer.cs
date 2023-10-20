using System.Threading;

namespace Reversi
{
    public class HumanPlayer : IReversiPlayer
    {
        public void Think(in Board board,CancellationToken cancelToken,SynchronizationContext mainThread)
        {

        }
        
        public IReversiPlayer.ActionResult Act(in Board board, Point point)
        {
            if(point.Equals(Point.Passed))
            {
                if(board.Pass()) return IReversiPlayer.ActionResult.Passed;
                else return IReversiPlayer.ActionResult.Failed;
            }
            else if(point.Equals(Point.Undone))
            {
                if(board.Undo()) return IReversiPlayer.ActionResult.Undone;
                else return IReversiPlayer.ActionResult.Failed;
            }
            else
            {
                if(board.Move(point)) return IReversiPlayer.ActionResult.Placed;
                else return IReversiPlayer.ActionResult.Failed;
            }
        }
    }
}
