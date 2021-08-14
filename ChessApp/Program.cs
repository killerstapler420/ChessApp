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
            Castles = false;
            CastlesKingSide = false;
        }
        public Move(Point from, Point to, Piece piece, Piece promotion, bool capture)
        {
            From = from;
            To = to;
            Promotion = promotion;
            Capture = capture;
            Piece = piece;
            EnPassant = false;
            Castles = false;
            CastlesKingSide = false;
        }
        //castles
        public Move(Point from, Point to,  bool castlesKingSide)
        {
            From = from;
            To = to;
            Promotion = Piece.NONE;
            Capture = false;
            Piece = Piece.KING;
            EnPassant = false;
            Castles = true;
            CastlesKingSide = castlesKingSide;
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
        public bool Castles { get; }
        public bool CastlesKingSide { get; }

        public override string ToString() => (Piece + " from: " + (char)(From.Y + 97) + (From.X + 1) + " to: " + (char)(To.Y + 97) + (To.X + 1) +(Promotion == Piece.NONE ? "": " Promotion: " + Promotion));

    }
    class Program
    {
        private ChessBoard chessBoard;
        static void Main(string[] args)
        {
            Program P = new Program();
            P.chessBoard = new ChessBoard();
            P.chessBoard.setupByFEN("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");
            P.chessBoard.printAscii();


            List<Move> moves = P.chessBoard.GetLegalMoves();

            foreach ( Move i in moves)
            {
                Console.WriteLine(i);
            }
        }
    }
}
