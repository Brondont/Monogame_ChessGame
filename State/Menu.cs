using ChessGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ChessGame.State
{
    public class Menu
    {
        private List<Button> buttons;
        private int selectedIndex;

        public Menu(SpriteFont font, Vector2 screenSize)
        {
            buttons = new List<Button>();

            // Define button positions and sizes
            selectedIndex = -1;
            var buttonSize = new Vector2(400, 50);
            var startPosition = new Vector2(screenSize.X / 2 - buttonSize.X / 2, screenSize.Y / 2 - buttonSize.Y);

            // Create buttons
            var button1 = new Button("1 VS 1", font, startPosition, buttonSize);
            var button2 = new Button("1 VS RandomMoves", font, startPosition + new Vector2(0, 60), buttonSize);
            var button3 = new Button("1 VS Capture only moves", font, startPosition + new Vector2(0, 120), buttonSize);
            var button4 = new Button("1 VS Min max algorithme V2", font, startPosition + new Vector2(0, 180), buttonSize);
            var button5 = new Button("Random moves VS Capture only moves", font, startPosition + new Vector2(0, 240), buttonSize);
            var button6 = new Button("Capture only moves VS Min max algorithme V1", font, startPosition + new Vector2(0, 300), buttonSize);

            // Add buttons to the list
            buttons.Add(button1);
            buttons.Add(button2);
            buttons.Add(button3);
            buttons.Add(button4);
            buttons.Add(button5);
            buttons.Add(button6);

            // Subscribe to button click events
            button1.Click += OnButtonClick;
            button2.Click += OnButtonClick;
            button3.Click += OnButtonClick;
            button4.Click += OnButtonClick;
            button5.Click += OnButtonClick;
            button6.Click += OnButtonClick;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                selectedIndex = buttons.IndexOf(button);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var button in buttons)
                button.Update(gameTime);
        }

        public int GetSelectedIndex()
        {
            return selectedIndex;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in buttons)
                button.Draw(spriteBatch);
        }
    }
}
