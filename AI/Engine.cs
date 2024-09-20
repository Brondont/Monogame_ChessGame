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
        private int _depth;


        public ChessEngine(Player color, GraphicsDevice graphicsDevice, ContentManager content, int depth)
        {
            Color = color;
            _random = new Random();
            _graphicsDevice = graphicsDevice;
            _content = content;
            _depth = depth;
        }

        private int EvaluateBoard(List<ChessPiece> chessPieces)
        {
            int score = 0;
            foreach (var piece in chessPieces)
            {
                score += piece.PieceColor == Color ? piece.Value : -piece.Value;
            }
            return score;
        }

        private void CheckPawnPromotion(ChessPiece selectedPiece, List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            int selectedPieceIndex = chessBoard.IndexOf(selectedPiece.HomeTile);
            if (selectedPiece.Type == "pawn" && (selectedPieceIndex > 56 || selectedPieceIndex < 8))
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
            List<ChessTile> legalMoves = new List<ChessTile>();

            do
            {
                // Select a random piece
                selectedPiece = playerPieces[_random.Next(playerPieces.Count)];
                // Get legal moves for the selected piece
                legalMoves = selectedPiece.GetLegalSafeMoves(chessBoard, chessPieces);
            } while(legalMoves.Count == 0);

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

        public void MakeMinMaxMove(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // Store the best move and piece
            ChessPiece bestPiece = null;
            ChessTile bestMove = null;

            // Call the minimax function
            int _ = MinMax(new List<ChessTile>(chessBoard), new List<ChessPiece>(chessPieces), _depth, true, out bestPiece, out bestMove);

            // Execute the best move
            if (bestPiece != null && bestMove != null)
            {
                bestPiece.MoveTo(bestMove, chessBoard, chessPieces);
                CheckPawnPromotion(bestPiece, chessBoard, chessPieces);
            }
        }
        private int MinMax(List<ChessTile> chessBoard, List<ChessPiece> chessPieces, int depth, bool isMaxing, out ChessPiece bestPiece, out ChessTile bestMove)
        {
            bestPiece = null;
            bestMove = null;
            int evaluation = 0;
            Player currentTurn = isMaxing == true ? this.Color : (this.Color == Player.White ? Player.Black : Player.White);

            if (depth == 0 || ChessUtils.IsGameOver(chessBoard, chessPieces, currentTurn, out _))
            {
                return EvaluateBoard(chessPieces); // Implement this to evaluate the board's state
            }

            // simulate each possible move 
            foreach (ChessPiece piece in chessPieces)
            {
                List<ChessTile> legalMoves = piece.GetLegalMoves(chessBoard, chessPieces);
                foreach (ChessTile legalMove in legalMoves)
                {
                    piece.MoveTo(legalMove, chessBoard, chessPieces);
                    evaluation = MinMax(new List<ChessTile>(chessBoard), new List<ChessPiece>(chessPieces), depth - 1, false, out _, out _);
                }
            }
            return 0;
        }
    }
}

