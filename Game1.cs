using System;
using ChessGame.Models;
using ChessGame.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Menu _menu;
		private Board _board;
		private bool isGameStarted;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			_graphics.PreferredBackBufferWidth = 800; // Default width
			_graphics.PreferredBackBufferHeight = 600; // Default height
			_graphics.ApplyChanges();
			Window.AllowUserResizing = true;
			isGameStarted = false;
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			SpriteFont font = Content.Load<SpriteFont>("fonts/DejaVuSans");

			// Initialize the menu with the font and screen size
			_menu = new Menu(font, new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (!isGameStarted)
			{
				// update menu for clicks
				_menu.Update(gameTime);

				StartGame();
			}
			else
			{
				_board.Update();
			}

			base.Update(gameTime);
		}

		private void StartGame()
		{
			SpriteFont font = Content.Load<SpriteFont>("fonts/DejaVuSans");

			// Initialize the board based on the selected menu option
			Console.Write(_menu.GetSelectedIndex());
			switch (_menu.GetSelectedIndex())
			{
				case 0:
					// Start 1v1 game
					_board = new Board(font);
					break;
				case 1:
					// Start 1v Stockfish game
					_board = new Board(font);
					break;
				case 2:
					// Start 1v Personal Engine game
					_board = new Board(font);
					break;
				default:
					return;
			}

			_board.LoadContent(GraphicsDevice, Content);
			isGameStarted = true;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();
			if (!isGameStarted)
			{
				// Draw the menu
				_menu.Draw(_spriteBatch);
			}
			else
			{
				// Draw the board
				_board.Draw(_spriteBatch);
			}
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
