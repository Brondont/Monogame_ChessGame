using ChessGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Board _board;
		private MouseState mouseState;
		private MouseState previousMouseState;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			_graphics.PreferredBackBufferWidth = 800; // Default width
			_graphics.PreferredBackBufferHeight = 600; // Default height
			_graphics.ApplyChanges();
			Window.AllowUserResizing = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			SpriteFont font = Content.Load<SpriteFont>("fonts/DejaVuSans");
			_board = new Board(font);
			_board.LoadContent(GraphicsDevice, Content);
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			previousMouseState = mouseState;
			mouseState = Mouse.GetState();

			_board.Update(mouseState, previousMouseState);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();
			_board.Draw(_spriteBatch);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
