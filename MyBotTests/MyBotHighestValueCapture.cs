using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

[TestFixture]
public class MyBotHighestValueCapture
{
    private const string FenStartingBoard = "8/8/2r1b3/3P4/8/8/8/8 w - - 0 1";
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
        // Test
        var resultMove = _chessBot.Think(_board, _timer);
        
        // Assert
        // TODO this fails because teh bot assumes that a board with no moves means it's a default setup. This is not a good assumption.
        Assert.That(resultMove.IsCapture, Is.True);
        Assert.That(resultMove.CapturePieceType, Is.EqualTo(PieceType.Knight));
    }
}