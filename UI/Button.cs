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
        private Color textHoverColor;
        private Color textColor;
        private bool isHovered;
        private MouseState _currentMouse;
        private MouseState _prevMouse;
        private const float SelectionDelay = 0.1f;
        private float _selectionTimer = 0f;
        private bool _isDelayFinished = false;

        public EventHandler Click;

        public Button(string text, SpriteFont font, Vector2 position, Vector2 size)
        {
            this.text = text;
            this.font = font;
            this.bounds = new Rectangle(position.ToPoint(), size.ToPoint());
            this.normalColor = Color.White;
            this.hoverColor = Color.Gray;
            this.textColor = Color.Black;
            this.textHoverColor = Color.White;
        }


        public void Update(GameTime gameTime)
        {

          // delay the click to prevent accidental clicks when game updates too fast for the click to unregister or something idk
            if (!_isDelayFinished)
            {
                _selectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_selectionTimer >= SelectionDelay)
                {
                    _isDelayFinished = true;
                }
            }
            else
            {
                _prevMouse = _currentMouse;
                _currentMouse = Mouse.GetState();

                isHovered = bounds.Contains(_currentMouse.Position);


                if (_currentMouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
                {
                    if (isHovered)
                        Click?.Invoke(this, new EventArgs());
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color buttonColor = isHovered ? hoverColor : normalColor;
            Color buttonTextColor = isHovered ? textHoverColor : textColor;

            // Draw button bounds
            Texture2D whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { buttonColor });
            spriteBatch.Draw(whitePixel, bounds, buttonColor);

            // Draw text
            Vector2 textSize = font.MeasureString(text);
            Vector2 textPosition = new Vector2(bounds.X + (bounds.Width - textSize.X) / 2, bounds.Y + (bounds.Height - textSize.Y) / 2);
            spriteBatch.DrawString(font, text, textPosition, buttonTextColor);
        }

    }

}
