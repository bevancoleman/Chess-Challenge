using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

[TestFixture]
public class MyBotCheckMateTests
{
    /// <summary>
    /// Setup before each test, this makes sure to reset to a fresh playing field before each test.
    /// </summary>
    [SetUp]
    public void SetupBeforeEachTest()
    {
        _chessBot = new MyBot();
        _timer = new Timer(GameTimeMs, GameTimeMs, GameTimeMs);
    }

    private const string FenCheckMateBlack = "k7/8/8/8/8/8/R7/1R6 w - - 0 1"; //https://www.chess.com/analysis?tab=analysis
    private const string FenCheckMateWhite = ""; //https://www.chess.com/analysis?tab=analysis
    private const int GameTimeMs = 60 * 1000;

    private IChessBot _chessBot;
    private Timer _timer;


    /// <summary>
    /// Test scoring if white and black is check-mated
    /// </summary>
    [Test]
    public void WhiteMove_InCheckMate_ValueCheck()
    {
        // Setup
        const int expectedValue = 900;
        var board = Board.CreateBoardFromFEN(FenCheckMateBlack);

        // Test
        var resultValue = ((MyBot)_chessBot).CalculateBoardValue(board, true);

        // Assert
        Assert.That(resultValue, Is.EqualTo(expectedValue));
    }

    /// <summary>
    /// Test scoring if black and white is check-mated
    /// </summary>
    [Test]
    public void BackMove_InCheckMate_ValueCheck()
    {
        // Setup
        const int expectedValue = 900;
        var board = Board.CreateBoardFromFEN(FenCheckMateWhite);

        // Test
        var resultValue = ((MyBot)_chessBot).CalculateBoardValue(board, false);

        // Assert
        Assert.That(resultValue, Is.EqualTo(expectedValue));
    }
}