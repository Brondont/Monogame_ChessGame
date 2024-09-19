using ChessGame.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame.AI
{

    public class ChessEngine
    {
        public Player Color;
        private Random _random;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;


        public ChessEngine(Player color, GraphicsDevice graphicsDevice, ContentManager content)
        {
            Color = color;
            _random = new Random();
            _graphicsDevice = graphicsDevice;
            _content = content;
        }

        private void CheckPawnPromotion(ChessPiece selectedPiece, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            int selectedPieceIndex = chessBoard.IndexOf(selectedPiece.HomeTile);
            if (selectedPiece.Type == "pawn" && (selectedPieceIndex > 55 || selectedPieceIndex < 8))
            {
                // for now we will just auto promote to queen
                ChessPiece queen = new Queen(selectedPiece.PieceColor, selectedPiece.HomeTile);
                queen.LoadContent(_graphicsDevice, _content);
                chessPieces.Add(queen);
                chessPieces.Remove(selectedPiece);
            }

        }

        public void MakeRandomMove(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            var playerPieces = chessPieces.Where(p => p.PieceColor == Color).ToList();


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
            CheckPawnPromotion(selectedPiece, chessBoard, chessPieces);
        }

        public void MakeCaptureOnlyMove(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // get all pieces of the same color and randomize their order 
            var playerPieces = chessPieces.Where(p => p.PieceColor == Color).OrderBy(x => _random.Next()).ToList();


            foreach (ChessPiece selectedPiece in playerPieces)
            {
                // Get legal moves for the selected piece
                List<ChessTile> legalMoves = selectedPiece.GetLegalSafeMoves(chessBoard, chessPieces);
                foreach (ChessTile legalMove in legalMoves)
                {
                    ChessPiece tilePiece = ChessUtils.GetPieceAtTile(legalMove, chessPieces);
                    if (tilePiece != null && tilePiece.PieceColor != Color)
                    {
                        selectedPiece.MoveTo(legalMove, chessBoard, chessPieces);
                        CheckPawnPromotion(selectedPiece, chessBoard, chessPieces);
                        return; // capture is made exit the function
                    }
                }
            }

            // if we get to here there is no capture move so we just make a random move
            MakeRandomMove(chessBoard, chessPieces);
}
    }
}

