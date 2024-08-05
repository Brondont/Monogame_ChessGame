using ChessGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ChessGame
{
  public enum Player
  {
    Black,
    White,
    None,
  }

  public class Board
  {
    private List<ChessTile> _chessBoard;
    private List<ChessPiece> _chessPieces;
    private SpriteFont _font;
    private ChessPiece _selectedPiece;
    private List<ChessTile> _highlightedTiles;
    public Player _playerTurn { get; private set; } = Player.White;

    public Board(SpriteFont font)
    {
      _chessBoard = new List<ChessTile>();
      _chessPieces = new List<ChessPiece>();
      _font = font;

      InitializeChessBoard();
      InitializeChessPieces();
    }

    private void InitializeChessBoard()
    {
      _chessBoard.Clear();
      int screenWidth = 800; // Default width
      int screenHeight = 600; // Default height
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
        _chessPieces.Add(new Pawn(Player.White, _chessBoard[i + 48])); // Row 7
      }

      _chessPieces.Add(new Rook(Player.White, _chessBoard[56])); // a8
      _chessPieces.Add(new Knight(Player.White, _chessBoard[57])); // b8
      _chessPieces.Add(new Bishop(Player.White, _chessBoard[58])); // c8
      _chessPieces.Add(new Queen(Player.White, _chessBoard[59])); // d8
      _chessPieces.Add(new King(Player.White, _chessBoard[60])); // e8
      _chessPieces.Add(new Bishop(Player.White, _chessBoard[61])); // f8
      _chessPieces.Add(new Knight(Player.White, _chessBoard[62])); // g8
      _chessPieces.Add(new Rook(Player.White, _chessBoard[63])); // h8

      // Piece setup for Black
      for (int i = 0; i < 8; i++)
      {
        _chessPieces.Add(new Pawn(Player.Black, _chessBoard[i + 8])); // Row 2
      }

      _chessPieces.Add(new Rook(Player.Black, _chessBoard[0])); // a1
      _chessPieces.Add(new Knight(Player.Black, _chessBoard[1])); // b1
      _chessPieces.Add(new Bishop(Player.Black, _chessBoard[2])); // c1
      _chessPieces.Add(new Queen(Player.Black, _chessBoard[3])); // d1
      _chessPieces.Add(new King(Player.Black, _chessBoard[4])); // e1
      _chessPieces.Add(new Bishop(Player.Black, _chessBoard[5])); // f1
      _chessPieces.Add(new Knight(Player.Black, _chessBoard[6])); // g1
      _chessPieces.Add(new Rook(Player.Black, _chessBoard[7])); // h1
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

    private ChessPiece GetPieceAtTile(ChessTile tile)
    {
      foreach (var piece in _chessPieces)
      {
        if (piece.HomeTile == tile)
        {
          return piece;
        }
      }
      return null;
    }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
    {
      // Load all tile textures
      foreach (var tile in _chessBoard)
      {
        tile.LoadContent(graphicsDevice);
      }

      // Load all piece textures
      foreach (var piece in _chessPieces)
      {
        piece.LoadContent(graphicsDevice, content);
      }
    }

    public void Update(MouseState mouseState, MouseState previousMouseState)
    {
      var mousePosition = new Vector2(mouseState.X, mouseState.Y);

      if (previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
      {
        if (_selectedPiece == null)
        {
          foreach (var piece in _chessPieces)
          {
            if (piece.HomeTile.Bounds.Contains(mousePosition))
            {
              // Check if the clicked piece belongs to the active player 
              if (_playerTurn != piece.PieceColor) return;

              _selectedPiece = piece;
              piece.IsSelected = true;
              // Highlight the legal tiles that the piece can move to after selecting it 
              _highlightedTiles = _selectedPiece.GetValidMoves(_chessBoard, _chessPieces);
              foreach (var tile in _highlightedTiles)
              {
                tile.IsHighlighted = true;
              }
              break;
            }
          }
        }
        // Piece is already selected
        else
        {
          // get the new tile 
          var newTile = GetTileAtPosition(mousePosition);


          if (newTile == null)
            return;

          // get piece in the new tile 
          var capturedPiece = GetPieceAtTile(newTile);


          // change the selected piece if the piece is of the same color 
          if (capturedPiece != null)
          {
            if (capturedPiece.PieceColor == _selectedPiece.PieceColor)
            {
              // clear previous highlights
              foreach (var tile in _highlightedTiles)
              {
                tile.IsHighlighted = false;
              }

              _selectedPiece.IsSelected = false;
              _selectedPiece = capturedPiece;
              _highlightedTiles = _selectedPiece.GetValidMoves(_chessBoard, _chessPieces);
              foreach (var tile in _highlightedTiles)
              {
                tile.IsHighlighted = true;
              }
              return;

            }
          }
          // if its not the same color piece this runs 
          if (_highlightedTiles.Contains(newTile) && _selectedPiece.HomeTile != newTile)
          {
            // capture piece if it exists on new tile 
            if (capturedPiece != null)
            {
              _chessPieces.Remove(capturedPiece);
            }
            // move selected piece to new position
            _selectedPiece.MoveTo(newTile);
            //  switch player
            foreach (var tile in _highlightedTiles)
            {
              tile.IsHighlighted = false;
            }
            _playerTurn = _playerTurn == Player.Black ? Player.White : Player.Black;
            _selectedPiece.IsSelected = false;
            _selectedPiece = null;
          }
        }
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      foreach (var square in _chessBoard)
      {
        square.Draw(spriteBatch);
      }

      foreach (var piece in _chessPieces)
      {
        piece.Draw(spriteBatch);
      }
    }
  }
}
