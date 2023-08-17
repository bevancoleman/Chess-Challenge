using ChessChallenge.API;
using MyBotTests;
using Timer = ChessChallenge.API.Timer;

[TestFixture]
public class MyBotHighestValueCaptureTests
{
    private const string FenStartingBoard = "8/8/2r1b3/3P4/8/8/8/8 w - - 0 1";  //https://www.chess.com/analysis?tab=analysis
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
    

    [Test]
    public void WhiteFirstMove_HighestValue_CaptureKnight()
    {
        // Test
        var resultMove = _chessBotWhite.Think(_board, _timer);
        
        // Assert
        Assert.That(TestHelpers.IsLegalMove(_board, resultMove), Is.True);
        
        Assert.That(TestHelpers.GenerateMoveName(resultMove), Is.EqualTo("d5c6"));
        
        Assert.That(resultMove.IsCapture, Is.True);
        Assert.That(resultMove.CapturePieceType, Is.EqualTo(PieceType.Rook));
    }
    
    /// <summary>
    /// Test to check board value generation.
    /// </summary>
    [Test]
    public void WhiteFirstMove_CalculateValue_StartingBoard()
    {
        // Expect
        const int expectedValue = -70; // Playing White, White(Pawn=10) - Black(Bishop=30, Rook=50)
        
        // Test
        var resultValue = ((MyBot)_chessBotWhite).CalculateBoardValue(_board, true);
        
        // Assert
        Assert.That(resultValue, Is.EqualTo(expectedValue));
    }
    
    [Test]
    public void WhiteFirstMove_CalculateValue_ThreeMoves()
    {
        // Expect
        // Playing White, White(Pawn=10) - Black(Bishop=30, Rook=50)
        var expectedValues = new Dictionary<string, int>()
        {
            { "d5c6", -20},
            { "d5d6", -70},
            { "d5e6", -40}
        };
        
        // Test
        var moves = _board.GetLegalMoves();

        var resultValues = new Dictionary<string, int>();
        foreach (var move in moves)
        {
            var moveKey = TestHelpers.GenerateMoveName(move);
            _board.MakeMove(move);
            var value =  ((MyBot)_chessBotWhite).CalculateBoardValue(_board, true);
            _board.UndoMove(move);

            resultValues.Add(moveKey, value);
        }
        
        // Assert
        Assert.That(moves.Length, Is.EqualTo(3));
        Assert.That(expectedValues.Keys, Is.EquivalentTo(resultValues.Keys));
        Assert.That(expectedValues.Values, Is.EquivalentTo(resultValues.Values));
    }
}