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
        private const int MAX_QUIESCENCE_DEPTH = 1;




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
            for (int i = 0; i < chessPieces.Count; i++)
            {
                score += chessPieces[i].PieceColor == Color ? chessPieces[i].Value : -chessPieces[i].Value;
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

        private bool BoardIsQuiet(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            for (int i = 0; i < chessPieces.Count; i++)
            {
                ChessPiece piece = chessPieces[i];
                if (piece.GetCaptureCheckMoves(chessBoard, chessPieces).Count != 0)
                    return false;
            }
            return true;
        }


        public void MakeRandomMove(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            var playerPieces = chessPieces.Where(p => p.PieceColor == Color).ToList();

            ChessPiece selectedPiece = null; List<ChessTile> legalMoves = new List<ChessTile>();
            do
            { // Select a random piece
                selectedPiece = playerPieces[_random.Next(playerPieces.Count)];
                // Get legal moves for the selected piece
                legalMoves = selectedPiece.GetLegalSafeMoves(chessBoard, chessPieces);
            } while (legalMoves.Count == 0);

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

        public void Barbie(List<ChessTile> chessBoard, List<ChessPiece> chessPieces)
        {
            // Store the best move and piece
            ChessPiece bestPiece = null;
            ChessTile bestMove = null;


            // Call the minimax function
            int _ = MinMax(chessBoard, chessPieces, _depth, true, out bestPiece, out bestMove);
            // Execute the best move
            if (bestPiece != null && bestMove != null)
            {
                Console.WriteLine($"Best move is: {bestPiece.PieceColor} {bestPiece.Type} to {bestMove.TileCoordinate}");
                bestPiece.MoveTo(bestMove, chessBoard, chessPieces);
                CheckPawnPromotion(bestPiece, chessBoard, chessPieces);
            }
        }

        public int MinMax(List<ChessTile> chessBoard, List<ChessPiece> chessPieces, int depth, bool isMaxing, out ChessPiece bestPiece, out ChessTile bestMove)
        {
            bestPiece = null;
            bestMove = null;

            int evaluation = isMaxing ? int.MinValue : int.MaxValue;
            Player currentTurn = isMaxing ? this.Color : (this.Color == Player.White ? Player.Black : Player.White);
            // add random moves for now as it plays in a dumb way where it moves the pieces in order when evaluation is the same 
            var playerPieces = chessPieces.Where(p => p.PieceColor == Color).OrderBy(x => _random.Next()).ToList();

            // Base case: if we've reached the maximum depth or the game is over
            if (depth == 0 || ChessUtils.IsGameOver(chessBoard, chessPieces, currentTurn, out _))
            {
                return QuiescenceSearch(chessBoard, chessPieces, isMaxing, MAX_QUIESCENCE_DEPTH);

                // return EvaluateBoard(chessPieces);
            }

            // Iterate over each piece and simulate its moves
            for (int i = 0; i < playerPieces.Count; i++)
            {
                ChessPiece piece = playerPieces[i];
                List<ChessTile> legalMoves = piece.GetLegalSafeMoves(chessBoard, chessPieces);
                Console.WriteLine($"Trying move: {piece.PieceColor} {piece.Type} to");
                foreach (ChessTile legalMove in legalMoves)
                {
                    // Store the original state
                    ChessTile originalTile = piece.HomeTile;
                    ChessTile originalLastTile = piece.LastTile;
                    int originalLastTurn = piece.LastMovedTurn;
                    Console.WriteLine($"{legalMove.TileCoordinate}");
                    ChessPiece capturedPiece = piece.MoveTo(legalMove, chessBoard, chessPieces);
                    // Recursively call MinMax with the simulated board state
                    int newEvaluation = MinMax(chessBoard, chessPieces, depth - 1, !isMaxing, out _, out _);

                    // Update evaluation based on maximizing/minimizing
                    if (isMaxing && newEvaluation > evaluation || !isMaxing && newEvaluation < evaluation)
                    {
                        evaluation = newEvaluation;
                        bestPiece = piece;
                        bestMove = legalMove;
                    }

                    // Revert the move (restore the original state)
                    piece.UndoMove(originalTile, originalLastTile, originalLastTurn);

                    // If a piece was captured, restore it
                    if (capturedPiece != null)
                    {
                        chessPieces.Add(capturedPiece);
                        capturedPiece.LoadContent(_graphicsDevice, _content);
                    }
                }
            }
            return evaluation;
        }


        private int QuiescenceSearch(List<ChessTile> chessBoard, List<ChessPiece> chessPieces, bool isMaxing, int quiescenceDepth)
        {
            int evaluation = EvaluateBoard(chessPieces);


            Player currentTurn = isMaxing ? this.Color : (this.Color == Player.White ? Player.Black : Player.White);
            var playerPieces = chessPieces.Where(p => p.PieceColor == currentTurn).ToList();

            if (BoardIsQuiet(chessBoard, chessPieces) || ChessUtils.IsGameOver(chessBoard, chessPieces, currentTurn, out _) || quiescenceDepth == 0)
            {
                Console.WriteLine("Quiescence search ended");
                return evaluation;
            }

            for (int i = 0; i < playerPieces.Count; i++)
            {
                ChessPiece piece = playerPieces[i];

                List<ChessTile> legalMoves = piece.GetCaptureCheckMoves(chessBoard, chessPieces);

                foreach (ChessTile legalMove in legalMoves)
                {
                    // Store the original state
                    ChessTile originalTile = piece.HomeTile;
                    ChessTile originalLastTile = piece.LastTile;
                    int originalLastTurn = piece.LastMovedTurn;

                    ChessPiece capturedPiece = piece.MoveTo(legalMove, chessBoard, chessPieces);
                    // Recursively call with the simulated board state
                    int newEvaluation = QuiescenceSearch(chessBoard, chessPieces, !isMaxing, quiescenceDepth - 1);

                    // Update evaluation based on maximizing/minimizing
                    if (isMaxing && newEvaluation > evaluation || !isMaxing && newEvaluation < evaluation)
                    {
                        evaluation = newEvaluation;
                    }
                    // Revert the move
                    piece.UndoMove(originalTile, originalLastTile, originalLastTurn);
                    // If a piece was captured, restore it
                    if (capturedPiece != null)
                    {
                        chessPieces.Add(capturedPiece);
                        capturedPiece.LoadContent(_graphicsDevice, _content);
                    }

                }
            }
            return evaluation;
        }
    }

}


