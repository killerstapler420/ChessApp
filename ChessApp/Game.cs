using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApp
{
    class Game
    {
        private ChessBoard chessBoard;
        private string blackPlayer;
        private string whitePlayer;
        public bool gameOver { get; set; }
        public Game()
        {
            chessBoard = new ChessBoard();
            chessBoard.setupByFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            gameOver = false;
        }

        public void makeMove(Move move)
        {
            chessBoard.makeLegalMove(move);
            if (chessBoard.isGameOver())
            {
                gameOver = true;
            }
        }

        public void makeRandomMove()
        {
            var random = new Random();
            List<Move> _moves = chessBoard.GetLegalMoves();
            int index = random.Next(_moves.Count);
            makeMove(_moves[index]);
        }

        public void playRandomGame()
        {
            while (!gameOver)
            {
                makeRandomMove();
                chessBoard.printAscii();
            }
            Console.WriteLine(chessBoard.getBoardstate());
        }

    }
}
