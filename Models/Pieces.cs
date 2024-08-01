using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ChessGame.Models
{
  public class Pawn : ChessPiece
  {
    public Pawn(Player color, ChessTile homeTile) : base("pawn", color, homeTile) { }

    public override List<ChessTile> GetValidMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int direction = (PieceColor == Player.White) ? -1 : 1;
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Move one tile forward
      int forwardIndex = currentIndex + direction * 8;
      if (IsTileFree(forwardIndex, chessBoard, chessPieces))
      {
        validMoves.Add(chessBoard[forwardIndex]);
      }

      // Move two tiles forward if at starting position
      if ((PieceColor == Player.White && HomeTile.TileCoordinate[1] == '2') ||
          (PieceColor == Player.Black && HomeTile.TileCoordinate[1] == '7'))
      {
        int doubleForwardIndex = currentIndex + direction * 16;
        if (IsTileFree(doubleForwardIndex, chessBoard, chessPieces) && IsTileFree(forwardIndex, chessBoard, chessPieces))
        {
          validMoves.Add(chessBoard[doubleForwardIndex]);
        }
      }

      // Capture moves
      int leftCaptureIndex = currentIndex + direction * 8 - 1;
      int rightCaptureIndex = currentIndex + direction * 8 + 1;

      if (IsValidCapture(leftCaptureIndex, chessBoard, chessPieces))
      {
        validMoves.Add(chessBoard[leftCaptureIndex]);
      }

      if (IsValidCapture(rightCaptureIndex, chessBoard, chessPieces))
      {
        validMoves.Add(chessBoard[rightCaptureIndex]);
      }

      // TODO: Implement en passant capture

      return validMoves;
    }

    private bool IsTileFree(int index, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      if (index < 0 || index >= chessBoard.Count)
      {
        return false;
      }

      foreach (var piece in chessPieces)
      {
        if (piece.HomeTile == chessBoard[index])
        {
          return false;
        }
      }
      return true;
    }

    private bool IsValidCapture(int index, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      if (index < 0 || index >= chessBoard.Count)
      {
        return false;
      }

      foreach (var piece in chessPieces)
      {
        if (piece.HomeTile == chessBoard[index] && piece.PieceColor != this.PieceColor)
        {
          return true;
        }
      }
      return false;
    }
  }

  public class Rook : ChessPiece
  {
    public Rook(Player color, ChessTile homeTile) : base("rook", color, homeTile) { }

    public override List<ChessTile> GetValidMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Vertical and horizontal moves
      for (int i = 1; i < 8; i++)
      {
        int upIndex = currentIndex - i * 8;
        int downIndex = currentIndex + i * 8;
        int leftIndex = currentIndex - i;
        int rightIndex = currentIndex + i;

        if (upIndex >= 0) validMoves.Add(chessBoard[upIndex]);
        if (downIndex < chessBoard.Count) validMoves.Add(chessBoard[downIndex]);
        if (leftIndex >= 0 && leftIndex / 8 == currentIndex / 8) validMoves.Add(chessBoard[leftIndex]);
        if (rightIndex < chessBoard.Count && rightIndex / 8 == currentIndex / 8) validMoves.Add(chessBoard[rightIndex]);
      }
      return validMoves;
    }
  }

  public class Knight : ChessPiece
  {
    public Knight(Player color, ChessTile homeTile) : base("knight", color, homeTile) { }

    public override List<ChessTile> GetValidMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Possible moves
      int[] moveOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };

      foreach (var offset in moveOffsets)
      {
        int newIndex = currentIndex + offset;
        if (newIndex >= 0 && newIndex < chessBoard.Count)
        {
          int oldX = currentIndex % 8;
          int oldY = currentIndex / 8;
          int newX = newIndex % 8;
          int newY = newIndex / 8;
          if (Math.Abs(oldX - newX) == 1 && Math.Abs(oldY - newY) == 2 ||
              Math.Abs(oldX - newX) == 2 && Math.Abs(oldY - newY) == 1)
          {
            validMoves.Add(chessBoard[newIndex]);
          }
        }
      }
      return validMoves;
    }
  }
  public class Bishop : ChessPiece
  {
    public Bishop(Player color, ChessTile homeTile) : base("bishop", color, homeTile) { }

    public override List<ChessTile> GetValidMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Define directions for diagonal moves
      int[] directions = { 9, 7, -7, -9 };

      foreach (var direction in directions)
      {
        for (int i = 1; i < 8; i++)
        {
          int newIndex = currentIndex + i * direction;

          // Check if the new index is out of bounds or invalid
          if (newIndex < 0 || newIndex >= chessBoard.Count)
            break;

          // Check if the move wraps around the board horizontally
          int oldX = currentIndex % 8;
          int newX = newIndex % 8;

          if (Math.Abs(oldX - newX) != i)
            break;

          validMoves.Add(chessBoard[newIndex]);

        }
      }

      return validMoves;
    }
  }


  public class Queen : ChessPiece
  {
    public Queen(Player color, ChessTile homeTile) : base("queen", color, homeTile) { }

    public override List<ChessTile> GetValidMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();

      // Combine moves of Rook and Bishop
      validMoves.AddRange(new Rook(PieceColor, HomeTile).GetValidMoves(chessBoard, chessPieces));
      validMoves.AddRange(new Bishop(PieceColor, HomeTile).GetValidMoves(chessBoard, chessPieces));
      return validMoves;
    }
  }

  public class King : ChessPiece
  {
    public King(Player color, ChessTile homeTile) : base("king", color, homeTile) { }

    public override List<ChessTile> GetValidMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Possible moves
      int[] moveOffsets = { -9, -8, -7, -1, 1, 7, 8, 9 };

      foreach (var offset in moveOffsets)
      {
        int newIndex = currentIndex + offset;
        if (newIndex >= 0 && newIndex < chessBoard.Count)
        {
          int oldX = currentIndex % 8;
          int oldY = currentIndex / 8;
          int newX = newIndex % 8;
          int newY = newIndex / 8;
          if (Math.Abs(oldX - newX) <= 1 && Math.Abs(oldY - newY) <= 1)
          {
            validMoves.Add(chessBoard[newIndex]);
          }
        }
      }
      return validMoves;
    }
  }
}
