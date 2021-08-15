using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApp.Bots
{
    class RandomMoveBot : IBot
    {
        public Move getBestMove(ChessBoard chessBoard)
        {
            var random = new Random();
            List<Move> _moves = chessBoard.GetLegalMoves();
            int index = random.Next(_moves.Count);
            return _moves[index];
        }

        public double getEvaluation(ChessBoard chessBoard)
        {
            //this bot stupid LMAO xd 
            return 0;
        }
    }
}
