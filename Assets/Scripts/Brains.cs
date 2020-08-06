using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts
{
    
    class Brains
    {
        public Color MyColor = Color.black;

        public string MiniMaxRoot(int depth, Chess chess, Color me)
        {
            bool IsMe;
            int BestMoveScore = -99999;
            int MoveScore;
            string BestMove = "";
            List<string> allMoves = chess.GetAllMovesPub();
            //Parallel.ForEach(allMoves, element =>//это цикл с многопоточностью
            //{
            //    Debug.Log(" - - - " + element);
            //    Chess newchess = chess.Move(element);
            //    if (chess.board.moveColor == me)
            //        IsMe = true;
            //    else
            //        IsMe = false;
            //    MoveScore = MiniMax(depth - 1, newchess, element, !IsMe, -100000, 100000);
            //    if (MoveScore > BestMoveScore)
            //    {
            //        BestMoveScore = MoveScore;
            //        BestMove = element;
            //    }
            //});
            foreach (string moves in allMoves)//это нормальный цикл, без многопоточности
            {
                //Debug.Log("" + moves);
                Chess newchess = chess.Move(moves);
                if (chess.board.moveColor == me)
                    IsMe = true;
                else
                    IsMe = false;
                MoveScore = MiniMax(depth - 1, newchess, moves, !IsMe, -100000, 100000);
                if (MoveScore > BestMoveScore)
                {
                    BestMoveScore = MoveScore;
                    BestMove = moves;
                }
               // Debug.Log("=====   " + moves + " - " + MoveScore + "   =======");
            }
           // Console.Write("\n+++++++   " + BestMove + " - " + BestMoveScore + "   +++++++\n");

            return BestMove;
        }

        int MiniMax(int depth, Chess chess, string move, bool me, int alpha, int beta)
        {
            if (depth == 0)
            {
                return MakeScore(chess, move);
            }
            int MoveScore;
            string BestMove="";
            List<string> allMoves = chess.GetAllMovesPub();
            if (allMoves.Count == 0)
            {
                
                if (chess.board.moveColor == MyColor)
                    return -9010;
                if (chess.board.moveColor != MyColor)
                    return 9010;
            }
            if (me == true)
            {
                int BestMoveScore = -99999;
                foreach (string moves in allMoves)
                {
                    Chess newchess = chess.Move(moves);

                    MoveScore = MiniMax(depth - 1, newchess, moves, !me, alpha, beta);
                    if (MoveScore > BestMoveScore)
                    {
                        BestMoveScore = MoveScore;
                        BestMove = moves;
                    }
                    alpha = Math.Max(alpha, BestMoveScore);
                    if (beta <= alpha)
                    {
                        //Debug.Log("+====--=-====");
                        // Console.Write("\n");
                        return BestMoveScore;
                    }
                }
                //  Console.Write("\n");
                return BestMoveScore;
            }
            else
            {
                int BestMoveScore = 99999;
                foreach (string moves in allMoves)
                {
                    Chess newchess = chess.Move(moves);
                    MoveScore = MiniMax(depth - 1, newchess, moves, !me, alpha, beta);
                    if (MoveScore < BestMoveScore)
                    {
                        BestMoveScore = MoveScore;
                        BestMove = moves;
                    }
                    beta = Math.Min(beta, BestMoveScore);
                    if (beta <= alpha)
                    {
                       // Debug.Log("+====--=-====");
                        //Console.Write(" --- \n");
                        return BestMoveScore;
                    }
                }
                // Console.Write(" -- \n");
                //Debug.Log("=====   " + BestMove + " - " + BestMoveScore + "   =======");
                return BestMoveScore;

            }
        }

      static int[,] Reverse(int[,] mas)
      {
            int g = 0, f = 0;
            int[,] newmas = new int[8, 8];
            for (int i = 7; i > -1; i--)
            {
                for (int j = 7; j > -1; j--)
                {
                    newmas[f, g] = mas[i, j];
                    g++;
                }
                g = 0;
                f++;
            }
            return newmas;
            //for (int i = 0; i < mas.GetLength(0); i++)
            //{
            //    for (int j = 0; j < newmas.GetLength(1); j++)
            //        Console.Write("{0} ", newmas[i, j]);
            //    Console.WriteLine();
            //}
      }

        static int[,] WhitePawnPos = { //позиции для Пешек
                {0,  0,  0,  0,  0,  0,  0,  0,},
                {50, 50, 50, 50, 50, 50, 50, 50,},
                {10, 10, 20, 30, 30, 20, 10, 10,},
                {5, 5, 10, 25, 25, 10,  5,  5,},
                {0, 0,  0, 20, 20,  0,  0,  0,},
                {5, -5,-10,  0,  0,-10, -5,  5,},
                {5, 10, 10,-20,-20, 10, 10,  5,},
                {0, 0,  0,  0,  0,  0,  1,  1}};
        int[,] BlackPawnPos = Reverse(WhitePawnPos);
        static int[,] WhiteKnightPos = {//позициии для Коней
                {-50,-40,-30,-30,-30,-30,-40,-50,},
                {-40,-20,  0,  0,  0,  0,-20,-40,},
                {-30,  0, 10, 15, 15, 10,  0,-30,},
                {-30,  5, 15, 20, 20, 15,  5,-30,},
                {-30,  0, 15, 20, 20, 15,  0,-30,},
                {-30,  5, 10, 15, 15, 10,  5,-30,},
                {-40,-20,  0,  5,  5,  0,-20,-40,},
                {-50,-40,-30,-30,-30,-30,-40,-50,}};
        int[,] BlackKnightPos = Reverse(WhiteKnightPos);
        // Array.Reverse(BlackKnightPos);
        static int[,] WhiteBishopPos = {//позициии для Слонов
                {-20,-10,-10,-10,-10,-10,-10,-20,},
                {-10,  0,  0,  0,  0,  0,  0,-10,},
                {-10,  0,  5, 10, 10,  5,  0,-10,},
                {-10,  5,  5, 10, 10,  5,  5,-10,},
                {-10,  0, 10, 10, 10, 10,  0,-10,},
                {-10, 10, 10, 10, 10, 10, 10,-10,},
                {-10,  5,  0,  0,  0,  0,  5,-10,},
                {-20,-10,-10,-10,-10,-10,-10,-20,}};
        int[,] BlackBishopPos = Reverse(WhiteBishopPos);
        // Array.Reverse(BlackBishopPos);
        static int[,] WhiteRookPos = {//позициии для Башенок
                {0,  0,  0,  0,  0,  0,  0,  0,},
                {5, 10, 10, 10, 10, 10, 10,  5,},
                {-5,  0,  0,  0,  0,  0,  0, -5,},
                {-5,  0,  0,  0,  0,  0,  0, -5,},
                {-5,  0,  0,  0,  0,  0,  0, -5,},
                {-5,  0,  0,  0,  0,  0,  0, -5,},
                {-5,  0,  0,  0,  0,  0,  0, -5,},
                {0,  0,  0,  5,  5,  0,  0,  0}};
        int[,] BlackRookPos = Reverse(WhiteRookPos);
        // Array.Reverse(BlackRookPos);
        static int[,] WhiteQueenPos = {//позициии для Ферззя
                {-20,-10,-10, -5, -5,-10,-10,-20,},
                {-10,  0,  0,  0,  0,  0,  0,-10,},
                {-10,  0,  5,  5,  5,  5,  0,-10,},
                {-5,  0,  5,  5,  5,  5,  0, -5,},
                {0,  0,  5,  5,  5,  5,  0, -5,},
                {-10,  5,  5,  5,  5,  5,  0,-10,},
                {-10,  0,  5,  0,  0,  0,  0,-10,},
                {-20,-10,-10, -5, -5,-10,-10,-20}};
        int[,] BlackQueenPos = Reverse(WhiteQueenPos);
        //  Array.Reverse(BlackQueenPos);
        static int[,] WhiteKingPos = {//позициии для Кроля в мидгейме
                {-30,-40,-40,-50,-50,-40,-40,-30,},
                {-30,-40,-40,-50,-50,-40,-40,-30,},
                {-30,-40,-40,-50,-50,-40,-40,-30,},
                {-30,-40,-40,-50,-50,-40,-40,-30,},
                {-20,-30,-30,-40,-40,-30,-30,-20,},
                {-10,-20,-20,-20,-20,-20,-20,-10,},
                {20, 20,  0,  0,  0,  0, 20, 20,},
                {20, 30, 10,  0,  0, 10, 30, 20}};
        int[,] BlackKingPos = Reverse(WhiteKingPos);



        int MakeScore(Chess chess, string move)
        {
            int score = 0;
            Figure[,] figures;
            figures = chess.board.figures;
            if (chess.IsMateToEnemy() == true)
                return 9000;
            if (chess.IsCheck() == true)
                score += 20;
            if (MyColor == Color.black)
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        if (figures[x, y] == Figure.blackPawn)
                            score += 100 + BlackPawnPos[x, y];
                        if (figures[x, y] == Figure.blackQueen)
                            score += 900 + BlackQueenPos[x, y];
                        if (figures[x, y] == Figure.blackRook)
                            score += 500 + BlackRookPos[x, y];
                        if (figures[x, y] == Figure.blackKnight)
                            score += 350 + BlackKnightPos[x, y];
                        if (figures[x, y] == Figure.blackBishop)
                            score += 350 + BlackBishopPos[x, y];
                        if (figures[x, y] == Figure.blackKing)
                            score += 200 + BlackKingPos[x, y];
                        if (figures[x, y] == Figure.whitePawn)
                            score -= 100 + WhitePawnPos[x, y];
                        if (figures[x, y] == Figure.whiteQueen)
                            score -= 900 + WhiteQueenPos[x, y];
                        if (figures[x, y] == Figure.whiteRook)
                            score -= 500 + WhiteRookPos[x, y];
                        if (figures[x, y] == Figure.whiteKnight)
                            score -= 350 + WhiteKnightPos[x, y];
                        if (figures[x, y] == Figure.whiteBishop)
                            score -= 350 + WhiteBishopPos[x, y];
                        if (figures[x, y] == Figure.whiteKing)//похоже это тут вообще не нужно
                            score -= 200 + WhiteKingPos[x, y];
                    }
            }
            if (MyColor == Color.white)
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        if (figures[x, y] == Figure.blackPawn)
                            score -= 100 + BlackPawnPos[x, y];
                        if (figures[x, y] == Figure.blackQueen)
                            score -= 900 + BlackQueenPos[x, y];
                        if (figures[x, y] == Figure.blackRook)
                            score -= 500 + BlackRookPos[x, y];
                        if (figures[x, y] == Figure.blackKnight)
                            score -= 350 + BlackKnightPos[x, y];
                        if (figures[x, y] == Figure.blackBishop)
                            score -= 350 + BlackBishopPos[x, y];
                        if (figures[x, y] == Figure.blackKing)
                            score -= 200 + BlackKingPos[x, y];
                        if (figures[x, y] == Figure.whitePawn)
                            score += 100 + WhitePawnPos[x, y];
                        if (figures[x, y] == Figure.whiteQueen)
                            score += 900 + WhiteQueenPos[x, y];
                        if (figures[x, y] == Figure.whiteRook)
                            score += 500 + WhiteRookPos[x, y];
                        if (figures[x, y] == Figure.whiteKnight)
                            score += 350 + WhiteKnightPos[x, y];
                        if (figures[x, y] == Figure.whiteBishop)
                            score += 350 + WhiteBishopPos[x, y];
                        if (figures[x, y] == Figure.whiteKing)
                            score += 200 + WhiteKingPos[x, y];
                    }
            }
            //Console.Write(move + " - " + score +"\t");
            //string fen = chess.fen;
            //string[] parts = fen.Split();
            //string[] lines = parts[0].Split('/');
            //Debug.Log("" + move + " - " + score + "");
            return score;
        }
    }
}
