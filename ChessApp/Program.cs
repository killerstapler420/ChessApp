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
            P.chessBoard.setupByFEN("rnbqkbnr/ppppp1pp/8/8/8/8/PPPPPpPP/RNBQKBNR w KQkq - 0 1");
            //P.chessBoard.setup();
            P.chessBoard.printAscii();
            List<Move> moves = P.chessBoard.GetLegalMovesOfColor(Color.WHITE);
            

            foreach (Move i in moves)
            {
                Console.WriteLine(i);
            }
        }
    }
}
