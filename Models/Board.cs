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

public static class Globals
{
  public static int TurnCount = 0;
  public static int MoveRule50 = 0;
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

      int startX = screenWidth - boardSize;
      int startY = screenHeight - boardSize;

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
      ChessTile selectedTile = GetTileAtPosition(mousePosition);
      ChessPiece piece = GetPieceAtTile(selectedTile);
      if (piece == null || piece.PieceColor != PlayerTurn)
        return;
      _selectedPiece = piece;
      _selectedPiece.IsSelected = true;

      _legalMoves = _selectedPiece.GetLegalSafeMoves(_chessBoard, _chessPieces);

      HighlightTiles(_legalMoves, true);
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
      // if move is legal
      if (_legalMoves.Contains(newTile))
      {
        int newTileIndex = _chessBoard.IndexOf(newTile);
        int selectedPieceIndex = _chessBoard.IndexOf(_selectedPiece.HomeTile);
        // 50 move rule increment
        Globals.MoveRule50++;

        // Check if the selected piece is a king and if the move is a castling move
        if (_selectedPiece.Type == "king" && Math.Abs(newTileIndex - selectedPieceIndex) == 2)
        {
          ChessPiece rook = newTileIndex < selectedPieceIndex
         ? GetPieceAtTile(_chessBoard[selectedPieceIndex - 4]) // Queenside
         : GetPieceAtTile(_chessBoard[selectedPieceIndex + 3]); // Kingside

         rook.MoveTo(_chessBoard[selectedPieceIndex + (newTileIndex < selectedPieceIndex ? 1 : -1)]);
        } else if (_selectedPiece.Type == "pawn") {
          Globals.TurnCount = 0;
        }

        // Standard move, handle potential capture and move the piece
        _selectedPiece.MoveTo(newTile);
        selectedPieceIndex = _chessBoard.IndexOf(_selectedPiece.HomeTile);

        if (capturedPiece != null)
        {
          _chessPieces.Remove(capturedPiece);
          Globals.MoveRule50 = 0; // reset counter 
        } else {
           // check if it was an en passant and if the move was a capture move
          // its a capture move if a pawn exists behind the moved pawn
          if(_selectedPiece.Type == "pawn") {
            // check if theres a pawn behind to see if its a capture move
            int offset = _selectedPiece.PieceColor == Player.White ? -8 : 8;
            capturedPiece = GetPieceAtTile(_chessBoard[selectedPieceIndex - offset]);
            // if we find a piece it will 100% be the en passant pawn so we capture it 
            if(capturedPiece != null) {
              _chessPieces.Remove(capturedPiece);
            }
            Globals.MoveRule50 = 0;
          }
        }
        UpdatePlayerTurnAndCheckStatus();

        // Clear highlighted tiles and reset the selected piece
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
      // get the legal moves for the piece after it was moved
      foreach (var tile in _selectedPiece.GetLegalSafeMoves(_chessBoard, _chessPieces))
      {
        var tilePiece = GetPieceAtTile(tile);
        // check if one of the legal moves put the enemies king in check
        if (tilePiece != null && tilePiece.PieceColor == PlayerTurn && tilePiece.Type == "king")
        {
          IsInCheck = true;
          // check if that check is also a mate 
          IsMate();
          break;
        }
      }
        // check draw 

      bool draw = true;

      foreach (var piece in _chessPieces)
      {
        if (piece.PieceColor == PlayerTurn && piece.GetLegalSafeMoves(_chessBoard, _chessPieces).Count > 0) {
          draw = false;
          break; // Exit early since a legal move exists
          }
      }

      // Set the game result based on whether a draw condition was met
      if (draw)
      {
        GameResult = "Draw!";
        PlayerTurn = Player.None;
      }
      // check 50 rule move 
      if (Globals.MoveRule50 == 50) {
        GameResult = "Draw by 50 move rule!";
        PlayerTurn = Player.None;
      }
    }

    private void IsMate()
    {
      // had to use a for loop here because it kept throwing an error when using foreach about enumeration while collection
      // is being modified this took an hour to fix fucking kill me
      for (int i = 0; i < _chessPieces.Count; i++)
      {
        if (_chessPieces[i].PieceColor == PlayerTurn)
        {
          // check if there is any legal move that can save the king 
          var legalMoves = _chessPieces[i].GetLegalSafeMoves(_chessBoard, _chessPieces);
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
