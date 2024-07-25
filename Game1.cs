using ChessGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ChessGame
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private List<ChessTile> _chessBoard;
		private List<ChessPiece> _chessPieces;
		private SpriteFont _font;
		private ChessPiece _selectedPiece;
		private Vector2 _mouseOffset; // For dragging pieces

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
			// intialize variables
			_chessBoard = new List<ChessTile>();
			_chessPieces = new List<ChessPiece>();

			base.Initialize();
		}

		private void InitializeChessBoard()
		{
			_chessBoard.Clear();
			// Calculate the size of each square based on the current window size
			int screenWidth = _graphics.PreferredBackBufferWidth;
			int screenHeight = _graphics.PreferredBackBufferHeight;
			int boardSize = Math.Min(screenWidth, screenHeight) * 7 / 8; // 7/8 of the smaller dimension
			int tileSize = boardSize / 8;

			int startX = (screenWidth - boardSize) / 2;
			int startY = (screenHeight - boardSize) / 2;

			bool isWhite = true;
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					string tileCoordinate = $"{(char)('a' + x)}{8 - y}";
					var position = new Vector2(startX + x * tileSize, startY + y * tileSize);
					var color = isWhite ? Color.White : Color.Gray;
					_chessBoard.Add(new ChessTile(position, color, tileSize, tileCoordinate, _font));
					isWhite = !isWhite;
				}
				isWhite = !isWhite;
			}
		}
		private void InitializeChessPieces()
		{
			// Piece setup for White
			for (int i = 0; i < 8; i++)
			{
				_chessPieces.Add(new ChessPiece("white-pawn", Color.White, _chessBoard[i + 8])); // Row 2
			}

			_chessPieces.Add(new ChessPiece("white-rook", Color.White, _chessBoard[0])); // a1
			_chessPieces.Add(new ChessPiece("white-knight", Color.White, _chessBoard[1])); // b1
			_chessPieces.Add(new ChessPiece("white-bishop", Color.White, _chessBoard[2])); // c1
			_chessPieces.Add(new ChessPiece("white-queen", Color.White, _chessBoard[3])); // d1
			_chessPieces.Add(new ChessPiece("white-king", Color.White, _chessBoard[4])); // e1
			_chessPieces.Add(new ChessPiece("white-bishop", Color.White, _chessBoard[5])); // f1
			_chessPieces.Add(new ChessPiece("white-knight", Color.White, _chessBoard[6])); // g1
			_chessPieces.Add(new ChessPiece("white-rook", Color.White, _chessBoard[7])); // h1

			// Piece setup for Black
			for (int i = 0; i < 8; i++)
			{
				_chessPieces.Add(new ChessPiece("black-pawn", Color.Black, _chessBoard[i + 48])); // Row 7
			}

			_chessPieces.Add(new ChessPiece("black-rook", Color.Black, _chessBoard[56])); // a8
			_chessPieces.Add(new ChessPiece("black-knight", Color.Black, _chessBoard[57])); // b8
			_chessPieces.Add(new ChessPiece("black-bishop", Color.Black, _chessBoard[58])); // c8
			_chessPieces.Add(new ChessPiece("black-queen", Color.Black, _chessBoard[59])); // d8
			_chessPieces.Add(new ChessPiece("black-king", Color.Black, _chessBoard[60])); // e8
			_chessPieces.Add(new ChessPiece("black-bishop", Color.Black, _chessBoard[61])); // f8
			_chessPieces.Add(new ChessPiece("black-knight", Color.Black, _chessBoard[62])); // g8
			_chessPieces.Add(new ChessPiece("black-rook", Color.Black, _chessBoard[63])); // h8
		}
		private ChessTile GetTileAtPosition(Vector2 position)
		{
			foreach (var tile in _chessBoard)
			{
				if (tile.Bounds.Contains((int)position.X, (int)position.Y))
				{
					return tile;
				}
			}
			return null;
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_font = Content.Load<SpriteFont>("fonts/DejaVuSans");

			InitializeChessBoard();
			InitializeChessPieces();

			// Load all tile textures
			foreach (var tile in _chessBoard)
			{
				tile.LoadContent(GraphicsDevice);
			}

			foreach (var piece in _chessPieces)
			{
				piece.LoadContent(GraphicsDevice, Content);
			}

		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			var mouseState = Mouse.GetState();
			var mousePosition = new Vector2(mouseState.X, mouseState.Y);
			bool mouseClickHandled = false;



			if (mouseState.LeftButton == ButtonState.Pressed && mouseClickHandled == false)
			{
				// if we have no piece selected select one 
				if (_selectedPiece == null)
				{
					foreach (var piece in _chessPieces)
					{
						if (piece._homeTile.Bounds.Contains(mousePosition))
						{
							_selectedPiece = piece;
							_mouseOffset = mousePosition - new Vector2(piece._homeTile.Bounds.X, piece._homeTile.Bounds.Y);
							piece.IsSelected = true;
							break;
						}
					}
				}
				// piece is already selected
				else
				{
					var newTile = GetTileAtPosition(mousePosition);
					if (newTile != null)
					{
						_selectedPiece.MoveTo(newTile);
						_selectedPiece.IsSelected = false;
						_selectedPiece = null;
					}
				}
			}
			else if (mouseState.LeftButton == ButtonState.Released)
			{
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();
			foreach (var square in _chessBoard)
			{
				square.Draw(_spriteBatch);
			}
			foreach (var piece in _chessPieces)
			{
				piece.Draw(_spriteBatch);
			}
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}