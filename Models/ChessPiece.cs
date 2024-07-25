using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame.Models
{
  public class ChessPiece
  {
    public string Type { get; set; } // E.g., "Pawn", "Knight", "Bishop", etc.
    public Color Color { get; set; } // Color of the piece
    public ChessTile _homeTile { get; set; } // The tile the piece is on
    public bool Status { get; set; } // Captured or not, not sure how im going to get this to work but yes yes 
    public bool IsSelected { get; set; } // New property to indicate selection

    private Texture2D _texture; // Texture to represent the piece

    public ChessPiece(string type, Color color, ChessTile homeTile)
    {
      Type = type;
      Color = color;
      _homeTile = homeTile;
      _texture = null;
      IsSelected = false; // Initialize as not selected
    }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
    {
      // Load texture from content pipeline
      string texturePath = $"pieces/{Type}"; // Adjust path format if needed
      _texture = content.Load<Texture2D>(texturePath);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if (_texture != null && _homeTile != null)
      {
        // Draw the background of the piece if selected
        if (IsSelected)
        {
          var backgroundColor = Color.Red;
          var tileBounds = _homeTile.Bounds;
          var backgroundBounds = new Rectangle(
              (int)tileBounds.X,
              (int)tileBounds.Y,
              tileBounds.Width,
              tileBounds.Height
          );
          spriteBatch.Draw(Texture2DHelper.CreateSingleColorTexture(spriteBatch.GraphicsDevice, backgroundColor), backgroundBounds, backgroundColor);
        }

        var tileCenter = new Vector2(
            _homeTile.Bounds.X + _homeTile.Bounds.Width / 2,
            _homeTile.Bounds.Y + _homeTile.Bounds.Height / 2
        );
        var pieceSize = _homeTile.Bounds.Width * 1.2f; // Adjust size if needed
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
      _homeTile = newTile;
    }
  }

  // Helper class to create a solid color texture
  public static class Texture2DHelper
  {
    public static Texture2D CreateSingleColorTexture(GraphicsDevice graphicsDevice, Color color)
    {
      Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
      texture.SetData(new[] { color });
      return texture;
    }
  }
}
