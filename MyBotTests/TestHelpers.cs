
using ChessChallenge.API;

namespace MyBotTests;

/// <summary>
/// Some helper methods to make tests easier
/// </summary>
public static class TestHelpers
{
    public static bool IsLegalMove(Board board, Move move)
    {
        var legalmoves = board.GetLegalMoves();
        return legalmoves.Any(m => move.Equals(m));
    }

    public static string GenerateMoveName(Move move)
    {
        return $"{move.StartSquare.Name}{move.TargetSquare.Name}";
    }
}