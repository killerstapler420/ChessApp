using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApp.Bots
{
    class MaterialisticBotV1 : IBot
    {
        public Move getBestMove(ChessBoard chessBoard)
        {
            throw new NotImplementedException();
        }

        public double getEvaluation(ChessBoard chessBoard)
        {
            double eval =0;
            Square[,] board= chessBoard.GetSquares();
            for (int i = 7; i > -1; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    double value = 0;
                    switch (board[i, j].GetPiece())
                        {
                            case Piece.NONE:
                                value = 0;
                                break;
                            case Piece.KNIGTH:
                                value = 3;
                                break;
                            case Piece.BISHOP:
                            value = 3;
                            break;
                            case Piece.ROOK:
                            value = 5;
                            break;
                            case Piece.QUEEN:
                            value = 9;
                            break;
                            case Piece.PAWN:
                            value = 9;
                            break;
                    }
                    if (board[i, j].GetColor() == Color.BLACK)
                    {
                        value = value * (-1);
                    }
                    eval += value;
                }

            }
            return eval;
        }
    }
}
