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
                board.Pass();
                return IReversiPlayer.ActionResult.Passed;
            }
            else if(point.Equals(Point.Undone))
            {
                if(board.Undo()) return IReversiPlayer.ActionResult.Undone;
                else return IReversiPlayer.ActionResult.Failed;
            }
            else
            {
                board.Move(point);
                return IReversiPlayer.ActionResult.Placed;
            }
        }
    }
}
