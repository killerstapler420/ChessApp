using System;
using System.Collections.Generic;
using System.Drawing;

namespace ChessApp
{
    public enum Color
    {
        NONE,WHITE,BLACK
    }
    public enum Piece
    {
        NONE,
        KING,
        ROOK,
        KNIGTH,
        QUEEN,
        BISHOP,
        PAWN,
        
    }

    public struct Move
    {
        public Move(Point from, Point to, Piece piece, bool capture, bool enPassant)
        {
            From = from;
            To = to;
            Promotion = Piece.NONE;
            Capture = capture;
            Piece = piece;
            EnPassant = enPassant;
        }
        public Move(Point from, Point to,Piece piece, Piece promotion, bool capture, bool enPassant)
        {
            From = from;
            To = to;
            Promotion = promotion;
            Capture = capture;
            Piece = piece;
            EnPassant = enPassant;
        }

        //TODO: implement notation
            //-> determine if check
            //-> determine if checkmate/stalemate/...
            //check for ambiguous moves

        public Point From { get; }
        public Point To { get; }
        public Piece Promotion { get; }
        public Piece Piece { get; }
        public bool Capture { get; }
        public bool EnPassant { get; }

        public override string ToString() => (Piece + " from: " + (char)(From.Y + 97) + (From.X +1) + " to: "+ (char)(To.Y + 97) + (To.X+1));

    }
    class Program
    {
        private ChessBoard chessBoard;
        static void Main(string[] args)
        {
            Program P = new Program();
            P.chessBoard = new ChessBoard();
            P.chessBoard.setupByFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            //P.chessBoard.setup();
            P.chessBoard.printAscii();
            List<Move> moves = P.chessBoard.GetLegalMoves();
            
            Console.WriteLine("");

            
            foreach (Move i in moves)
            {
                Console.WriteLine(i);
            }

            Console.WriteLine("");

            Console.WriteLine(P.chessBoard.getBoardstate());
            Move move = new Move(new Point(1, 5), new Point(2, 5), Piece.PAWN, false, false);
            P.chessBoard.makeLegalMove(move);
            P.chessBoard.printAscii();

            Console.WriteLine(P.chessBoard.getBoardstate());

            Console.WriteLine("");
            move = new Move(new Point(6, 4), new Point(5, 4), Piece.PAWN, false, false);
            P.chessBoard.makeLegalMove(move);
            P.chessBoard.printAscii();

            move = new Move(new Point(1, 6), new Point(3, 6), Piece.PAWN, false, false);
            P.chessBoard.makeLegalMove(move);
            P.chessBoard.printAscii();

            Console.WriteLine("");
            move = new Move(new Point(7, 3), new Point(3, 7), Piece.QUEEN, false, false);
            P.chessBoard.makeLegalMove(move);
            P.chessBoard.printAscii();

            Console.WriteLine(P.chessBoard.getBoardstate());
        }
    }
}
