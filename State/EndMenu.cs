using ChessGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ChessGame.State
{
  public class EndMenu
  {
    private readonly Button button;
    private string _winMessage;
    private readonly SpriteFont _font;
    private Vector2 _messagePosition;

    public bool Reset;


    public EndMenu(SpriteFont font, Vector2 screenSize, string winMessage)
    {
      Reset = false;
      _winMessage = winMessage;
      _font = font;

      var buttonSize = new Vector2(200, 50);
      var startPosition = new Vector2(screenSize.X / 2 - buttonSize.X / 2, screenSize.Y / 2 - buttonSize.Y);
      button = new Button("Reset", font, startPosition, buttonSize);

      var messageSize = _font.MeasureString(_winMessage);
      _messagePosition = new Vector2(screenSize.X / 2 - messageSize.X / 2, screenSize.Y / 4);


      button.Click += OnButtonClick;
    }

    private void OnButtonClick(object sender, EventArgs e)
    {
      if (sender is Button button)
      {
        Reset = true;
      }
    }

    public void Update(GameTime gameTime)
    {
      button.Update(gameTime);
    }


    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.DrawString(_font, _winMessage, _messagePosition, Color.White);


      button.Draw(spriteBatch);
    }
  }
}
