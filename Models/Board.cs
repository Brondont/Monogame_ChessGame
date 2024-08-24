using ChessGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ChessGame.Models
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
    private List<ChessTile> _legalMoves;
    private MouseState _currentMouse;
    private MouseState _prevMouse;

    public string GameResult;
    public Player PlayerTurn { get; private set; } = Player.White;
    public bool IsInCheck { get; private set; } = false;

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
      int screenHeight = 800; // Default height
      int boardSize = Math.Min(screenWidth, screenHeight);
      int tileSize = boardSize / 8;

      int startX = (screenWidth - boardSize);
      int startY = (screenHeight - boardSize);

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

    public void Update(GameTime gameTime)
    {
      _prevMouse = _currentMouse;
      _currentMouse = Mouse.GetState();

      var mousePosition = new Vector2(_currentMouse.X, _currentMouse.Y);

      if (_prevMouse.LeftButton == ButtonState.Released && _currentMouse.LeftButton == ButtonState.Pressed)
      {
        if (_selectedPiece == null)
        {
          SelectPiece(mousePosition);
        }
        else
        {
          MoveSelectedPiece(mousePosition);
        }
      }
    }

    private void SelectPiece(Vector2 mousePosition)
    {
      foreach (var piece in _chessPieces)
      {
        if (piece.HomeTile.Bounds.Contains(mousePosition) && piece.PieceColor == PlayerTurn)
        {
          _selectedPiece = piece;
          piece.IsSelected = true;

          _legalMoves = _selectedPiece.GetLegalSafeMoves(_chessBoard, _chessPieces);

          HighlightTiles(_legalMoves, true);
          break;
        }
      }
    }

    private void ChangeSelectedPiece(ChessPiece capturedPiece)
    {
      HighlightTiles(_legalMoves, false);

      _selectedPiece.IsSelected = false;
      _selectedPiece = capturedPiece;

      _legalMoves = _selectedPiece.GetLegalSafeMoves(_chessBoard, _chessPieces);
      HighlightTiles(_legalMoves, true);
    }


    private void MoveSelectedPiece(Vector2 mousePosition)
    {
      var newTile = GetTileAtPosition(mousePosition);
      if (newTile == null) return;

      var capturedPiece = GetPieceAtTile(newTile);
      // if the tile clicked is of the same color as the player select it instead
      if (capturedPiece != null && capturedPiece.PieceColor == _selectedPiece.PieceColor)
      {
        ChangeSelectedPiece(capturedPiece);
        return;
      }

      if (_legalMoves.Contains(newTile) && _selectedPiece.HomeTile != newTile)
      {
        if (capturedPiece != null)
        {
          _chessPieces.Remove(capturedPiece);
        }
        _selectedPiece.MoveTo(newTile);

        UpdatePlayerTurnAndCheckStatus();

        HighlightTiles(_legalMoves, false);
        _selectedPiece.IsSelected = false;
        _selectedPiece = null;
      }
    }

    private static void HighlightTiles(List<ChessTile> tiles, bool highlight)
    {
      foreach (var tile in tiles)
      {
        tile.IsHighlighted = highlight;
      }
    }

    private void UpdatePlayerTurnAndCheckStatus()
    {
      PlayerTurn = PlayerTurn == Player.Black ? Player.White : Player.Black;
      IsInCheck = false;

      foreach (var tile in _selectedPiece.GetLegalSafeMoves(_chessBoard, _chessPieces))
      {
        var tilePiece = GetPieceAtTile(tile);
        if (tilePiece != null && tilePiece.PieceColor == PlayerTurn && tilePiece.Type == "king")
        {
          IsInCheck = true;
          IsMate();
          break;
        }
      }
    }

    private void IsMate()
    {
      foreach (var piece in _chessPieces)
      {
        if (piece.PieceColor == PlayerTurn)
        {
          var legalMoves = piece.GetLegalSafeMoves(_chessBoard, _chessPieces);
          if (legalMoves.Count != 0)
          {
            return;
          }
        }
      }
      // if it went over all the pieces and there is no legal move left for player its checkmate 
      GameResult = PlayerTurn == Player.Black ? "White won!" : "Black Won !";
      PlayerTurn = Player.None;
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
