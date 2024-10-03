using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame.Models
{
    public abstract class ChessPiece
    {
        public string Type { get; set; }
        public Player PieceColor { get; set; }
        public ChessTile HomeTile { get; set; }
        public ChessTile LastTile { get; set; }
        public bool IsSelected { get; set; }
        public int LastMovedTurn { get; set; }
        public int Value;
        private Texture2D _texture;

        protected ChessPiece(string type, Player color, ChessTile homeTile, int pieceValue)
        {
            Type = type;
            LastMovedTurn = 0;
            PieceColor = color;
            HomeTile = homeTile;
            _texture = null;
            IsSelected = false;
            Value = pieceValue;
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            var colorString = PieceColor == Player.White ? "white" : "black";
            string texturePath = "pieces/" + $"{colorString}-{Type}";// Adjust path format if needed
            _texture = content.Load<Texture2D>(texturePath);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null && HomeTile != null)
            {
                var tileCenter = new Vector2(
                    HomeTile.Bounds.X + HomeTile.Bounds.Width / 2,
                    HomeTile.Bounds.Y + HomeTile.Bounds.Height / 2
                );
                var pieceSize = HomeTile.Bounds.Width * 1.2f; // Adjust size if needed
                var pieceBounds = new Rectangle(
                    (int)(tileCenter.X - pieceSize / 2),
                    (int)(tileCenter.Y - pieceSize / 2),
                    (int)pieceSize,
                    (int)pieceSize
                );

                spriteBatch.Draw(_texture, pieceBounds, Color.White);
            }
        }

        public bool IsUnderAttack(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            for (int i = 0; i < chessPieces.Count; i++)
            {
                ChessPiece piece = chessPieces[i];
                if (piece.PieceColor != this.PieceColor)
                {
                    var moves = piece.GetLegalMoves(chessBoard, chessPieces);
                    if (moves.Contains(this.HomeTile))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void UndoMove(ChessTile homeTile, ChessTile lastTile, int lastMovedTurn)
        {
            HomeTile = homeTile;
            LastTile = lastTile;
            LastMovedTurn = lastMovedTurn;
        }

        public ChessPiece MoveTo(ChessTile newTile, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            Globals.MoveRule50++;
            // check if piece exists in destination
            ChessPiece capturedPiece = ChessUtils.GetPieceAtTile(newTile, chessPieces);

            // move piece 
            LastTile = HomeTile;
            LastMovedTurn = ++Globals.TurnCount;
            HomeTile = newTile;

            // capture the piece at destination if exists
            if (capturedPiece != null)
            {
                Globals.MoveRule50 = 0;
                chessPieces.Remove(capturedPiece);
                Console.WriteLine($"Capture happened to {capturedPiece.PieceColor} {capturedPiece.Type} by {this.PieceColor} {this.Type}");
            }
            else {
                // handle special capture case of en passant
                if (this.Type == "pawn")
                {
                    // check if theres a pawn behind to see if its a capture move
                    // PS: this logic breaks when you are reverting moves with the engine thus we need a differnet function that just undoes a move
                    int offset = this.PieceColor == Player.White ? -8 : 8;
                    capturedPiece = ChessUtils.GetPieceAtTile(chessBoard[chessBoard.IndexOf(newTile) - offset], chessPieces);
                    // if we find a piece it will 100% be the en passant pawn so we capture it 
                    if (capturedPiece != null)
                    {
                        Console.WriteLine($"En passant capture happened to: {capturedPiece.PieceColor} {capturedPiece.Type} by {this.PieceColor} {this.Type}");
                        chessPieces.Remove(capturedPiece);
                    }
                }
            }
            return capturedPiece;
        }

        public abstract List<ChessTile> GetLegalMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces);

        public List<ChessTile> GetLegalSafeMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // pass each move through the is safe move to check whether that move leads to their king being in check 
            var legalMoves = GetLegalMoves(chessBoard, chessPieces);
            var safeMoves = new List<ChessTile>();
            foreach (var legalMove in legalMoves)
            {
                if (IsSafeMove(legalMove, chessBoard, chessPieces))
                {
                    safeMoves.Add(legalMove);
                }
            }

            return safeMoves;
        }

        public List<ChessTile> GetCaptureCheckMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            List<ChessTile> legalMoves = this.GetLegalSafeMoves(chessBoard, chessPieces);

            List<ChessTile> legalCaptureCheckMoves = legalMoves.Where(tile =>
            {
                ChessPiece piece = ChessUtils.GetPieceAtTile(tile, chessPieces);
                return piece != null && piece.PieceColor != this.PieceColor;
            }).ToList();

            return legalCaptureCheckMoves;
        }


        // checks if the move causes king to be in check
        public bool IsSafeMove(ChessTile targetTile, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // Save the current state
            ChessTile originalTile = this.HomeTile;


            ChessPiece king = chessPieces.FirstOrDefault(p => p.Type == "king" && p.PieceColor == this.PieceColor);

            // Handle special case for castling
            ChessPiece rook = null;
            ChessTile rookOriginalTile = null;
            // if the piece is king and the king isnt in check and the move is one of the castles movess
            if (this.Type == "king" && Math.Abs(chessBoard.IndexOf(targetTile) - chessBoard.IndexOf(originalTile)) == 2)
            {
                int kingIndex = chessBoard.IndexOf(king.HomeTile);
                if (chessBoard.IndexOf(targetTile) < kingIndex)
                {
                    // Queenside castling
                    rook = ChessUtils.GetPieceAtTile(chessBoard[kingIndex - 4], chessPieces);
                    rookOriginalTile = rook.HomeTile;
                    rook.HomeTile = chessBoard[kingIndex - 1];
                }
                else
                {
                    // Kingside castling
                    rook = ChessUtils.GetPieceAtTile(chessBoard[kingIndex + 3], chessPieces);
                    rookOriginalTile = rook.HomeTile;
                    rook.HomeTile = chessBoard[kingIndex + 1];
                }
            }

            // store state of piece
            ChessTile savedHomeTile = this.HomeTile;
            ChessTile savedLastTile = this.LastTile;
            int savedLastMovedTurn = this.LastMovedTurn;

            // Simulate the move
            ChessPiece capturedPiece = this.MoveTo(targetTile, chessBoard, chessPieces);
            // Check if the king is in check
            bool isInCheck = king.IsUnderAttack(chessBoard, chessPieces);
            // Revert the move
            this.UndoMove(savedHomeTile, savedLastTile, savedLastMovedTurn);

            if (capturedPiece != null)
            {
                chessPieces.Add(capturedPiece);
            }
            // if castling check if rook is under attack after the move
            bool castlsePossible = false;
            if (rook != null)
            {
                // rook isnt under attack after the move and the king isnt currently in check
                castlsePossible = rook.IsUnderAttack(chessBoard, chessPieces);
                rook.HomeTile = rookOriginalTile;
            }
            return !isInCheck && !castlsePossible;
        }
    }
}
