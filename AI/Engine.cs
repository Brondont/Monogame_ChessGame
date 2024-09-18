using ChessGame.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ChessGame.AI
{

    public class ChessEngine
    {
        private Player _color;
        private Random _random;

        public ChessEngine(Player color)
        {
            _color = color;
            _random = new Random();
        }


        public void MakeRandomMove(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            var playerPieces = chessPieces.Where(p => p.PieceColor == _color).ToList();


            ChessPiece selectedPiece = null;
            List<ChessTile> legalMoves = null;

            while (selectedPiece == null || legalMoves.Count == 0)
            {
                // Select a random piece
                selectedPiece = playerPieces[_random.Next(playerPieces.Count)];

                // Get legal moves for the selected piece
                legalMoves = selectedPiece.GetLegalSafeMoves(chessBoard, chessPieces);
            }

            // Select a random legal move for the selected piece
            var targetTile = legalMoves[_random.Next(legalMoves.Count)];
            
            selectedPiece.MoveTo(targetTile, chessBoard, chessPieces);
        }

        public void MakeCaptureOnlyMove(List<ChessTile> chessBoard, List<ChessPiece> chessPieces){}
      
    }
}

