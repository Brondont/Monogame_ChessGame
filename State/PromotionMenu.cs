using ChessGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using ChessGame.Models;

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
        private List<Texture2D> textures;

        private string[] pieceTypes = { "queen", "rook", "bishop", "knight" };

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
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                Vector2 buttonPosition = position + new Vector2(0, i * buttonSize.Y);
                var button = new Button("", font, buttonPosition, buttonSize);
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

        public void LoadContent(ContentManager content)
        {
            textures = new List<Texture2D>(pieceTypes.Length); // Initialize textures list
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                string texturePath = "pieces/" + $"{_pieceToPromote.PieceColor}-{pieceTypes[i]}";// Adjust path format if needed
                textures.Add(content.Load<Texture2D>(texturePath));
            }
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
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Draw(spriteBatch);
                // Draw the texture for the button at the button's location
                if (i < textures.Count)
                {
                    Vector2 buttonPosition = position + new Vector2(0, i * buttonSize.Y);
                    spriteBatch.Draw(textures[i], new Rectangle(buttonPosition.ToPoint(), buttonSize.ToPoint()), Color.White);
                }
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
