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
            chessBoard.printAscii();
            Console.WriteLine(chessBoard.getBoardstate());
            Console.WriteLine("");
            Console.WriteLine(chessBoard.getPGN());

        }

        public void playGameBetweenBots(IBot white, IBot black)
        {
            chessBoard = new ChessBoard();
            chessBoard.setupByFEN("3b4/1R3p2/1r2k1pp/4Np2/3P4/7P/5PP1/6K1 w - - 0 35");
            gameOver = false;
            chessBoard.printAscii();
            while (!gameOver)
            {
                makeMove(white.getBestMove(chessBoard));
                chessBoard.printAscii();
                if (gameOver)
                {
                    break;
                }
                makeMove(black.getBestMove(chessBoard));
                chessBoard.printAscii();
            }
            Console.WriteLine(chessBoard.getBoardstate());
            Console.WriteLine("");
            Console.WriteLine(chessBoard.getPGN());
        }

        public void playVsComputer()
        {
            // play a crappy game vs crappy computer
            while (!gameOver)
            {
                Console.WriteLine("from x");
                int fromx = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("from y");
                int fromy = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("to x");
                int tox = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("to y");
                int toy = Convert.ToInt32(Console.ReadLine());

                foreach (Move move in chessBoard.GetLegalMoves())
                {
                    if(move.From.X == fromx && move.From.Y == fromy && move.To.X == tox && move.To.Y == toy)
                    {
                        chessBoard.makeLegalMove(move);
                    }
                }
                makeRandomMove();
                chessBoard.printAscii();
            }
            Console.WriteLine(chessBoard.getBoardstate());
        }
    }
}
