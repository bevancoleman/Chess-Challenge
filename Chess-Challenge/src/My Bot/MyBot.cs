using System;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    // See PieceType
    // None, Pawn, Knight, Bishop, Rook, Queen, King
    private int[] _pieceValue = { 0, 10, 30, 30, 50, 90, 900 };

    // First movies - rather than leave it to calculation, just take a popular one.
    // https://www.masterclass.com/articles/chess-101-what-are-the-best-opening-moves-in-chess-learn-5-tips-for-improving-your-chess-opening
    // White, Black, White, etc...
    private string[] _moveList = { "e2e4", "c7c5" };


    private bool _playingWhite;
    private bool _playingFromStart;
    private string _miniMaxTree;


    public Move Think(Board board, Timer timer)
    {
        // Check init and validate side
        if (_miniMaxTree == null)
        {
            _playingWhite = board.IsWhiteToMove;
            _playingFromStart = board.ZobristKey == 13227872743731781434; // ZobristKey from a Fresh Board. Got this inline because it's more compact to do inline.
            _miniMaxTree = "";
        }
        
        // TODO remove this, this is just for testing to make sure nothing is going wrong. It wastes tokens space though.
        if (_playingWhite != board.IsWhiteToMove)
            throw new InvalidOperationException("Somehow changed side, probably reused ");

        //var currentMove = board.PlyCount;
        
        
        if (_playingFromStart)
        {
            // Pick a starting move from well known starters rather than calculating anything.
            switch (board.PlyCount)
            {
                // Pick a move for white Attack
                case 0:
                    return new Move(_moveList[0], board);
                // Pick a move for black defence
                case 1:
                    return new Move(_moveList[1], board);
            }
        }

        var moves = board.GetLegalMoves();

        // Simple board value calculation, it's just looking for the highest value board at any point.
        var scores = new int[moves.Length];
        for (var i = 0; i<moves.Length; i++)
        {
            
            board.MakeMove(moves[i]);
            scores[i] = CalculateBoardValue(board);
            board.UndoMove(moves[i]);
        }
        var maxScoreIndexOf = scores.ToList().IndexOf(scores.Max());

        // Use max score
        return moves[maxScoreIndexOf];
        
        
        // TODO MiniMax B-Tree structure, then add AB culling.
        // Then start getting smarter about biasing the scoring for non capture moves
    }


    /// <summary>
    /// Calculate Value of board.
    /// TODO: This probably should be inline to reduce symbols... but it's easier to test as a separate method.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public int CalculateBoardValue(Board board)
    {
        var value = 0;
        foreach (var piece in board.GetAllPieceLists())
        {
            // Calc value, make sure negative value for other side pieces.
            value += _pieceValue[(int)(piece.TypeOfPieceInList)] * piece.Count * (piece.IsWhitePieceList == _playingWhite ? 1: -1);
        }

        return value;
    }
}