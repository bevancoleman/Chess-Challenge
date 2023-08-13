using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

[TestFixture]
public class MyBotFreshBoardTests
{
    private const string FenStartingBoard = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
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
    public void WhiteFirstMove_UseBestByTest()
    {
        // Expect
        var expectedMoveW1 = new Move("e2e4", _board);
        
        // Test
        var resultMove = _chessBot.Think(_board, _timer);
        
        // Assert
        Assert.That(resultMove, Is.EqualTo(expectedMoveW1));
    }
}