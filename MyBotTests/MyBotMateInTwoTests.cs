using ChessChallenge.API;
using ChessChallenge.Chess;
using MyBotTests;

/// <summary>
/// Mate in Two puzzle
/// See
///   - https://www.chesspuzzles.com/mate-in-two
///   - https://www.chess.com/analysis?tab=analysis
/// </summary>
[TestFixture]
public class MyBotMateInTwoTests
{
    private const string FenStartingBoard = "r1bk2nr/p2p1pNb/n2B4/1p1NP2P/6P1/3P1Q2/P1P1K3/q5b1 w - - 0 1";
    private const int GameTimeMs = 60*1000;

    private TestChallengeController _testChallengeController;
    private IChessBot _chessBotWhite;
    private IChessBot _chessBotBlack;

    /// <summary>
    /// Setup before each test, this makes sure to reset to a fresh playing field before each test.
    /// </summary>
    [SetUp]
    public void SetupBeforeEachTest()
    {
        _testChallengeController = new TestChallengeController(FenStartingBoard, GameTimeMs);
        _chessBotWhite = new MyBot();
        _chessBotBlack = new MyBot();
    }

    [Test]
    public void WhiteToMove_CheckInTwo()
    {
        // Expected Moves (using Stockfish 16 from Chess.com) for Mate-in-two
        string[] expectedMoved = { "f3f6", "g8f6", "d6e7" };

        // Test (both White and Black Movies) and Assert
        string[] resultMoved = new string[expectedMoved.Length];
        
        var resultMoveW1 = _testChallengeController.MakeBotMove(_chessBotWhite);
        resultMoved[0] = MoveUtility.GetMoveNameUCI(resultMoveW1); 
        
        var resultMoveB1 = _testChallengeController.MakeBotMove(_chessBotBlack);
        resultMoved[1] = MoveUtility.GetMoveNameUCI(resultMoveB1);
        
        var resultMoveW2 = _testChallengeController.MakeBotMove(_chessBotWhite);
        resultMoved[2] = MoveUtility.GetMoveNameUCI(resultMoveW2);
        
        // Assert
        Assert.That(resultMoved, Is.EquivalentTo(expectedMoved));
    }
}