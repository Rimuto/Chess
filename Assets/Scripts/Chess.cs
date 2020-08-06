using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts
{
    class Chess
    {
        public Board board;


        Square g1 = new Square("g1");//for short white
        Square f1 = new Square("f1");
        Square b1 = new Square("b1");//for long white
        Square c1 = new Square("c1");
        Square d1 = new Square("d1");

        Square g8 = new Square("g8");//for short black
        Square f8 = new Square("f8");
        Square b8 = new Square("b8");//for long black
        Square c8 = new Square("c8");
        Square d8 = new Square("d8");
        //rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
        public string fen { get; private set; }
        Moves moves;
        List<FigureMoving> allMoves;
        public Chess(string fen/* = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"*/)
        {
            this.fen = fen;
            board = new Board(fen);
            //board.LW = true;
            //board.LB = true;
            //board.SW = true;
            //board.SB = true;

            moves = new Moves(board);
        }

        Chess(Board board)
        {
            this.board = board;
            this.fen = board.fen;
            moves = new Moves(board);
            //board.LW = true;
            //board.LB = true;
            //board.SW = true;
            //board.SB = true;
        }

        public bool Rogue()
        {


            return true;
        }


        private void UpdateFen(FigureMoving fm)
        {
            string Updt;
            string[] parts = board.fen.Split();
            board.TurnCounter = 0;
            board.WasJump = true;
            if (parts.Length != 6) return;
            Updt = Convert.ToChar('a' + fm.from.x).ToString();
            if (board.moveColor == Color.white)
            {
                Updt += Convert.ToChar('1' + fm.from.y + 1).ToString();
            }
            if (board.moveColor == Color.black)
            {
                Updt += Convert.ToChar('1' + fm.from.y - 1).ToString();
            }
            parts[3] = Updt;
            board.fen = parts[0] + " " + parts[1] + " " + parts[2] + " " + parts[3] + " " + parts[4] + " " + parts[5];
        }

        public Chess Move(string move)
        {
            //если рокировка возможна, то рокировка
            if (move == "0-0" || move == "0-0-0")
            {
                if (IsRoguePossible(move) == true)
                {
                    //Board nextBoard1 = new Board(fen);
                    Board nextBoard1 = board.Rogue(move);
                    Chess NextChess1 = new Chess(nextBoard1);
                    //nextBoard1.LW = board.LW;
                    //nextBoard1.LB = board.LB;
                    //nextBoard1.SW = board.SW;
                    //nextBoard1.SB = board.SB;

                    return NextChess1;
                }
                return this;
            }
            FigureMoving fm = new FigureMoving(move);//
            int stepY = fm.figure.GetColor() == Color.white ? 1 : -1;

            if (board.IsCheckAfterMove(fm))
                return this;
            if (!moves.CanMove(fm))
                return this;
            if (moves.TestPawnJump(fm))//тут логичнее конечно вызывать метод board'а, но что есть - то есть
            {
                UpdateFen(fm);//ЕСли это ход пешкой через две клетки, то меняем Fen
                board.WasJump = true;
                //board.TurnCounter=1;
                //Потом ставим флаг в board что был совершен прыжок пешки
                //
            }


            Board nextBoard = board.Move(fm);
            if (moves.TestPawnEat(fm))
            {
                int y = fm.to.y;
                int x = fm.to.x;
                if (nextBoard.moveColor == Color.black)
                {
                    Square sq = new Square(x, y - 1);
                    if (nextBoard.GetFigureAt(sq).GetColor() == Color.black)
                        nextBoard.SetFiguresAt(sq, Figure.none);
                }
                if (nextBoard.moveColor == Color.white)
                {
                    Square sq = new Square(x, y + 1);
                    if (nextBoard.GetFigureAt(sq).GetColor() == Color.white)
                        nextBoard.SetFiguresAt(sq, Figure.none);
                }
                nextBoard.GenFENout();
            }
            Chess NextChess = new Chess(nextBoard);
            //nextBoard.LW = board.LW;
            //nextBoard.LB = board.LB;
            //nextBoard.SW = board.SW;
            //nextBoard.SB = board.SB;
            return NextChess;
        }

        bool IsRoguePossible(string move)// проверка на возможность рокировки
        {
            //проверить нотацию // Доделать утром
            string[] parts = fen.Split();
            string CanRogue = parts[2];// код в котором я не уверен, может не работать, при проверке смотреть сюдаЁ!!

            //if (board.moveColor == Color.black)
            //    board.moveColor = Color.white;
            //if (board.moveColor == Color.white)
            //    board.moveColor = Color.black;
            board.moveColor = board.moveColor.FlipColor();
            findAllMoves();
            board.moveColor = board.moveColor.FlipColor();
            //if (board.moveColor == Color.black)
            //    board.moveColor = Color.white;
            //if (board.moveColor == Color.white)
            //    board.moveColor = Color.black;
            if (move == "0-0-0")
            {
                if (board.moveColor == Color.white)
                {
                    if (GetFigureAt("a1") != 'R')
                        return false;
                    if (CanRogue[1] == '-')
                        return false;

                    if (IsFigureAt(b1) == true || IsFigureAt(c1) == true || IsFigureAt(d1) == true)
                        return false;
                    foreach (FigureMoving fm1 in allMoves)
                        if (fm1.to == b1 || fm1.to == c1 || fm1.to == d1)
                            return false;

                }
                if (board.moveColor == Color.black)
                {
                    if (GetFigureAt("a8") != 'r')
                        return false;

                    if (CanRogue[3] == '-')
                            return false;
                        if (IsFigureAt(b8) == true || IsFigureAt(c8) == true || IsFigureAt(d8) == true)
                            return false;
                        foreach (FigureMoving fm1 in allMoves)
                            if (fm1.to == b8 || fm1.to == c8 || fm1.to == d8)
                                return false;
                    
                }

            }
            if (move == "0-0")
            {
                bool tr = IsFigureAt(f1);
                if (board.moveColor == Color.white)
                {
                    if (GetFigureAt("h1") != 'R')
                        return false;
                    if (CanRogue[0] == '-')
                        return false;
                    if (IsFigureAt(f1) == true || IsFigureAt(g1) == true)
                        return false;
                    foreach (FigureMoving fm1 in allMoves)
                        if (fm1.to == f1 || fm1.to == g1)
                            return false;
                }
                if (board.moveColor == Color.black)
                {
                    if (GetFigureAt("h8") != 'r')
                        return false;
                    if (CanRogue[2] == '-')
                        return false;
                    if (IsFigureAt(f8) == true || IsFigureAt(g8) == true)
                        return false;
                    foreach (FigureMoving fm1 in allMoves)
                        if (fm1.to == f8 || fm1.to == g8)
                            return false;
                }
            }
            return true;
        }

        bool IsFigureAt(Square sq)
        {
            Figure f = board.GetFigureAt(sq);
            return f == Figure.none ? false : true;
        }

        public char GetFigureAt(int x, int y)
        {
            Square square = new Square(x, y);
            Figure f = board.GetFigureAt(square);
            return f == Figure.none ? '.' : (char)f;
        }

        public char GetFigureAt(string sqr)
        {
            Square square = new Square(sqr);
            Figure f = board.GetFigureAt(square);
            return f == Figure.none ? '.' : (char)f;
        }
        void findAllMoves()
        {
            allMoves = new List<FigureMoving>();
            foreach (FigureOnSquare fs in board.YieldFigures())
                foreach (Square to in Square.YieldSquares())
                {
                    FigureMoving fm = new FigureMoving(fs, to);
                    if (moves.CanMove(fm))
                        if (!board.IsCheckAfterMove(fm))
                            allMoves.Add(fm);
                }
        }

        public List<string> GetAllMovesPub()//новая функция для вызова из брэинзз
        {
            // allMoves = new List<FigureMoving>();
            findAllMoves();
            List<string> list = new List<string>();
            foreach (FigureMoving fm in allMoves)
            {
                //Debug.Log("fffff     " + fm.from.ToString());
                if (fm.figure == Figure.blackPawn)
                    if (fm.from.y == 1 && fm.to.y == 0)
                    {
                       // Debug.Log("fffff     "+ fm.from.ToString()[1]);
                        list.Add(fm.ToString() + "q");
                        continue;
                    }
                if (fm.figure == Figure.whitePawn)
                    if (fm.from.y == 6 && fm.to.y == 7)
                    {
                        list.Add(fm.ToString() + "Q");
                        continue;
                    }
                list.Add(fm.ToString());
            }
            if (IsRoguePossible("0-0") == true)
                list.Add("0-0");
            if (IsRoguePossible("0-0-0") == true)
                list.Add("0-0-0");
            return list;
        }

        public List<string> GetAllMoves()
        {
            findAllMoves();
            List<string> list = new List<string>();
            foreach (FigureMoving fm in allMoves)
                list.Add(fm.ToString());
            return list;
        }

        public bool IsCheck()
        {
            return board.IsCheck();
        }

        public bool IsMate()
        {

            List<string> list = GetAllMoves();
            if (list.Count == 0 && (IsCheck() == true))
                return true;
            return false;
        }



        public bool IsPat()
        {
            List<string> list = GetAllMoves();
            Chess next = new Chess(fen);
            next.board.moveColor.FlipColor();
            if (list.Count == 0 && (next.IsCheck() != true))
                return true;
            return false;
        }

        public bool IsMateToEnemy()
        {
            Chess next = new Chess(fen);
            next.board.moveColor.FlipColor();
            List<string> list = next.GetAllMoves();
            next.board.moveColor.FlipColor();
            if (list.Count == 0 && (IsCheck() == true))
                return true;
            return false;
        }
    }
}
