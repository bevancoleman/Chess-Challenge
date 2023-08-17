using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

[TestFixture]
public class MyBotCheckMateTests
{
    private const string FenStartingBoard = "8/8/3qk3/8/8/3KR3/8/8 w - - 0 1";  //https://www.chess.com/analysis?tab=analysis
    private const int GameTimeMs = 60*1000;
    
    private Board _board;
    private IChessBot _chessBotWhite;
    private Timer _timer;

    /// <summary>
    /// Setup before each test, this makes sure to reset to a fresh playing field before each test.
    /// </summary>
    [SetUp]
    public void SetupBeforeEachTest()
    {
        _board = Board.CreateBoardFromFEN(FenStartingBoard);
        _chessBotWhite = new MyBot();
        _timer = new Timer(GameTimeMs, GameTimeMs, GameTimeMs);
    }
    

   
    /// <summary>
    /// Test to check board value generation.
    /// </summary>
    [Test]
    public void WhiteFirstMove_InCheckMate_ValueCheck()
    {
        // Expect
        const int expectedValue = 900; // Board is in Checkmate for white
        
        // Test
        var resultValue = ((MyBot)_chessBotWhite).CalculateBoardValue(_board, true);
        
        // Assert
        Assert.That(resultValue, Is.EqualTo(expectedValue));
    }
}