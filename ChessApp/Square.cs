using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApp
{
    class Square
    {
        public int column { get; set; }
        public int row { get; set; }

        private bool occupied { get; set; }
        public bool isLegal { get; set; }
        private Color occupationColor { get; set; }
        private Piece occupationPiece { get; set; }



        public Square(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
        public string getAsciiSymbol()
        {
            string a = "";
            switch (occupationPiece)
            {
                case Piece.NONE:
                    return null;
                case Piece.KING:
                    a = "k";
                    break;
                case Piece.QUEEN:
                    a = "q";
                    break;
                case Piece.BISHOP:
                    a = "b";
                    break;
                case Piece.KNIGTH:
                    a = "n"; //n and not k because of ambiguity with king
                    break;
                case Piece.ROOK:
                    a = "r";
                    break;
                case Piece.PAWN:
                    a = "p";
                    break;

            }
            if (occupationColor == Color.WHITE)
            {
                a = a.ToUpper();
            }

            return a;

        }

        public void print()
        {
            string a="";
            switch (occupationPiece)
                {
                case Piece.NONE:
                    a = ". ";
                    break;
                case Piece.KING :
                    a = "k ";
                    break;
                    case Piece.QUEEN:
                    a = "q ";
                    break;
                    case Piece.BISHOP:
                    a = "b ";
                    break;
                    case Piece.KNIGTH:
                    a = "n "; //n and not k because of ambiguity with king
                    break;
                    case Piece.ROOK:
                    a = "r ";
                    break;
                    case Piece.PAWN:
                    a = "p ";
                    break;

                }
            if(occupationColor == Color.WHITE)
            {
                a = a.ToUpper();
            }

            Console.Write(a);
            
        }


        public void setPiece(Piece piece, Color color)
        {
            this.occupationPiece = piece;
            this.occupationColor = color;
        }

        public Piece GetPiece()
        {
            return occupationPiece;
        }

        public Color GetColor()
        {
            return occupationColor;
        }
    }
}
