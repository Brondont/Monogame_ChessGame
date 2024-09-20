using System;
using ChessGame.Models;
using ChessGame.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

enum GameState
{
    MainMenu,
    Gameplay,
    EndOfGame,
}

namespace ChessGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Menu _menu;
        private Board _board;
        private EndMenu _endMenu;
        private GameState _state;
        private SpriteFont _font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            Window.AllowUserResizing = true;
            _state = GameState.MainMenu;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("fonts/DejaVuSans");
            _menu = new Menu(_font, GetViewportSize());
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (_state)
            {
                case GameState.MainMenu:
                    UpdateMenu(gameTime);
                    break;
                case GameState.Gameplay:
                    UpdateBoard(gameTime);
                    break;
                case GameState.EndOfGame:
                    UpdateEndOfGame(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateMenu(GameTime gameTime)
        {
            _menu.Update(gameTime);

            if (_menu.GetSelectedIndex() != -1)
                StartGame();
        }

        private void UpdateBoard(GameTime gameTime)
        {
            _board?.Update(gameTime);

            if (_board?.PlayerTurn == Player.None)
            {
                _state = GameState.EndOfGame;
                _endMenu = new EndMenu(_font, GetViewportSize(), _board.GameResult);
            }
        }

        private void UpdateEndOfGame(GameTime gameTime)
        {
            _endMenu?.Update(gameTime);

            if (_endMenu.Reset)
            {
                ResetGame();
            }
        }

        private void StartGame()
        {
            switch (_menu.GetSelectedIndex())
            {
                case 0:
                    _board = new Board(_font, GameMode.PlayerVsPlayer); 
                    break;
                case 1:
                    _board = new Board(_font, GameMode.PlayerVsRandomMoves);
                    break;
                case 2:
                    _board = new Board(_font, GameMode.PlayerVsCaptureOnlyMoves);
                    break;
                case 3:
                    _board = new Board(_font, GameMode.PlayerVsMinMaxMovesV1);
                    break;
                case 4:
                    _board = new Board(_font, GameMode.RandomMovesVsCaptureOnlyMoves);
                    break;
                case 5:
                    _board = new Board(_font, GameMode.CaptureOnlyMovesVsMinMaxMovesV1);
                    break;
            }
            _board?.LoadContent(GraphicsDevice, Content);
            _state = GameState.Gameplay;
        }


        private void ResetGame()
        {
            // Menu re-initialization should be handled carefully since it depends on loaded content.
            _menu = new Menu(_font, GetViewportSize());
            _board = null;
            _endMenu = null;
            _state = GameState.MainMenu;
        }

        private Vector2 GetViewportSize()
        {
            return new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            switch (_state)
            {
                case GameState.MainMenu:
                    _menu?.Draw(_spriteBatch);
                    break;
                case GameState.Gameplay:
                    _board?.Draw(_spriteBatch);
                    break;
                case GameState.EndOfGame:
                    _board?.Draw(_spriteBatch);
                    _endMenu?.Draw(_spriteBatch);
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
