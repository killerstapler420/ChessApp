using ChessApp.Bots;
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

    public enum BoardState
    {
        PLAYING,
        DRAW,
        WIN, // win for white
        LOSS // loss for white

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

        private string pointToSquare(Point point)
        {
            return "" + (char)(point.Y + 97) + (point.X + 1);
            
        }
        public string RawAlgebraic(bool startSquare) { //bool tells if we need to specify the startsquare
            //this function does not add symbols like + or # as it doesn't know this.
            string algebraic = "";
            if (Castles)
            {
                if (CastlesKingSide)
                {
                    return "0-0";
                }
                return "0-0-0";
            }

            switch (Piece)
            {
                case Piece.PAWN:
                    if (Capture)
                    {
                        //add column of origin (ex: fxe4)
                        algebraic = (char)(From.Y + 97) + "x";
                    }
                    algebraic += pointToSquare(To);
                    if (Promotion != Piece.NONE)
                    {
                        algebraic += "=";
                        switch (Promotion)
                        {
                            case Piece.KNIGTH:
                                algebraic += "N";
                                break;
                            case Piece.BISHOP:
                                algebraic += "B";
                                break;
                            case Piece.ROOK:
                                algebraic += "R";
                                break;
                            case Piece.QUEEN:
                                algebraic += "Q";
                                break;
                        }
                    }
                    return algebraic;
                case Piece.KNIGTH:
                    algebraic = "N";
                    break;
                case Piece.BISHOP:
                    algebraic = "B";
                    break;
                case Piece.ROOK:
                    algebraic = "R";
                    break;
                case Piece.QUEEN:
                    algebraic = "Q";
                    break;
                case Piece.KING:
                    algebraic = "K";
                    break;

            }
            if (startSquare)
            {
                algebraic += pointToSquare(From);
            }
            if (Capture)
            {
                algebraic += "x";
            }
            algebraic += pointToSquare(To);
            return algebraic;
        }

        public override string ToString() => (Piece + " from: " + (char)(From.Y + 97) + (From.X + 1) + " to: " + (char)(To.Y + 97) + (To.X + 1) +(Promotion == Piece.NONE ? "": " Promotion: " + Promotion));

    }
    class Program
    {
        private ChessBoard chessBoard;
        static void Main(string[] args)
        {
            Program P = new Program();
            Game game = new Game();

            game.playGameBetweenBots( new RandomMoveBot(),new MaterialisticBotV1());
        }
    }
}
