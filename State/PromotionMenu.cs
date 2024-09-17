using ChessGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ChessGame.Models;
using System;

namespace ChessGame.State
{
    public class PromotionMenu
    {
        private List<Button> buttons;
        private int selectedIndex;
        public ChessPiece _pieceToPromote;
        private SpriteFont font;
        private Vector2 position;
        private Vector2 buttonSize;
        private Rectangle menuBounds;

        public PromotionMenu(ChessPiece pieceToPromote, SpriteFont font, int tileSize, Vector2 pos, int screenHeight)
        {
            _pieceToPromote = pieceToPromote;
            this.font = font;
            this.buttonSize = new Vector2(tileSize);
            buttons = new List<Button>();
            selectedIndex = -1;

            // Adjust the position to ensure the menu stays within the screen
            AdjustPosition(pos, screenHeight, tileSize);

            // Create buttons for promotion options
            string[] pieceTypes = { "Queen", "Rook", "Bishop", "Knight" };
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                Vector2 buttonPosition = position + new Vector2(0, i * buttonSize.Y);
                var button = new Button(pieceTypes[i], font, buttonPosition, buttonSize);
                button.Click += OnButtonClick;
                buttons.Add(button);
            }

            menuBounds = new Rectangle(position.ToPoint(), new Point((int)buttonSize.X, (int)buttonSize.Y * buttons.Count));
        }

        private void AdjustPosition(Vector2 pos, int screenHeight, int tileSize)
        {
            int menuHeight = tileSize * 4;
            float difference = pos.Y + menuHeight - screenHeight;
            position = difference > 0 ? new Vector2(pos.X, pos.Y - difference) : pos;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var button in buttons)
            {
                button.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in buttons)
            {
                button.Draw(spriteBatch);
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                selectedIndex = buttons.IndexOf(button);
            }
        }

        public int GetSelectedIndex()
        {
            return selectedIndex;
        }

    }
}
