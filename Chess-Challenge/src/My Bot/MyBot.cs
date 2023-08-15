using System;
using System.Collections;
using System.Collections.Generic;
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
    private TreeNode _treeRoot;
    private ulong? _startZobristKey;
    
    private const int MaxDepth = 2;


    private Dictionary<ulong, int> _scoreCache = new();

    public class TreeNode : IEnumerable<TreeNode>
    {
        private readonly Dictionary<Move, TreeNode> _children = new();

        public readonly Move Move; //ID
        public readonly ulong ZobristKey;
       
        public TreeNode Parent { get; private set; }

        public TreeNode(Move move, ulong zobristKey)
        {
            Move = move;
            ZobristKey = zobristKey;
        }

        public TreeNode GetChild(Move move)
        {
            return _children[move];
        }

        public void Add(TreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent._children.Remove(item.Move);
            }
            item.Parent = this;
            _children.Add(item.Move, item);
        }

        public IEnumerator<TreeNode> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }
        
        public int Count => _children.Count;
    }

    public Move Think(Board board, Timer timer)
    {
        // Check init and validate side
        if (_startZobristKey == null)
        {
            _startZobristKey = board.ZobristKey;
            _playingFromStart = board.ZobristKey == 13227872743731781434; // ZobristKey from a Fresh Board. Got this inline because it's more compact to do inline.
            _playingWhite = board.IsWhiteToMove;
        }
        
        // TODO remove this, this is just for testing to make sure nothing is going wrong. It wastes tokens space though.
        if (_playingWhite != board.IsWhiteToMove)
            throw new InvalidOperationException("Somehow changed side, probably reused ");

        // If a fresh board, pick a starting move from well known starters rather than calculating anything.
        if (_playingFromStart)
        {
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
        
        // Start by making sure we have our root
        if (_treeRoot == null)
        {
            _scoreCache.Add(board.ZobristKey, CalculateBoardValue(board, _playingWhite));
            _treeRoot = new TreeNode(Move.NullMove, board.ZobristKey);
        }
        else
        {
            // Update tree to point to opposition move... we dont' care about paths not taken or history (we can get that from the board anyway)
            _treeRoot = _treeRoot.GetChild(board.GameMoveHistory.Last());
        }
        
        // Update Tree, this should reuse the tree and add depth
        GenerateMoves(board, _treeRoot, MaxDepth, _playingWhite);

        // Find move
        var pickedMove = Move.NullMove;
        var maxScore = int.MinValue;
        foreach (var child in _treeRoot)
        {
            var score = Minimax(child, MaxDepth, true);
            if (score > maxScore)
            {
                maxScore = score;
                pickedMove = child.Move;
            }
        }

        if (pickedMove.Equals(Move.NullMove))
            throw new InvalidOperationException("Unable to pick a valid move????");

        // Update TreeRoot to move taken;
        _treeRoot = _treeRoot.GetChild(pickedMove);
        
        // Return move
        return pickedMove;
    }

    public void GenerateMoves(Board board, TreeNode parentNode, int depth, bool playingWhite)
    {
        // Hit max depth, abort
        if (depth == 0)
            return;
        
        // No Children (yet?)
        if (parentNode.Count == 0)
        {
            var moves = board.GetLegalMoves();
            var maxChildScore = int.MinValue;
            
            foreach (var move in moves)
            {
                // Try Move
                board.MakeMove(move);
                
                var checkmate = board.IsInCheckmate();                
                
                // Calculate score if we don't already know it.
                int moveScore;
                if (!_scoreCache.ContainsKey(board.ZobristKey))
                {
                    moveScore = CalculateBoardValue(board, playingWhite);
                    _scoreCache.Add(board.ZobristKey, moveScore);
                }

                var childNode = new TreeNode(move, board.ZobristKey);
                parentNode.Add(childNode);

                // Check if it's worth going on. The game stops at checkmate... so no point going past this!
                // TODO Consider how to prune out worthless trees at this point and not continue to explore
                if (!checkmate)
                {
                    GenerateMoves(board, childNode, depth - 1, playingWhite);
                }

                // Undo move
                board.UndoMove(move);
            }
        }
    }

    static string MoveName(Move move)
    {
        return $"{move.StartSquare.Name}{move.TargetSquare.Name}";
    }

    /// <summary>
    /// Minimax alg, see https://en.wikipedia.org/wiki/Minimax
    /// </summary>
    /// <param name="node"></param>
    /// <param name="depth"></param>
    /// <param name="maximizingPlayer"></param>
    /// <returns></returns>
    public int Minimax(TreeNode node, int depth, bool maximizingPlayer)
    {
        if (depth == 0 || node.Count == 0)  // At max depth, or hit a leaf
        {
            // TODO: This assumes each leaf-node has a score generated.
            return _scoreCache[node.ZobristKey];
        }

        int value;
        if (maximizingPlayer)
        {
            value = int.MinValue;
            foreach (var child in node)
                value = Math.Max(value, Minimax(child, depth - 1, false));
        }
        else
        {
            value = int.MaxValue;
            foreach (var child in node)
                value = Math.Min(value, Minimax(child, depth - 1, true));
        }
        return value;
    }


    /// <summary>
    /// Calculate Value of board.
    /// TODO: This probably should be inline to reduce symbols... but it's easier to test as a separate method.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public int CalculateBoardValue(Board board, bool playingWhite)
    {
        var value = 0;
        foreach (var piece in board.GetAllPieceLists())
        {
            // Calc value, make sure negative value for other side pieces.
            value += _pieceValue[(int)(piece.TypeOfPieceInList)] * piece.Count * (piece.IsWhitePieceList == playingWhite ? 1: -1);
        }

        return value;
    }
}