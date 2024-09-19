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
        private Texture2D _texture;

        protected ChessPiece(string type, Player color, ChessTile homeTile)
        {
            Type = type;
            LastMovedTurn = 0;
            PieceColor = color;
            HomeTile = homeTile;
            _texture = null;
            IsSelected = false;
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

        public void MoveTo(ChessTile newTile, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
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
            }
            else
            {
                // handle special capture case of en passant
                if (this.Type == "pawn")
                {
                    Globals.MoveRule50 = 0;
                    // check if theres a pawn behind to see if its a capture move
                    int offset = this.PieceColor == Player.White ? -8 : 8;
                    capturedPiece = ChessUtils.GetPieceAtTile(chessBoard[chessBoard.IndexOf(newTile) - offset], chessPieces);
                    // if we find a piece it will 100% be the en passant pawn so we capture it 
                    if (capturedPiece != null)
                    {
                        chessPieces.Remove(capturedPiece);
                    }
                }
            }
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

        // checks if the move causes king to be in check
        public bool IsSafeMove(ChessTile targetTile, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // Save the current state
            ChessTile originalTile = this.HomeTile;
            ChessPiece capturedPiece = ChessUtils.GetPieceAtTile(targetTile, chessPieces);


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

            // Simulate the move
            this.HomeTile = targetTile;
            if (capturedPiece != null)
            {
                chessPieces.Remove(capturedPiece);
            }

            // Check if the king is in check
            bool isInCheck = ChessUtils.IsPieceUnderAttack(king, chessBoard, chessPieces);
            // Revert the move
            this.HomeTile = originalTile;

            if (capturedPiece != null)
            {
                chessPieces.Add(capturedPiece);
            }
            // if castling check if rook is under attack after the move
            bool castlsePossible = false;
            if (rook != null)
            {
                // rook isnt under attack after the move and the king isnt currently in check
                castlsePossible = ChessUtils.IsPieceUnderAttack(rook, chessBoard, chessPieces) && ChessUtils.IsPieceUnderAttack(rook, chessBoard, chessPieces);
                rook.HomeTile = rookOriginalTile;
            }
            return !isInCheck && !castlsePossible;
        }
    }
}
