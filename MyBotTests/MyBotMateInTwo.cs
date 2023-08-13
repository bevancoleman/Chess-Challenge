using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

/// <summary>
/// Mate in Two puzzle
/// See
///   - https://www.chesspuzzles.com/mate-in-two
///   - https://www.chess.com/analysis?tab=analysis
/// </summary>
[TestFixture]
public class MyBotMateInTwo
{
    private const string FenStartingBoard = "r1bk2nr/p2p1pNb/n2B4/1p1NP2P/6P1/3P1Q2/P1P1K3/q5b1 w - - 0 1";
    private const int GameTimeMs = 60*1000;
    
    private Board _board;
    private IChessBot _chessBot;
    private Timer _timer;

    /// <summary>
    /// Setup before each test, this makes sure to reset to a "black" playing field before each test.
    /// </summary>
    [SetUp]
    public void SetupBeforeEachTest()
    {
        _board = Board.CreateBoardFromFEN(FenStartingBoard);
        _chessBot = new MyBot();
        _timer = new Timer(GameTimeMs, GameTimeMs, GameTimeMs);
    }
    

    [Test]
    public void WhiteToMove_CheckInTwo()
    {
        // Expected Moves (using Stockfish 16 from Chess.com)
        var expectedMoveW1 = new Move("f2f6", _board);
        var expectedMoveB1 = new Move("g8f6", _board);
        var expectedMoveW2 = new Move("d6e7", _board);
        
        // Test (both White and Black Movies)
        var resultMoveW1 = _chessBot.Think(_board, _timer);
        
        var resultMoveB1 = _chessBot.Think(_board, _timer);
        
        var resultMoveW2 = _chessBot.Think(_board, _timer);
        
        // Assert
        Assert.That(resultMoveW1, Is.EqualTo(expectedMoveW1));
        Assert.That(resultMoveW1.IsCapture, Is.False);
        
        Assert.That(resultMoveB1, Is.EqualTo(expectedMoveB1));
        Assert.That(resultMoveB1.IsCapture, Is.True);
        Assert.That(resultMoveB1.CapturePieceType, Is.EqualTo(PieceType.Queen));
        
        Assert.That(resultMoveW2, Is.EqualTo(expectedMoveW2));
        Assert.That(resultMoveW2.IsCapture, Is.False);
        Assert.That(_board.IsInCheckmate, Is.True);
        
        
    }
}