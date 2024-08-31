using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ChessGame.Models
{
  public static class ChessUtils
  {
    public static ChessPiece GetPieceAtTile(ChessTile tile, List<ChessPiece> chessPieces)
    {
      foreach (var piece in chessPieces)
      {
        if (piece.HomeTile == tile)
        {
          return piece;
        }
      }
      return null;
    }

    public static bool IsTileInBounds(int index, int boardSize)
    {
      return index >= 0 && index < boardSize;
    }


    public static bool IsPieceUnderAttack(ChessPiece testPiece, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var pieceTile = testPiece.HomeTile;
      foreach (var piece in chessPieces)
      {
        if (piece.PieceColor != testPiece.PieceColor)
        {
          var moves = piece.GetLegalMoves(chessBoard, chessPieces);
          if (moves.Contains(pieceTile))
          {
            return true;
          }
        }
      }
      return false;
    }


  }

  public class Pawn : ChessPiece
  {
    public Pawn(Player color, ChessTile homeTile) : base("pawn", color, homeTile) { }

    public override List<ChessTile> GetLegalMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int direction = (PieceColor == Player.White) ? -1 : 1;
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Move one tile forward
      int forwardIndex = currentIndex + direction * 8;
      if (ChessUtils.IsTileInBounds(forwardIndex, chessBoard.Count) && IsTileFree(forwardIndex, chessBoard, chessPieces))
      {
        validMoves.Add(chessBoard[forwardIndex]);
      }

      // Move two tiles forward if at starting position
      // if the pawns are starting position 
      if (!HasMoved)
      {
        int doubleForwardIndex = currentIndex + direction * 16;
        // check if both tiles infront of the pawn are free 
        if (ChessUtils.IsTileInBounds(doubleForwardIndex, chessBoard.Count) &&
            IsTileFree(doubleForwardIndex, chessBoard, chessPieces) &&
            IsTileFree(forwardIndex, chessBoard, chessPieces))
        {
          validMoves.Add(chessBoard[doubleForwardIndex]);
        }
      }
      // Capture moves
      int leftCaptureIndex = currentIndex + direction * 8 - 1;
      int rightCaptureIndex = currentIndex + direction * 8 + 1;

      // Check if left capture is within the same row
      if (leftCaptureIndex >= 0 && leftCaptureIndex / 8 == (currentIndex / 8) + direction)
      {
        if (IsValidCapture(leftCaptureIndex, chessBoard, chessPieces))
        {
          validMoves.Add(chessBoard[leftCaptureIndex]);
        }
      }

      // Check if right capture is within the same row
      if (rightCaptureIndex < chessBoard.Count && rightCaptureIndex / 8 == (currentIndex / 8) + direction)
      {
        if (IsValidCapture(rightCaptureIndex, chessBoard, chessPieces))
        {
          validMoves.Add(chessBoard[rightCaptureIndex]);
        }
      }
      // En passant
      int[] captureOffsets = { -1, 1 };
      foreach (var captureOffset in captureOffsets)
      {
        int adjacentIndex = currentIndex + captureOffset;
        // TODO: en passant can only happen immediately after moving the pawn, not after any other moves
        // TODO: make en passant capture the pawn
        // TODO: possible bug fixes with wrapping not sure ?

        // Get the adjacent piece
        var enPassantPawn = ChessUtils.GetPieceAtTile(chessBoard[adjacentIndex], chessPieces);

        // Check if there is a pawn of the opposite color adjacent to this pawn
        if (enPassantPawn != null &&
            enPassantPawn.Type == "pawn" &&
            enPassantPawn.PieceColor != this.PieceColor)
        {
          // Calculate the difference between the current tile and the previous tile
          if (Math.Abs(chessBoard.IndexOf(enPassantPawn.HomeTile) - chessBoard.IndexOf(enPassantPawn.LastTile)) == 16)
          {
            int captureIndex = adjacentIndex + direction * 8;
            validMoves.Add(chessBoard[captureIndex]);
          }

        }
      }

      return validMoves;
    }


    private static bool IsTileFree(int index, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      return ChessUtils.IsTileInBounds(index, chessBoard.Count) &&
             ChessUtils.GetPieceAtTile(chessBoard[index], chessPieces) == null;
    }

    private bool IsValidCapture(int index, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      return ChessUtils.IsTileInBounds(index, chessBoard.Count) &&
             ChessUtils.GetPieceAtTile(chessBoard[index], chessPieces) is ChessPiece piece &&
             piece.PieceColor != this.PieceColor;
    }
  }

  public class Rook : ChessPiece
  {
    public Rook(Player color, ChessTile homeTile) : base("rook", color, homeTile) { }

    public override List<ChessTile> GetLegalMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Directions: up, down, left, right
      int[] directions = { -8, 8, -1, 1 };

      // for every direction
      foreach (var direction in directions)
      {
        // for every tile in that direction
        for (int i = 1; i < 8; i++)
        {
          // i acts to navigate in every direction to every tile vertically and hori depeending on whcih direction it is 
          int nextIndex = currentIndex + direction * i;

          if (!IsValidMove(nextIndex, direction, currentIndex, chessBoard.Count))
            break;

          var targetTile = chessBoard[nextIndex];
          var occupyingPiece = ChessUtils.GetPieceAtTile(targetTile, chessPieces);

          if (occupyingPiece == null)
          {
            validMoves.Add(targetTile);
          }
          else
          {
            if (occupyingPiece.PieceColor != this.PieceColor)
            {
              validMoves.Add(targetTile);
            }
            // stop checking futher tiles because its blocked by an early piece 
            break;
          }
        }
      }

      return validMoves;
    }

    private static bool IsValidMove(int nextIndex, int direction, int currentIndex, int boardSize)
    {
      return ChessUtils.IsTileInBounds(nextIndex, boardSize) &&
             (direction == -1 || direction == 1 ? nextIndex / 8 == currentIndex / 8 : true);
    }
  }

  public class Knight : ChessPiece
  {
    public Knight(Player color, ChessTile homeTile) : base("knight", color, homeTile) { }

    public override List<ChessTile> GetLegalMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Possible moves
      int[] moveOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };

      foreach (var offset in moveOffsets)
      {
        int newIndex = currentIndex + offset;

        if (ChessUtils.IsTileInBounds(newIndex, chessBoard.Count) &&
            IsValidKnightMove(currentIndex, newIndex))
        {
          var targetTile = chessBoard[newIndex];
          var occupyingPiece = ChessUtils.GetPieceAtTile(targetTile, chessPieces);

          if (occupyingPiece == null || occupyingPiece.PieceColor != this.PieceColor)
          {
            validMoves.Add(targetTile);
          }
        }
      }

      return validMoves;
    }

    private static bool IsValidKnightMove(int currentIndex, int newIndex)
    {
      int oldX = currentIndex % 8;
      int oldY = currentIndex / 8;
      int newX = newIndex % 8;
      int newY = newIndex / 8;

      return (Math.Abs(oldX - newX) == 1 && Math.Abs(oldY - newY) == 2) ||
             (Math.Abs(oldX - newX) == 2 && Math.Abs(oldY - newY) == 1);
    }
  }

  public class Bishop : ChessPiece
  {
    public Bishop(Player color, ChessTile homeTile) : base("bishop", color, homeTile) { }

    public override List<ChessTile> GetLegalMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
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

          if (!IsValidMove(newIndex, direction, currentIndex, chessBoard.Count))
            break;

          var targetTile = chessBoard[newIndex];
          var occupyingPiece = ChessUtils.GetPieceAtTile(targetTile, chessPieces);

          if (occupyingPiece == null)
          {
            validMoves.Add(targetTile);
          }
          else
          {
            if (occupyingPiece.PieceColor != this.PieceColor)
            {
              validMoves.Add(targetTile);
            }
            break;
          }
        }
      }

      return validMoves;
    }

    private static bool IsValidMove(int newIndex, int direction, int currentIndex, int boardSize)
    {
      return ChessUtils.IsTileInBounds(newIndex, boardSize) &&
             Math.Abs(currentIndex % 8 - newIndex % 8) == Math.Abs(currentIndex / 8 - newIndex / 8);
    }
  }

  public class Queen : ChessPiece
  {
    public Queen(Player color, ChessTile homeTile) : base("queen", color, homeTile) { }

    public override List<ChessTile> GetLegalMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();

      // Combine moves of Rook and Bishop
      validMoves.AddRange(new Rook(PieceColor, HomeTile).GetLegalMoves(chessBoard, chessPieces));
      validMoves.AddRange(new Bishop(PieceColor, HomeTile).GetLegalMoves(chessBoard, chessPieces));

      return validMoves;
    }
  }

  public class King : ChessPiece
  {
    public King(Player color, ChessTile homeTile) : base("king", color, homeTile) { }

    public override List<ChessTile> GetLegalMoves(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      var validMoves = new List<ChessTile>();
      int currentIndex = chessBoard.IndexOf(HomeTile);

      if (currentIndex == -1) return validMoves;

      // Possible moves
      int[] moveOffsets = { -9, -8, -7, -1, 1, 7, 8, 9 };

      foreach (var offset in moveOffsets)
      {
        int newIndex = currentIndex + offset;

        if (ChessUtils.IsTileInBounds(newIndex, chessBoard.Count) &&
            IsValidKingMove(currentIndex, newIndex))
        {
          var targetTile = chessBoard[newIndex];
          var occupyingPiece = ChessUtils.GetPieceAtTile(targetTile, chessPieces);

          if (occupyingPiece == null || occupyingPiece.PieceColor != this.PieceColor) // check if tile has no piece or the piece of is opposite color 
            validMoves.Add(targetTile);
        }
      }

      // Add castling moves if eligible
      if (!HasMoved)
      {
        AddCastlingMoves(validMoves, chessBoard, chessPieces);
      }

      return validMoves;
    }

    private static bool IsValidKingMove(int currentIndex, int newIndex)
    {
      int oldX = currentIndex % 8;
      int oldY = currentIndex / 8;
      int newX = newIndex % 8;
      int newY = newIndex / 8;

      // checks if the distance between the current move and next move is one tile only
      return Math.Abs(oldX - newX) <= 1 && Math.Abs(oldY - newY) <= 1;
    }

    private void AddCastlingMoves(List<ChessTile> validMoves, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
    {
      int currentIndex = chessBoard.IndexOf(HomeTile);
      var rooks = chessPieces.FindAll(p => p.Type == "rook" && p.PieceColor == PieceColor && !p.HasMoved);

      ChessPiece qRook = null;
      ChessPiece kRook = null;

      foreach (var rook in rooks)
      {
        int rookIndex = chessBoard.IndexOf(rook.HomeTile);
        if (rookIndex < currentIndex)
          qRook = rook;  // Queenside rook
        else
          kRook = rook;  // Kingside rook
      }

      // Check kingside castling
      if (kRook != null)
      {
        bool isPathClear = true;
        for (int i = 1; i <= 2; i++)
        {
          if (ChessUtils.GetPieceAtTile(chessBoard[currentIndex + i], chessPieces) != null)
          {
            isPathClear = false;
            break;
          }
        }
        if (isPathClear)
          validMoves.Add(chessBoard[currentIndex + 2]);
      }

      // Check queenside castling
      if (qRook != null)
      {
        bool isPathClear = true;
        for (int i = 1; i <= 3; i++)
        {
          if (ChessUtils.GetPieceAtTile(chessBoard[currentIndex - i], chessPieces) != null)
          {
            isPathClear = false;
            break;
          }
        }
        if (isPathClear)
          validMoves.Add(chessBoard[currentIndex - 2]);
      }
    }
  }
}
