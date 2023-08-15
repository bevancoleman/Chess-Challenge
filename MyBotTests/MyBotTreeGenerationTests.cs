using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

[TestFixture]
public class MyBotTreeGenerationTests
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
    public void GenerateBoardMoves_FromSimpleBoard()
    {
        var initalZobristKey = _board.ZobristKey;
        var rootTree = new MyBot.TreeNode(Move.NullMove, initalZobristKey);
        const int maxDepth = 6;
        const bool playingWhite = true;
        
        // Test
        ((MyBot)_chessBotWhite).GenerateMoves(_board, rootTree, maxDepth, playingWhite);
        
        var finishedZobristKey = _board.ZobristKey;
            
        // Assert that board was returned back to initial state
        Assert.That(finishedZobristKey, Is.EqualTo(initalZobristKey));
    }
    
}