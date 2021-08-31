using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApp.Bots
{
    class MaterialisticBotV2 : IBot
    {
        private int _depth;

        public MaterialisticBotV2(int depth)
        {
            _depth = depth;
        }
        public Move getBestMove(ChessBoard chessBoard)
        {
            List<Move> bestMoves = new List<Move>();
            double eval;
            Color color = chessBoard.getToMove();
            if (color == Color.WHITE)
            {
                eval = -180; //lowest possible score
            }
            else
            {
                eval = 180; // highest possible score
            }
            foreach (Move move in chessBoard.GetLegalMoves())
            {
                double temp_eval = 0; //declared because stupid machine needs it to be...
                ChessBoard clone = chessBoard.Clone();
                clone.makeLegalMove(move);

                //evaluate the position
                temp_eval = getEvaluation(clone);
                Console.WriteLine(move + " : " + temp_eval);
                

                if (temp_eval == eval)
                {
                    bestMoves.Add(move);
                }
                else
                {
                    if (color == Color.WHITE && temp_eval > eval)
                    {
                        eval = temp_eval;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }
                    else
                    {
                        if (color == Color.BLACK && temp_eval < eval)
                        {
                            eval = temp_eval;
                            bestMoves.Clear();
                            bestMoves.Add(move);
                        }

                    }
                }


            }

            //pick random move of bestmoves list
            var random = new Random();
            int index = random.Next(bestMoves.Count);
            return bestMoves[index];

        }

        public double getEvaluation(ChessBoard chessBoard)
        {
            return _getEvaluation(chessBoard, _depth);
        }

        private double _getEvaluation(ChessBoard chessBoard, int depth)
        {
            double eval = 0;
            //base case
            if (depth == 0)

            {

                switch (chessBoard.getBoardstate())
                {
                    case BoardState.PLAYING:
                        //calc further below
                        break;
                    case BoardState.DRAW:
                        return 0;
                    case BoardState.LOSS:
                        return -180;
                    case BoardState.WIN:
                        return 180;

                }

                Square[,] board = chessBoard.GetSquares();
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
                                value = 1;
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
            else
            {
                switch (chessBoard.getBoardstate())
                {
                    //check if there is check or stalemate, this is so the program doesn't calculate further
                    case BoardState.PLAYING:
                        //calc further below
                        break;
                    case BoardState.DRAW:
                        return 0;
                    case BoardState.LOSS:
                        return -180;
                    case BoardState.WIN:
                        return 180;

                }
                List<Move> bestMoves = new List<Move>();
                Color color = chessBoard.getToMove();
                if (color == Color.WHITE)
                {
                    eval = -180; //lowest possible score
                }
                else
                {
                    eval = 180; // highest possible score
                }
                foreach (Move move in chessBoard.GetLegalMoves())
                {
                    double temp_eval = 0; //declared because stupid machine needs it to be...
                    ChessBoard clone = chessBoard.Clone();
                    clone.makeLegalMove(move);


                    //evaluate the position
                    temp_eval = _getEvaluation(clone, depth-1);
                    

                    //Console.WriteLine(move + " : " + temp_eval);

                    if (temp_eval == eval)
                    {
                        bestMoves.Add(move);
                    }
                    else
                    {
                        if (color == Color.WHITE && temp_eval > eval)
                        {
                            eval = temp_eval;
                            bestMoves.Clear();
                            bestMoves.Add(move);
                        }
                        else
                        {
                            if (color == Color.BLACK && temp_eval < eval)
                            {
                                eval = temp_eval;
                                bestMoves.Clear();
                                bestMoves.Add(move);
                            }

                        }
                    }


                }
            }

                return eval;
            
        }
            
    }
}
