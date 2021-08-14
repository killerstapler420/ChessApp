using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ChessApp
{

    class ChessBoard
    {
        private Square[,] board;
        private Color toMove;
        private bool wCanCastleKing;
        private bool wCanCastleQueen;
        private bool bCanCastleKing;
        private bool bCanCastleQueen;
        private int moveNumber;
        private int halfMoves; //important for 50 move rule
        private int ?enPassantColumn; //resets after every move.
                                      //for white the en passant row will always be on the 6the row (for capturing) for black the 3th.
                                      //TODO

        //gamestate variables
        List<Move> ?_legalMoves;
        private bool inCheck;
        private bool inCheckMate;
        private bool inStaleMate;
        private bool gameOver;


        public ChessBoard()
        {
            board = new Square[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Square(i, j);
                }
            }
        }

        public void printAscii()
        {
            Console.WriteLine("");
            for (int i = 7; i > -1; i--)
            {
                
                for (int j = 0; j < 8; j++)
                {
                    board[i, j].print();
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
            Console.WriteLine("w 0-0: " + wCanCastleKing);
            Console.WriteLine("w 0-0-0: " + wCanCastleQueen);
            Console.WriteLine("w 0-0: " + bCanCastleKing);
            Console.WriteLine("w 0-0-0: " + bCanCastleQueen);
            Console.WriteLine("En passant: " + enPassantColumn);
            Console.WriteLine("Halfmoves: " + halfMoves);
            Console.WriteLine("Current Movenumber: " + moveNumber);
            Console.WriteLine("");
            Console.WriteLine("To move: " + toMove);
        }

        public ChessBoard Clone()
        {
            ChessBoard chessboard = new ChessBoard();
            chessboard.setupByFEN(exportFEN()); //This will do until I find a better way to clone an object
            return chessboard;
        }
        public string exportFEN()
        {
            string fen = "";
            for (int i = 7; i > -1; i--)
            {
                int empties = 0; //defines the amount of emptie squares after each other, resets after a piece
                for (int j = 0; j < 8; j++)
                {

                    if(board[i, j].getAsciiSymbol() == null)
                    {
                        empties++;
                    }
                    else
                    {
                        if(empties != 0)
                        {
                            fen += empties;
                            empties = 0;
                        }
                    }
                    fen += board[i, j].getAsciiSymbol();

                }
                if (empties != 0)
                {
                    fen += empties;
                }
                fen += "/";
            }
            fen = fen.Remove(fen.Length - 1, 1); //removes last "/" that was added in for loop
            fen += " ";

            //moveColor

            if(toMove == Color.WHITE)
            {
                fen += "w ";
            }
            else
            {
                fen += "b ";
            }

            //CastleRigths
            if(!wCanCastleKing && !wCanCastleQueen && !bCanCastleKing && !bCanCastleQueen)
            {
                fen += "- "; //no castling rights
            }
            else
            {
                if (wCanCastleKing)
                {
                    fen += "K";
                }
                if (wCanCastleQueen)
                {
                    fen += "Q";
                }
                if (bCanCastleKing)
                {
                    fen += "k";
                }
                if (bCanCastleQueen)
                {
                    fen += "q";
                }
                fen += " ";
            }

            //en passant column
            if(enPassantColumn == null)
            {
                fen += "- ";
            }
            else
            {
                fen += (char)(enPassantColumn + 97); //97 is char of a
                if(toMove == Color.WHITE)
                {
                    fen += "6 ";
                }
                else
                {
                    fen += "3 ";
                }
            }

            //halfmoves and moves
            fen += halfMoves;
            fen += " ";
            fen += moveNumber;

            return fen;
        }

        public void setupByFEN(string FEN)
        {
            //TODO: reset board
            string[] _fen = FEN.Split(' ');
            string[] _pieces = _fen[0].Split('/');

            // generate the board
            for (int i = 0; i < 8; i++)
            {

                //Console.WriteLine("");
                int column = 0;
                foreach (char c in _pieces[i])
                {
                    if (char.IsDigit(c))
                    {
                        column += Convert.ToInt32(char.GetNumericValue(c));
                    }
                    else
                    {
                        switch (c)
                        {
                            case 'r':
                                board[7 - i, column].setPiece(Piece.ROOK, Color.BLACK);
                                break;
                            case 'b':
                                board[7 - i, column].setPiece(Piece.BISHOP, Color.BLACK);
                                break;
                            case 'n':
                                board[7 - i, column].setPiece(Piece.KNIGTH, Color.BLACK);
                                break;
                            case 'q':
                                board[7 - i, column].setPiece(Piece.QUEEN, Color.BLACK);
                                break;
                            case 'k':
                                board[7 - i, column].setPiece(Piece.KING, Color.BLACK);
                                break;
                            case 'p':
                                board[7 - i, column].setPiece(Piece.PAWN, Color.BLACK);
                                break;
                            case 'R':
                                board[7 - i, column].setPiece(Piece.ROOK, Color.WHITE);
                                break;
                            case 'B':
                                board[7 - i, column].setPiece(Piece.BISHOP, Color.WHITE);
                                break;
                            case 'N':
                                board[7 - i, column].setPiece(Piece.KNIGTH, Color.WHITE);
                                break;
                            case 'Q':
                                board[7 - i, column].setPiece(Piece.QUEEN, Color.WHITE);
                                break;
                            case 'K':
                                board[7 - i, column].setPiece(Piece.KING, Color.WHITE);
                                break;
                            case 'P':
                                board[7 - i, column].setPiece(Piece.PAWN, Color.WHITE);
                                break;

                            default:
                                break;
                        }
                        column++;
                    }

                }
            }

            //decide who is to move
            if (_fen[1] == "w")
            {
                this.toMove = Color.WHITE;
            }
            else
            {
                this.toMove = Color.BLACK;
            }

            //decide castling rigths

            foreach (char c in _fen[2])
            {
                if(c == 'K')
                {
                wCanCastleKing = true;
                }
                if (c == 'Q')
                {
                    wCanCastleQueen = true;
                }
                if (c == 'k')
                {
                    bCanCastleKing = true;
                }
                if (c == 'q')
                {
                    bCanCastleQueen = true;
                }
            }

            //en passant square
            foreach (char c in _fen[3])
            {
                enPassantColumn = c.CompareTo('a');
                if (enPassantColumn >7 | enPassantColumn < 0)
                {
                    enPassantColumn = null;
                }
                break; // this contains 2 chars but we only need the first one. probably not best use but I couldn't be bothered
                
            }

            //decide halfmoves
            halfMoves = Int32.Parse(_fen[4]);
            //decide moveNumber
            moveNumber = Int32.Parse(_fen[5]);

        }
        public void setup()
        {
            //nullable parameters are weirdly not allowed
            if (null != null)
            {

            }
            else {
                //standard board setup
                board[0, 0].setPiece(Piece.ROOK, Color.WHITE);
                board[0, 7].setPiece(Piece.ROOK, Color.WHITE);
                board[0, 1].setPiece(Piece.KNIGTH, Color.WHITE);
                board[0, 6].setPiece(Piece.KNIGTH, Color.WHITE);
                board[0, 2].setPiece(Piece.BISHOP, Color.WHITE);
                board[0, 5].setPiece(Piece.BISHOP, Color.WHITE);
                board[0, 3].setPiece(Piece.QUEEN, Color.WHITE);
                board[0, 4].setPiece(Piece.KING, Color.WHITE);

                for (int i = 0; i < 8; i++)
                {
                    board[1, i].setPiece(Piece.PAWN, Color.WHITE);
                }
                for (int i = 0; i < 8; i++)
                {
                    board[6, i].setPiece(Piece.PAWN, Color.BLACK);
                }


                board[7, 0].setPiece(Piece.ROOK, Color.BLACK);
                board[7, 7].setPiece(Piece.ROOK, Color.BLACK);
                board[7, 1].setPiece(Piece.KNIGTH, Color.BLACK);
                board[7, 6].setPiece(Piece.KNIGTH, Color.BLACK);
                board[7, 2].setPiece(Piece.BISHOP, Color.BLACK);
                board[7, 5].setPiece(Piece.BISHOP, Color.BLACK);
                board[7, 3].setPiece(Piece.QUEEN, Color.BLACK);
                board[7, 4].setPiece(Piece.KING, Color.BLACK);
            }
            

        }

        public void printRawLegalMovesOfSquare(int row, int column)
        {

            //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/

            switch (board[row,column].GetPiece())
            {
                case Piece.NONE:
                    Console.WriteLine("None");
                    break;
                case Piece.KING:
                    Console.WriteLine("King");
                    List<Point> moves = new List<Point>();
                    for (int i = row-1; i < row + 2; i++)
                    {
                        for (int j = column - 1; j < column +2; j++)
                        {
                            if((i>-1 && j>-1) && (i,j)!=(row,column))
                            {
                                moves.Add(new Point(i, j));
                                board[i,j].isLegal = true;
                            }
                        }
                    }
                    Console.Write(moves);

                    break;
                case Piece.QUEEN:
                    Console.WriteLine("Queen");
                    //left-down
                    for (int i = 1; row - i > -1 && column - i > -1; i++)
                    {

                        board[row - i, column - i].isLegal = true;

                    }
                    //left-up
                    for (int i = 1; row + i < 8 && column - i > -1; i++)
                    {

                        board[row + i, column - i].isLegal = true;

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {

                        board[row - i, column + i].isLegal = true;

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {

                        board[row + i, column + i].isLegal = true;

                    }
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {

                        board[i, column].isLegal = true;

                    }
                    for (int i = row + 1; i < 8; i++)
                    {

                        board[i, column].isLegal = true;

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {

                        board[row, j].isLegal = true;

                    }
                    for (int j = column + 1; j < 8; j++)
                    {

                        board[row, j].isLegal = true;

                    }
                    break;
                case Piece.BISHOP:
                    Console.WriteLine("Bishop");
                    //left-down
                    for (int i = 1; row-i > -1 && column-i>-1; i++)
                    {

                        board[row-i, column - i].isLegal = true;

                    }
                    //left-up
                    for (int i = 1; row + i <8 && column - i > -1; i++)
                    {

                        board[row + i, column - i].isLegal = true;

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {

                        board[row - i, column + i].isLegal = true;

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {

                        board[row + i, column + i].isLegal = true;

                    }

                    break;
                case Piece.KNIGTH:
                    Console.WriteLine("Knigth");
                    int[,] jumps =
                    {
                        { row + 1, column + 2 },
                        { row + 1, column - 2},
                        {row - 1, column + 2 },
                        { row - 1, column - 2},
                        {row + 2, column + 1 },
                        {row + 2, column - 1 },
                        { row - 2, column + 1},
                        {row - 2, column - 1 }
                    };

                    for (int i = 0; i < jumps.GetLength(0); i++)
                    {
                        int x = jumps[i, 0];
                        int y = jumps[i, 1];

                        if(x> -1 && x < 8 && y > -1 && y < 8)
                        {
                            board[x, y].isLegal = true;
                        }
                    }
                    break;
                case Piece.ROOK:
                    Console.WriteLine("Rook");
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {
                        
                            board[i, column].isLegal = true;
                        
                    }
                    for (int i = row + 1; i <8; i++)
                    {

                        board[i, column].isLegal = true;

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {

                        board[row, j].isLegal = true;

                    }
                    for (int j = column + 1; j < 8; j++)
                    {

                        board[row, j].isLegal = true;

                    }
                    break;
                case Piece.PAWN:
                    Console.WriteLine("Pawn");
                    break;

            }

            for (int i = 7; i > -1; i--)
            {
                Console.WriteLine("");
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].isLegal)
                    {
                        Console.Write("+ ");
                    }
                    else { Console.Write(". "); }

                }
            }
        }

        public List<Point> getRawLegalMovesOfSquare(int row, int column)
        {
            List<Point> moves = new List<Point>();

            //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/

            switch (board[row, column].GetPiece())
            {
                case Piece.NONE:
                    Console.WriteLine("None");
                    break;
                case Piece.KING:
                    Console.WriteLine("King");
                    
                    for (int i = row - 1; i < row + 2; i++)
                    {
                        for (int j = column - 1; j < column + 2; j++)
                        {
                            if ((i > -1 && j > -1) && (i, j) != (row, column))
                            {
                                moves.Add(new Point(i, j));
                                board[i, j].isLegal = true;
                            }
                        }
                    }

                    break;
                case Piece.QUEEN:
                    Console.WriteLine("Queen");
                    //left-down
                    for (int i = 1; row - i > -1 && column - i > -1; i++)
                    {
                        moves.Add(new Point(row - i, column - i));
                        board[row - i, column - i].isLegal = true;

                    }
                    //left-up
                    for (int i = 1; row + i < 8 && column - i > -1; i++)
                    {
                        moves.Add(new Point(row + i, column - i));
                        board[row + i, column - i].isLegal = true;

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {
                        moves.Add(new Point(row - i, column + i));
                        board[row - i, column + i].isLegal = true;

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {
                        moves.Add(new Point(row + i, column + i));
                        board[row + i, column + i].isLegal = true;

                    }
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {
                        moves.Add(new Point(i, column));
                        board[i, column].isLegal = true;

                    }
                    for (int i = row + 1; i < 8; i++)
                    {
                        moves.Add(new Point(i, column));
                        board[i, column].isLegal = true;

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {
                        moves.Add(new Point(row, j));
                        board[row, j].isLegal = true;

                    }
                    for (int j = column + 1; j < 8; j++)
                    {
                        moves.Add(new Point(row, j));
                        board[row, j].isLegal = true;

                    }
                    break;
                case Piece.BISHOP:
                    Console.WriteLine("Bishop");
                    //left-down
                    for (int i = 1; row - i > -1 && column - i > -1; i++)
                    {
                        moves.Add(new Point(row - i, column - i));
                        board[row - i, column - i].isLegal = true;

                    }
                    //left-up
                    for (int i = 1; row + i < 8 && column - i > -1; i++)
                    {
                        moves.Add(new Point(row + i, column - i));
                        board[row + i, column - i].isLegal = true;

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {
                        moves.Add(new Point(row - i, column + i));
                        board[row - i, column + i].isLegal = true;

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {
                        moves.Add(new Point(row + i, column + i));
                        board[row + i, column + i].isLegal = true;

                    }

                    break;
                case Piece.KNIGTH:
                    Console.WriteLine("Knigth");
                    int[,] jumps =
                    {
                        { row + 1, column + 2 },
                        { row + 1, column - 2},
                        {row - 1, column + 2 },
                        { row - 1, column - 2},
                        {row + 2, column + 1 },
                        {row + 2, column - 1 },
                        { row - 2, column + 1},
                        {row - 2, column - 1 }
                    };

                    for (int i = 0; i < jumps.GetLength(0); i++)
                    {
                        int x = jumps[i, 0];
                        int y = jumps[i, 1];

                        if (x > -1 && x < 8 && y > -1 && y < 8)
                        {
                            moves.Add(new Point(x, y));
                            board[x, y].isLegal = true;
                        }
                    }
                    break;
                case Piece.ROOK:
                    Console.WriteLine("Rook");
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {
                        moves.Add(new Point(i, column));
                        board[i, column].isLegal = true;

                    }
                    for (int i = row + 1; i < 8; i++)
                    {
                        moves.Add(new Point(i, column));
                        board[i, column].isLegal = true;

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {
                        moves.Add(new Point(row, j));
                        board[row, j].isLegal = true;

                    }
                    for (int j = column + 1; j < 8; j++)
                    {
                        moves.Add(new Point(row, j));
                        board[row, j].isLegal = true;

                    }
                    break;
                case Piece.PAWN:
                    Console.WriteLine("Pawn");
                    break;

            }

            return moves;
        }

        public List<Point> getPseudoLegalMovesOfSquare(int row, int column)
        {
            //This function will facture in the other pieces i.e captures/blocked attacks
            List<Point> moves = new List<Point>();
            Color color = board[row, column].GetColor();
            //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/

            switch (board[row, column].GetPiece())
            {
                case Piece.NONE:
                    //Console.WriteLine("None");
                    break;
                case Piece.KING:
                    //Console.WriteLine("King");

                    for (int i = row - 1; i < row + 2; i++)
                    {
                        for (int j = column - 1; j < column + 2; j++)
                        {
                            if ((i > -1 && j > -1 &&i<8 && j<8) && (i, j) != (row, column))
                            {
                                if(board[i,j].GetColor() != color)
                                {
                                    //color has to be opposing or none
                                    moves.Add(new Point(i, j));
                                }
                                
                            }
                        }
                    }

                    break;
                case Piece.QUEEN:
                    //Console.WriteLine("Queen");
                    //left-down...I think
                    for (int i = 1; row - i > -1 && column - i > -1; i++)
                    {
                        if (board[row - i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row - i, column - i].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(row - i, column - i));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row - i, column - i));

                    }
                    //left-up
                    for (int i = 1; row + i < 8 && column - i > -1; i++)
                    {
                        if (board[row + i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row + i, column - i].GetColor() == color)
                            {
                                // Console.WriteLine("Blocked");
                            }
                            else
                            {

                                moves.Add(new Point(row + i, column - i));
                                // Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row + i, column - i));

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {
                        if (board[row - i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row - i, column + i].GetColor() == color)
                            {
                                // Console.WriteLine("Blocked");
                            }
                            else
                            {

                                moves.Add(new Point(row - i, column + i));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row - i, column + i));

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {
                        if (board[row + i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row + i, column + i].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {

                                moves.Add(new Point(row + i, column + i));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row + i, column + i));

                    }
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[i, column].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(i, column));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }
                    for (int i = row + 1; i < 8; i++)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[i, column].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(i, column));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row, j].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(row, j));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }

                        moves.Add(new Point(row, j));

                    }
                    for (int j = column + 1; j < 8; j++)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row, j].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(row, j));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row, j));

                    }
                    break;
                case Piece.BISHOP:
                    //Console.WriteLine("Bishop");
                    //left-down...I think
                    for (int i = 1; row - i > -1 && column - i > -1; i++)
                    {
                        if (board[row - i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row - i, column - i].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(row - i, column - i));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row - i, column - i));

                    }
                    //left-up
                    for (int i = 1; row + i < 8 && column - i > -1; i++)
                    {
                        if (board[row + i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row + i, column - i].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {

                                moves.Add(new Point(row + i, column - i));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row + i, column - i));

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {
                        if (board[row - i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row - i, column + i].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {

                                moves.Add(new Point(row - i, column + i));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row - i, column + i));

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {
                        if (board[row + i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row + i, column + i].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {

                                moves.Add(new Point(row + i, column + i));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row + i, column + i));

                    }

                    break;
                case Piece.KNIGTH:
                    //Console.WriteLine("Knigth");
                    int[,] jumps =
                    {
                        { row + 1, column + 2 },
                        { row + 1, column - 2},
                        {row - 1, column + 2 },
                        { row - 1, column - 2},
                        {row + 2, column + 1 },
                        {row + 2, column - 1 },
                        { row - 2, column + 1},
                        {row - 2, column - 1 }
                    };

                    for (int i = 0; i < jumps.GetLength(0); i++)
                    {
                        int x = jumps[i, 0];
                        int y = jumps[i, 1];

                        if (x > -1 && x < 8 && y > -1 && y < 8)
                        {
                            if (board[x, y].GetColor() != color)
                            {
                                //color has to be opposing or none
                                moves.Add(new Point(x, y));
                            }
                            
                        }
                    }
                    break;
                case Piece.ROOK:
                    //Console.WriteLine("Rook");
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[i, column].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(i, column));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }
                    for (int i = row + 1; i < 8; i++)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[i, column].GetColor() == color)
                            {
                                // Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(i, column));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row, j].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(row, j));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }

                        moves.Add(new Point(row, j));

                    }
                    for (int j = column + 1; j < 8; j++)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            if (board[row, j].GetColor() == color)
                            {
                                //Console.WriteLine("Blocked");
                            }
                            else
                            {
                                moves.Add(new Point(row, j));
                                //Console.WriteLine("Capture");
                            }

                            break;
                        }
                        moves.Add(new Point(row, j));

                    }
                    break;
                case Piece.PAWN:
                    //Console.WriteLine("Pawn");

                    //TODO: en passant
                    //TODO: promotion

                    if (color == Color.WHITE)
                    {
                        //TODO: place checks to stay in bounds
                        if (row + 1 < 8)
                        {
                            if (board[row + 1, column].GetColor() == Color.NONE)
                            {
                                moves.Add(new Point(row + 1, column));
                                if (row == 1)
                                {
                                    if (board[row + 2, column].GetColor() == Color.NONE)
                                    {
                                        moves.Add(new Point(row + 2, column));
                                    }
                                }
                            }
                            if (column + 1 < 8)
                            {
                                if (board[row + 1, column + 1].GetColor() == Color.BLACK) //We already know our color is white
                                {
                                    moves.Add(new Point(row + 1, column + 1));
                                    //Console.WriteLine("Capture");
                                }
                            }
                            if (column - 1 > -1)
                            {
                                if (board[row + 1, column - 1].GetColor() == Color.BLACK) //We already know our color is white
                                {
                                    moves.Add(new Point(row + 1, column - 1));
                                    //Console.WriteLine("Capture");
                                }
                            }
                        }
                    }
                    if (color == Color.BLACK)
                    {
                        if (row - 1 > -1)
                        {
                            if (board[row - 1, column].GetColor() == Color.NONE)
                            {
                                moves.Add(new Point(row - 1, column));
                                if (row == 6)
                                {
                                    if (board[row - 2, column].GetColor() == Color.NONE)
                                    {
                                        moves.Add(new Point(row - 2, column));
                                    }
                                }
                            }
                            if (column + 1 < 8)
                            {
                                if (board[row - 1, column + 1].GetColor() == Color.WHITE) //We already know our color is black
                                {
                                    moves.Add(new Point(row - 1, column + 1));
                                    //Console.WriteLine("Capture");
                                }
                            }
                            if (column - 1 > -1)
                            {
                                if (board[row - 1, column - 1].GetColor() == Color.WHITE) //We already know our color is black
                                {
                                    moves.Add(new Point(row - 1, column - 1));
                                    //Console.WriteLine("Capture");
                                }
                            }
                        }
                    }
                    break;

            }

            return moves;
        }

        public List<Point> getattackOfSquare(int row, int column)
        {
            //This function will facture in the other pieces i.e captures/blocked attacks
            List<Point> moves = new List<Point>();
            Color color = board[row, column].GetColor();
            //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/

            switch (board[row, column].GetPiece())
            {
                case Piece.NONE:
                    //Console.WriteLine("None");
                    break;
                case Piece.KING:
                    //Console.WriteLine("King");

                    for (int i = row - 1; i < row + 2; i++)
                    {
                        for (int j = column - 1; j < column + 2; j++)
                        {
                            if ((i > -1 && j > -1) && (i, j) != (row, column))
                            {
                                    moves.Add(new Point(i, j));
                            }
                        }
                    }

                    break;
                case Piece.QUEEN:
                    //Console.WriteLine("Queen");
                    //left-down...I think
                    for (int i = 1; row - i > -1 && column - i > -1; i++)
                    {
                        if (board[row - i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row - i, column - i));

                            break;
                        }
                        moves.Add(new Point(row - i, column - i));

                    }
                    //left-up
                    for (int i = 1; row + i < 8 && column - i > -1; i++)
                    {
                        if (board[row + i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row + i, column - i));

                            break;
                        }
                        moves.Add(new Point(row + i, column - i));

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {
                        if (board[row - i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row - i, column + i));

                            break;
                        }
                        moves.Add(new Point(row - i, column + i));

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {
                        if (board[row + i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row + i, column + i));

                            break;
                        }
                        moves.Add(new Point(row + i, column + i));

                    }
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(i, column));

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }
                    for (int i = row + 1; i < 8; i++)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(i, column));

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row, j));

                            break;
                        }

                        moves.Add(new Point(row, j));

                    }
                    for (int j = column + 1; j < 8; j++)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row, j));

                            break;
                        }
                        moves.Add(new Point(row, j));

                    }
                    break;
                case Piece.BISHOP:
                    //Console.WriteLine("Bishop");
                    //left-down...I think
                    for (int i = 1; row - i > -1 && column - i > -1; i++)
                    {
                        if (board[row - i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row - i, column - i));

                            break;
                        }
                        moves.Add(new Point(row - i, column - i));

                    }
                    //left-up
                    for (int i = 1; row + i < 8 && column - i > -1; i++)
                    {
                        if (board[row + i, column - i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row + i, column - i));

                            break;
                        }
                        moves.Add(new Point(row + i, column - i));

                    }
                    //rigth-down
                    for (int i = 1; row - i > -1 && column + i < 8; i++)
                    {
                        if (board[row - i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row - i, column + i));

                            break;
                        }
                        moves.Add(new Point(row - i, column + i));

                    }
                    //rigth-up
                    for (int i = 1; row + i < 8 && column + i < 8; i++)
                    {
                        if (board[row + i, column + i].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(row + i, column + i));

                            break;
                        }
                        moves.Add(new Point(row + i, column + i));

                    }

                    break;
                case Piece.KNIGTH:
                    //Console.WriteLine("Knigth");
                    int[,] jumps =
                    {
                        { row + 1, column + 2 },
                        { row + 1, column - 2},
                        {row - 1, column + 2 },
                        { row - 1, column - 2},
                        {row + 2, column + 1 },
                        {row + 2, column - 1 },
                        { row - 2, column + 1},
                        {row - 2, column - 1 }
                    };

                    for (int i = 0; i < jumps.GetLength(0); i++)
                    {
                        int x = jumps[i, 0];
                        int y = jumps[i, 1];

                        if (x > -1 && x < 8 && y > -1 && y < 8)
                        {
                            
                                //color has to be opposing or none
                                moves.Add(new Point(x, y));
                            

                        }
                    }
                    break;
                case Piece.ROOK:
                    //Console.WriteLine("Rook");
                    //I did it in 2 times per row/column because of how I'm going to implement blocking/capturing pieces... I think :/
                    //rows
                    for (int i = row - 1; i > -1; i--)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(i, column));

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }
                    for (int i = row + 1; i < 8; i++)
                    {
                        if (board[i, column].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");

                            moves.Add(new Point(i, column));

                            break;
                        }
                        moves.Add(new Point(i, column));

                    }

                    //columns

                    for (int j = column - 1; j > -1; j--)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");
                            moves.Add(new Point(row, j));

                            break;
                        }
                        moves.Add(new Point(row, j));

                    }
                    for (int j = column + 1; j < 8; j++)
                    {
                        if (board[row, j].GetColor() != Color.NONE)
                        {
                            //Console.WriteLine("Piece in the way");
                            moves.Add(new Point(row, j));

                            break;
                        }
                        moves.Add(new Point(row, j));

                    }
                    break;
                case Piece.PAWN:
                    //Console.WriteLine("Pawn");

                    if (color == Color.WHITE)
                    {
                        
                        if (column + 1 < 8)
                        {
                            
                                moves.Add(new Point(row + 1, column + 1));
                            
                        }
                        if (column - 1 > -1)
                        {
                            
                                moves.Add(new Point(row + 1, column - 1));
                            
                        }
                    }
                    if (color == Color.BLACK)
                    {
                        if (column + 1 < 8)
                        {
                           
                                moves.Add(new Point(row - 1, column + 1));
                            
                        }
                        if (column - 1 > -1)
                        {
                           
                                moves.Add(new Point(row - 1, column - 1));
                            
                        }
                    }
                    break;

            }

            return moves;
        }

        public HashSet<Point> getattackOfColor(Color color)
        {
            //hashset because no doubles needed.

            HashSet<Point> moves = new HashSet<Point>();

            //iterate over board and determine all occupied squares
            foreach ( Square i in board)
            {
                if (i.GetColor() == color)
                {
                    foreach(Point co in getattackOfSquare(i.row, i.column))
                    {
                        moves.Add(co);
                    }
                }
            }
            return moves;
        }

        public bool isInCeck(Color color)
        {
            Color oppositeColor = Color.WHITE;
            Point sqrToPoint = new Point(0,0);

            if (color == Color.WHITE)
            {
                oppositeColor = Color.BLACK;
            }

            foreach (Square i in board)
            {
                if (i.GetColor() == color && i.GetPiece() == Piece.KING)
                {
                     sqrToPoint = new Point(i.row, i.column);
                    break;
                }
            }
            return getattackOfColor(oppositeColor).TryGetValue(sqrToPoint, out sqrToPoint);
        }

        public void PrintLegalMovesOfColor(Color color)
        {
            Color oppositeColor = Color.WHITE;
            if (color == Color.WHITE)
            {
                oppositeColor = Color.BLACK;
            }
            foreach (Square i in board)
            {
                if (i.GetColor() == color )
                {
                    List<Point> pseudos = getPseudoLegalMovesOfSquare(i.row, i.column);
                    //implement check if move is valid here.
                    foreach(Point co in pseudos)
                    {
                        ChessBoard clone = Clone();
                        clone.MakeRawMove(i.row, i.column, co.X, co.Y);
                        if (!clone.isInCeck(color))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(i.GetPiece());
                            Console.Write(" From " +i.row + " " + i.column + " to: " + co.X + " " + co.Y);
                            Console.ResetColor();
                        }
                    }
                }
            }
        }

        public List<Move> GetLegalMoves()
        {
            if(_legalMoves != null)
            {
                //Console.WriteLine("legal moves already initialised!");
                return _legalMoves;
            }
            List<Move> moves = new List<Move>();
            
            foreach (Square i in board)
            {
                if (i.GetColor() == toMove)
                {
                    List<Point> pseudos = getPseudoLegalMovesOfSquare(i.row, i.column);
                    int promotionRow = 7;
                    if(toMove == Color.BLACK)
                    {
                        promotionRow = 0;
                    }
                    //check for castling

                    //implement check if move is valid here.
                    foreach (Point co in pseudos)
                    {
                        ChessBoard clone = Clone();
                        bool capture = false;
                        if(board[co.X, co.Y].GetPiece() != Piece.NONE) {
                            //there is a piece on this square and we know its not ours because we checked for that already
                            capture = true;
                        }
                        //check for promotion
                        if(i.GetPiece() == Piece.PAWN && co.X == promotionRow)
                        {
                            clone.MakeRawMove(i.row, i.column, co.X, co.Y);
                            if (!clone.isInCeck(toMove))
                            {
                                moves.Add(new Move(new Point(i.row, i.column), co, i.GetPiece(), Piece.ROOK,capture));
                                moves.Add(new Move(new Point(i.row, i.column), co, i.GetPiece(), Piece.BISHOP, capture));
                                moves.Add(new Move(new Point(i.row, i.column), co, i.GetPiece(), Piece.KNIGTH, capture));
                                moves.Add(new Move(new Point(i.row, i.column), co, i.GetPiece(), Piece.QUEEN, capture));
                            }
                        }
                        else
                        {
                            clone.MakeRawMove(i.row, i.column, co.X, co.Y);
                            if (!clone.isInCeck(toMove))
                            {
                                moves.Add(new Move(new Point(i.row, i.column), co, i.GetPiece(), capture, false));
                            }
                        }
                        

                        
                    }

                    //check for en passants
                    List<Move> possibleEnPassants = getPossibleEnPassants();
                    foreach (Move move in possibleEnPassants)
                    {
                        ChessBoard clone = Clone();
                        clone.doEnPassant(move);
                        if (!clone.isInCeck(toMove))
                        {
                            moves.Add(move);
                        }
                        //make raw en passant
                    }

                    
                }
            }
            List<Move> castles = getPossibleCastles(); //castles already does checks for checks
            moves.AddRange(castles);
            _legalMoves = moves;
            return moves;
        }

        public void doEnPassant(Move move)
        {
            Piece piece = move.Piece; //technically not necessary but could be fun for variants or smtn
            

            board[move.From.X, move.From.Y].setPiece(Piece.NONE, Color.NONE);
            board[move.To.X, move.To.Y].setPiece(piece, toMove);

            //en passant square
            if (toMove == Color.WHITE)
            {
                board[move.To.X-1, move.To.Y].setPiece(Piece.NONE, Color.NONE);
            }
            else
            {
                board[move.To.X+1, move.To.Y].setPiece(Piece.NONE, Color.NONE);
            }
        }
        public void doPromotion(Move move)
        {


            board[move.From.X, move.From.Y].setPiece(Piece.NONE, Color.NONE);
            board[move.To.X, move.To.Y].setPiece(move.Promotion, toMove);

            
        }
        public void doCastles(Move move)
        {
            if(toMove == Color.WHITE)
            {
                if (move.CastlesKingSide)
                {
                    board[0, 4].setPiece(Piece.NONE, Color.NONE);
                    board[0, 5].setPiece(Piece.ROOK, Color.WHITE);
                    board[0, 6].setPiece(Piece.KING, Color.WHITE);
                    board[0, 7].setPiece(Piece.NONE, Color.NONE);
                }
                else
                {
                    board[0, 4].setPiece(Piece.NONE, Color.NONE);
                    board[0, 3].setPiece(Piece.ROOK, Color.WHITE);
                    board[0, 2].setPiece(Piece.KING, Color.WHITE);
                    board[0, 0].setPiece(Piece.NONE, Color.NONE);
                }
            }
            else
            {
                if (move.CastlesKingSide)
                {
                    board[7, 4].setPiece(Piece.NONE, Color.NONE);
                    board[7, 5].setPiece(Piece.ROOK, Color.BLACK);
                    board[7, 6].setPiece(Piece.KING, Color.BLACK);
                    board[7, 7].setPiece(Piece.NONE, Color.NONE);
                }
                else
                {
                    board[7, 4].setPiece(Piece.NONE, Color.NONE);
                    board[7, 3].setPiece(Piece.ROOK, Color.BLACK);
                    board[7, 2].setPiece(Piece.KING, Color.BLACK);
                    board[7, 0].setPiece(Piece.NONE, Color.NONE);
                }
            }
        }
        public void MakeRawMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            Color oppositeColor = Color.WHITE;
            if(toMove == Color.WHITE)
            {
                oppositeColor = Color.BLACK;
            }
            Piece piece = board[fromRow, fromCol].GetPiece();
            Color color = board[fromRow, fromCol].GetColor();

            board[fromRow, fromCol].setPiece(Piece.NONE, Color.NONE);
            board[toRow, toCol].setPiece(piece, color);
        }

        public string getBoardstate()
        {
            inCheck = isInCeck(toMove);
            _legalMoves = GetLegalMoves();
            if(_legalMoves.Count != 0)
            {
                if (halfMoves > 99) {
                    gameOver = true;
                    return "50 move rule";
                    
                }

                return "Playing";
                
            }
            else
            {
                gameOver = true;
                if (inCheck)
                {
                    inCheckMate = true;
                    return "checkmate";
                }
                else
                {
                    inStaleMate = true;
                    return "stalemate";
                }
            }
        }

        public bool makeLegalMove(Move move)
        {
            GetLegalMoves();
            Console.WriteLine(move);
            if (_legalMoves.Contains(move))
            {
                Color oppositeColor = Color.WHITE;
                if (toMove == Color.WHITE)
                {
                    oppositeColor = Color.BLACK;
                }
                Console.WriteLine("okidoki");
                if (move.EnPassant)
                {
                    doEnPassant(move);
                }
                else
                {
                    if (move.Promotion != Piece.NONE)
                    {
                        //promotion
                        doPromotion(move);
                    }
                    else
                    {
                        if (move.Castles) {
                            //castle
                            doCastles(move);
                        }
                        else {

                            MakeRawMove(move.From.X, move.From.Y, move.To.X, move.To.Y);
                        }
                    }
                }
                

                //set board variables
                toMove = oppositeColor;
                enPassantColumn = null;
                
                if (toMove == Color.WHITE)
                {
                    moveNumber++;
                }

                if(move.Piece == Piece.PAWN | move.Capture ) //TODO: castles or losing of castling rigths needs to change this too
                {
                    halfMoves = 0;
                }
                else
                {
                    halfMoves++;
                }
                _legalMoves = null;
                getBoardstate();
                return true;
            }
            else
            {
                Console.WriteLine("nope, illigal mate");
                return false;
            }
        }

        public bool isGameOver()
        {
            return gameOver;
        }

        public List<Move> getPossibleEnPassants()
        {
            List<Move> moves = new List<Move>();
            int enPassantRow = 3; //row where we look for a pawn of the playing color
            int direction = -1; //the direction the pawn will go, black goes -1 white goes +1
            if (enPassantColumn == null)
            {
                return moves;
            }
            if (toMove == Color.WHITE)
            {
                enPassantRow = 4;
                direction = 1;
            }
            int v = enPassantColumn ?? default(int); //int? to int cast
            if (enPassantColumn - 1 > -1)
            {
                
                if (board[enPassantRow, v-1].GetPiece() == Piece.PAWN
                        && board[enPassantRow, v-1].GetColor() == toMove)
                {
                    moves.Add(new Move(new Point(enPassantRow, v - 1), new Point(enPassantRow + direction, v), Piece.PAWN, true, true));
                }
            }
            if (enPassantColumn + 1 < 8)
            {
                if (board[enPassantRow, v +1].GetPiece() == Piece.PAWN
                        && board[enPassantRow, v-1].GetColor() == toMove)
                {
                    moves.Add(new Move(new Point(enPassantRow, v + 1), new Point(enPassantRow + direction, v), Piece.PAWN, true, true));
                }
            }





            return moves;
        }


        public List<Move> getPossibleCastles()
        {
            //this is defined for normal board setups, won't work in chess960
            List<Move> moves = new List<Move>();
            if (isInCeck(toMove))
            {
                return moves; //don't waste energy
            }
            if (toMove == Color.WHITE)
            {

                if (!wCanCastleKing && !wCanCastleQueen)
                {
                    return moves; //don't waste energy
                }
                HashSet<Point> attacks = getattackOfColor(Color.BLACK);
                if (wCanCastleKing)
                {
                    //check if squares beside are not attacked

                    Point a = new Point(0, 5);
                    Point b = new Point(0, 6);
                    if (attacks.TryGetValue(a, out a) | attacks.TryGetValue(b, out b))
                    {
                        //can't castle

                    }
                    else
                    {
                        if (board[0, 5].GetPiece() == Piece.NONE && board[0, 6].GetPiece() == Piece.NONE)
                        {
                            //can castle
                            moves.Add(new Move(new Point(0, 4), new Point(0,6), true));
                            Console.WriteLine("kingside jup");
                        }
                    }
                }
                if (wCanCastleQueen)
                {
                    //check if squares beside are not attacked

                    Point a = new Point(0, 3);
                    Point b = new Point(0, 2);
                    if (attacks.TryGetValue(a, out a) | attacks.TryGetValue(b, out b))
                    {
                        //can't castle
                        
                    }
                    else
                    {
                        if (board[0, 3].GetPiece() == Piece.NONE && board[0, 2].GetPiece() == Piece.NONE && board[0, 1].GetPiece() == Piece.NONE)
                        {
                            //can castle
                            moves.Add(new Move(new Point(0, 4), new Point(0, 2), false));
                            Console.WriteLine("queenside jup");
                        }
                    }
                }
            }
            //check for black
            if (toMove == Color.BLACK)
            {

                if (!bCanCastleKing && !bCanCastleQueen)
                {
                    return moves; //don't waste energy
                }
                HashSet<Point> attacks = getattackOfColor(Color.WHITE);
                if (bCanCastleKing)
                {
                    //check if squares beside are not attacked

                    Point a = new Point(7, 5);
                    Point b = new Point(7, 6);
                    if (attacks.TryGetValue(a, out a) | attacks.TryGetValue(b, out b))
                    {
                        //can't castle

                    }
                    else
                    {
                        if (board[7, 5].GetPiece() == Piece.NONE && board[7, 6].GetPiece() == Piece.NONE)
                        {
                            //can castle
                            Console.WriteLine("black kingside jup");
                            moves.Add(new Move(new Point(7, 4), new Point(7,6), true));
                        }
                    }
                }
                if (bCanCastleQueen)
                {
                    //check if squares beside are not attacked

                    Point a = new Point(7, 3);
                    Point b = new Point(7, 2);
                    if (attacks.TryGetValue(a, out a) | attacks.TryGetValue(b, out b))
                    {
                        //can't castle

                    }
                    else
                    {
                        if (board[7, 3].GetPiece() == Piece.NONE && board[7, 2].GetPiece() == Piece.NONE && board[7, 1].GetPiece() == Piece.NONE)
                        {
                            //can castle
                            moves.Add(new Move(new Point(7, 4), new Point(7, 2), false));
                            Console.WriteLine("black queenside jup");
                        }
                    }
                }
            }

            return moves;
        }
    }
}
