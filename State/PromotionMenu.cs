using ChessGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;


namespace ChessGame.State
{
    public class PromotionMenu
    {
        private List<Button> buttons;
        private int selectedIndex;
        private SpriteFont font;
        private Vector2 position;
        private Vector2 buttonSize;
        private Rectangle menuBounds;
        private bool isVisible;

        public PromotionMenu(SpriteFont font, int tileSize, Vector2 screenSize)
        {
            this.font = font;
            this.buttonSize = new Vector2(tileSize); // Button size same as chess tile
            this.position = new Vector2((screenSize.X - buttonSize.X) / 2, (screenSize.Y - buttonSize.Y * 4) / 2); // Centering menu
            buttons = new List<Button>();
            selectedIndex = 0;
            isVisible = false;


            string[] pieceTypes = { "Queen", "Rook", "Bishop", "Knight" };
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                Vector2 buttonPosition = position + new Vector2(0, i * buttonSize.Y);
                var button = new Button(pieceTypes[i], font, buttonPosition, buttonSize);
                button.Click += OnButtonClick;
                buttons.Add(button);
            }

            // Define the bounds of the menu based on button size and count
            menuBounds = new Rectangle(position.ToPoint(), new Point((int)buttonSize.X, (int)buttonSize.Y * buttons.Count));
        }


        public void Update(GameTime gameTime)
        {
            if (!isVisible) return;

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
            }
        }

        private void HandlePromotion(string pieceType)
        {
            // Implement the promotion logic based on the selected piece type
            // For example, you might trigger a promotion event or update the game state here
            Console.WriteLine($"Promoted to: {pieceType}");
        }
    }
}
