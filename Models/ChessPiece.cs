using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ChessGame.Models
{
  public abstract class ChessPiece
  {
    public string Type { get; set; }
    public Player PieceColor { get; set; }
    public ChessTile HomeTile { get; set; }
    public bool IsSelected { get; set; }
    public bool HasMoved;
    private Texture2D _texture;

    protected ChessPiece(string type, Player color, ChessTile homeTile)
    {
      HasMoved = false;
      Type = type;
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

    public void MoveTo(ChessTile newTile)
    {
      HasMoved = true;
      HomeTile = newTile;
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

      // Simulate the move
      this.HomeTile = targetTile;
      if (capturedPiece != null)
      {
        chessPieces.Remove(capturedPiece);
      }

      // Check if the king is in check
      bool isInCheck = ChessUtils.IsKingInCheck(this.PieceColor, chessBoard, chessPieces);

      // Revert the move
      this.HomeTile = originalTile;
      if (capturedPiece != null)
      {
        chessPieces.Add(capturedPiece);
      }

      return !isInCheck;
    }
  }
}
