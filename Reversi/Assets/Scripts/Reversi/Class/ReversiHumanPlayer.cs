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
            _isInteractive = false;
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
