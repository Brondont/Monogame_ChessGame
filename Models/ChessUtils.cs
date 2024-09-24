using System.Collections.Generic;
using System.Linq;

namespace ChessGame.Models
{
    public class ChessUtils
    {

        public static ChessPiece GetPieceAtTile(ChessTile tile, List<ChessPiece> chessPieces)
        {
            foreach (var piece in chessPieces)
            {
                if (piece.HomeTile == tile)
                {
                    return piece;
                }
            }
            return null;
        }

        public static bool IsTileInBounds(int index, int boardSize)
        {
            return index >= 0 && index < boardSize;
        }


        public static bool IsPieceUnderAttack(ChessPiece testPiece, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            var pieceTile = testPiece.HomeTile;
            foreach (var piece in chessPieces)
            {
                if (piece.PieceColor != testPiece.PieceColor)
                {
                    var moves = piece.GetLegalMoves(chessBoard, chessPieces);
                    if (moves.Contains(pieceTile))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsGameOver(List<ChessTile> chessBoard, List<ChessPiece> chessPieces, Player currentPlayer, out string Message)
        {
            Message = null;
            if (IsCheckmate(currentPlayer, chessBoard, chessPieces))
            {
                Message = PlayerUtils.Opponent(currentPlayer)+ " won!";
                return true;
            }

            if (IsInsufficientMaterial(chessPieces) || IsStalemate(currentPlayer, chessBoard, chessPieces) || Is50MoveRule(chessPieces))
            {
                Message = "Draw !";
                return true;
            }

            return false;
        }

        public static bool IsKingInCheck(ChessPiece king, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // Check if any current player's piece is putting the king in check
            for (int i = 0; i < chessPieces.Count; i++)
            {
                ChessPiece piece = chessPieces[i];
                if (piece.PieceColor != king.PieceColor)
                {
                    var opponentMoves = piece.GetLegalMoves(chessBoard, chessPieces); // Get all legal moves of the piece

                    if (opponentMoves.Contains(king.HomeTile))
                    {
                        return true; // The king is under attack
                    }
                }
            }
            return false;
        }

        // Check if the current player is in checkmate
        public static bool IsCheckmate(Player player, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // Get all pieces of the attacked player
            var playerPieces = chessPieces.Where(p => p.PieceColor == player).ToList();

            // Check if any piece has a legal move
            foreach (var piece in playerPieces)
            {
                var legalMoves = piece.GetLegalSafeMoves(chessBoard, chessPieces);
                if (legalMoves.Count > 0)
                {
                    return false; // There is at least one legal move, so it's not checkmate
                }
            }

            var king = playerPieces.FirstOrDefault(p => p.Type == "king");
            return king != null && IsKingInCheck(king, chessBoard, chessPieces);
        }

        // Check if the current player is in stalemate
        public static bool IsStalemate(Player player, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // Get all pieces of the current player
            var playerPieces = chessPieces.Where(p => p.PieceColor == player).ToList();

            // Check if the king is in check
            var king = playerPieces.FirstOrDefault(p => p.Type == "king");
            if (king != null && IsKingInCheck(king, chessBoard, chessPieces))
            {
                return false; // If the king is in check, it's not stalemate
            }

            // Check if the player has any legal moves
            foreach (var piece in playerPieces)
            {
                var legalMoves = piece.GetLegalSafeMoves(chessBoard, chessPieces);
                if (legalMoves.Count > 0)
                {
                    return false; // There is at least one legal move, so it's not stalemate
                }
            }

            return true; // No legal moves and the king is not in check, so it's stalemate
        }

        // Check for insufficient material to checkmate
        public static bool IsInsufficientMaterial(List<ChessPiece> chessPieces)
        {
            // Get all remaining pieces
            var remainingPieces = chessPieces.Where(p => p.Type != "king").ToList();

            // Check for conditions where checkmate is impossible
            if (remainingPieces.Count == 0) return true; // King vs King
            if (remainingPieces.Count == 1 && (remainingPieces[0].Type == "bishop" || remainingPieces[0].Type == "knight"))
            {
                return true; // King + Bishop or King + Knight vs King
            }

            // King + Bishop vs King + Bishop (both bishops on the same color)
            if (remainingPieces.Count == 2 && remainingPieces.All(p => p.Type == "bishop"))
            {
                var bishops = remainingPieces.Cast<Bishop>().ToList();
                if (bishops[0].PieceColor == bishops[1].PieceColor)
                {
                    return true;
                }
            }

            return false;
        }

        // Placeholder function for 50-move rule, implement based on your move tracking
        public static bool Is50MoveRule(List<ChessPiece> chessPieces)
        {
            return Globals.MoveRule50 == 50;
        }

    }

}

