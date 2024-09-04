using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame.UI
{
    public class Button
    {
        private readonly string text;
        private readonly SpriteFont font;
        private Rectangle bounds;
        private Color normalColor;
        private Color hoverColor;
        private Color textColor;
        private bool isHovered;
        private MouseState _currentMouse;
        private MouseState _prevMouse;

        public EventHandler Click;

        public Button(string text, SpriteFont font, Vector2 position, Vector2 size)
        {
            this.text = text;
            this.font = font;
            this.bounds = new Rectangle(position.ToPoint(), size.ToPoint());
            this.normalColor = Color.White;
            this.hoverColor = Color.Gray;
            this.textColor = Color.Black;
        }


        public void Update(GameTime gameTime)
        {
            _prevMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            isHovered = bounds.Contains(_currentMouse.Position);


            if (_currentMouse.LeftButton == ButtonState.Released && _prevMouse.LeftButton == ButtonState.Pressed)
            {
                if (isHovered)
                    Click?.Invoke(this, new EventArgs());
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color buttonColor = isHovered ? hoverColor : normalColor;

            // Draw button bounds
            Texture2D whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { buttonColor });
            spriteBatch.Draw(whitePixel, bounds, buttonColor * 0.5f);

            // Draw text
            Vector2 textSize = font.MeasureString(text);
            Vector2 textPosition = new Vector2(bounds.X + (bounds.Width - textSize.X) / 2, bounds.Y + (bounds.Height - textSize.Y) / 2);
            spriteBatch.DrawString(font, text, textPosition, textColor);
        }

    }

}
