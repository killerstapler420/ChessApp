using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApp
{
    interface IBot
    {
        public Move getBestMove(ChessBoard chessBoard);
        public double getEvaluation(ChessBoard chessBoard);
    }
}
