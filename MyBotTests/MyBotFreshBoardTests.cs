using ChessChallenge.API;
using ChessChallenge.Chess;
using MyBotTests;
using Board = ChessChallenge.API.Board;
using Timer = ChessChallenge.API.Timer;

[TestFixture]
public class MyBotFreshBoardTests
{
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
        _board = Board.CreateBoardFromFEN(FenUtility.StartPositionFEN);
        _chessBotWhite = new MyBot();
        _timer = new Timer(GameTimeMs, GameTimeMs, GameTimeMs);
    }
    

    [Test]
    public void WhiteFirstMove_UseBestByTest()
    {
        // Expect
       
        // Test
        var resultMove = _chessBotWhite.Think(_board, _timer);
        
        // Assert
        Assert.That(TestHelpers.IsLegalMove(_board, resultMove), Is.True);
        Assert.That(resultMove.StartSquare.Name, Is.EqualTo("e2"));
        Assert.That(resultMove.TargetSquare.Name, Is.EqualTo("e4"));
    }
}