using ChessChallenge.API;

public class MyBot : IChessBot
{
    // See PieceType
    // None, Pawn, Knight, Bishop, Rook, Queen, King
    private int[] _pieceValue = { 0, 10, 30, 30, 50, 90, 900 };

    // First movies - rather than leave it to calculation, just take a popular one.
    // https://www.masterclass.com/articles/chess-101-what-are-the-best-opening-moves-in-chess-learn-5-tips-for-improving-your-chess-opening
    string[] whiteFirstAttack = { "e2e4", "d2d4" }; 
    string[] blackFirstDefence = { "c7c5" };
    

    public Move Think(Board board, Timer timer)
    {
        var moves = board.GetLegalMoves();
        var currentMove = board.GameMoveHistory.Length;

        // Pick a starting move from well known starters rather than calculating anything.
        switch (currentMove)
        {
            // Pick a move for white Attack
            case 0:
                return new Move(whiteFirstAttack[0], board);
            // Pick a move for black defence
            case 1:
                return new Move(blackFirstDefence[0], board);
        }

        // Simple board value calculation, it's just looking for the highest value board at any point.
        var values = new int[moves.Length];
        for (var i = 0; i<values.Length; i++)
        {
            var value = 0;
            board.MakeMove(moves[i]);
            foreach (var piece in board.GetAllPieceLists())
            {
                // Calc value, make sure negative value for other side pieces.
                value += _pieceValue[(int)(piece.TypeOfPieceInList)] * (piece.IsWhitePieceList == board.IsWhiteToMove ? 1: -1);
            }

            values[i] = value;
            
            board.UndoMove(moves[i]);
        }

        // TODO, find best value
        return moves[0];
        
        
        // MiniMax B-Tree structure, then add AB culling.
    }
}