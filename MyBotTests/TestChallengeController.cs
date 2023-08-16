using ChessChallenge.API;
using ChessChallenge.Chess;
using Board = ChessChallenge.Chess.Board;
using Move = ChessChallenge.Chess.Move;
using Timer = ChessChallenge.API.Timer;

namespace MyBotTests;

public class TestChallengeController
{
    private MoveGenerator _moveGenerator;
    private Board _board;
    private Timer _timer;
    
    public TestChallengeController(string fenStartingBoard, int gameTimeMs)
    {
        _moveGenerator = new MoveGenerator();

        _board = new Board();
        _board.LoadPosition(fenStartingBoard);
        
        _timer = new Timer(gameTimeMs, gameTimeMs, gameTimeMs);
    }
    
    public Move MakeBotMove(IChessBot bot)
    {
        // Bot uses a "lite" version of the actual objects
        ChessChallenge.API.Board botBoard = new ChessChallenge.API.Board(_board);
        
        var botMove = bot.Think(botBoard, _timer);
        
        var move = new Move(botMove.RawValue);
        if (!IsLegal(move))
            throw new InvalidOperationException("Invalid Move");

        // Make move on Core board
        _board.MakeMove(move, false);
        
        GameResult result = Arbiter.GetGameState(_board);
        if (result != GameResult.InProgress)
            throw new Exception("Game not in progress");
            
        
        return move;
    }
    
    public bool IsLegal(Move givenMove)
    {
        var moves = _moveGenerator.GenerateMoves(_board);
        foreach (var legalMove in moves)
        {
            if (givenMove.Value == legalMove.Value)
            {
                return true;
            }
        }
        return false;
    }
}