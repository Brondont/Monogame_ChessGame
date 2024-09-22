using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame.Models
{
    public class ChessTile
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Rectangle Bounds { get; set; }
        public string TileCoordinate { get; set; }
        public bool IsHighlighted { get; set; }
        private SpriteFont _font;
        private Texture2D _texture;

        public ChessTile(Vector2 position, Color color, int size, string tileCoordinate, SpriteFont font)
        {
            Position = position;
            Color = color;
            Bounds = new Rectangle((int)position.X, (int)position.Y, size, size);
            TileCoordinate = tileCoordinate;
            _font = font;
            _texture = null;
        }
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(graphicsDevice, 1, 1);
                _texture.SetData(new[] { Color });
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var drawColor = IsHighlighted ? Color.LightSkyBlue : Color;

            // Drawing the tile 
            spriteBatch.Draw(_texture, Bounds, drawColor);

            //drawing coordinate
            var textSize = _font.MeasureString(TileCoordinate);
            var textPosition = new Vector2(Bounds.X + (Bounds.Width - textSize.X - 5), Bounds.Y + (Bounds.Height - textSize.Y - 5));

            spriteBatch.DrawString(_font, TileCoordinate, textPosition, Color.Gray);
        }
    }
}
