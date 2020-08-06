using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Board
    {
        public string fen { get; set; }
        public Figure[,] figures;
        public Color moveColor { get; set; }
        public int moveNumber { get; private set; }
        public bool LW { get; set; }// Long White
        public bool SW { get; set; }// Short White
        public bool LB { get; set; }// Long Black
        public bool SB { get; set; }// Short Black

        public bool WasJump { get; set; }
        public int TurnCounter { get; set; }

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

        Square a1 = new Square("a1");//for Rooks
        Square h1 = new Square("h1");
        Square a8 = new Square("a8");
        Square h8 = new Square("h8");

        public Board(string fen)
        {
            this.fen = fen;
            figures = new Figure[8, 8];
            Init();
        }
        void Init()
        {
            //rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            //0                                           1 2    3 4 5
            string[] parts = fen.Split();
            if (parts.Length != 6) return;
            InitFigures(parts[0]);
            if (parts[2][0] == 'K')
                this.SW = true;
            else
                this.SW = false;

            if (parts[2][1] == 'Q')
                this.LW = true;
            else
                this.LW = false;

            if (parts[2][2] == 'k')
                this.SB = true;
            else
                this.SB = false;

            if (parts[2][3] == 'q')
                this.LB = true;
            else
                this.LB = false;

            moveNumber = int.Parse(parts[5]);
            moveColor = (parts[1] == "b") ? Color.black : Color.white;
            //SetFiguresAt(new Square("a1"), Figure.whiteKing);
            // SetFiguresAt(new Square("h8"), Figure.blackKing);
            //moveColor = Color.white;
        }

        void InitFigures(string data)
        {
            for (int j = 8; j >= 2; j--)
                data = data.Replace(j.ToString(), (j - 1).ToString() + "1");
            // data = data.Replace("1", ".");
            string[] lines = data.Split('/');
            for (int y = 7; y >= 0; y--)
                for (int x = 0; x < 8; x++)
                    figures[x, y] = lines[7 - y][x] == '1' ? Figure.none : (Figure)lines[7 - y][x];
        }

        public IEnumerable<FigureOnSquare> YieldFigures()
        {
            foreach (Square square in Square.YieldSquares())
                if (GetFigureAt(square).GetColor() == moveColor)
                    yield return new FigureOnSquare(GetFigureAt(square), square);
        }

        string FenFigures()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < 8; x++)
                    sb.Append(figures[x, y] == Figure.none ? '1' : (char)figures[x, y]);
                if (y > 0)
                    sb.Append('/');
            }
            //string eight = "11111111";
            //for (int j=8; j>=2; j--)
            //{
            //    sb.Replace(eight.Substring(0, j), j.ToString());
            //}
            return sb.ToString();
        }

        void GenerateFEN()
        {
            if (WasJump == true)
                TurnCounter++;
            string[] parts = fen.Split();
            if (parts.Length != 6) return;
            this.fen = FenFigures() + " " +
                (moveColor == Color.white ? "w" : "b") + " " +
                (SW == true ? "K" : "-") +
                (LW == true ? "Q" : "-") +
                (SB == true ? "k" : "-") +// (moveColor == Color.white ? "w" : "b") + " " +
                (LB == true ? "q" : "-") + " " +
                parts[3] +//дописать смену флага взятия при смене хода
                " 0 " + moveNumber.ToString();//добавить ничью для правила 50 ходов и рокировку
        }
        public void GenFENout()
        {
            string[] parts = fen.Split();
            if (parts.Length != 6) return;
            this.fen = FenFigures() + " " +
                (moveColor == Color.white ? "w" : "b") + " " +
                (SW == true ? "K" : "-") +
                (LW == true ? "Q" : "-") +
                (SB == true ? "k" : "-") +// (moveColor == Color.white ? "w" : "b") + " " +
                (LB == true ? "q" : "-") + " " +
                parts[3] +//дописать смену флага взятия при смене хода
                " 0 " + moveNumber.ToString();//добавить ничью для правила 50 ходов и рокировку
        }
        //bool IsUnderAttack (FigureOnSquare fs)
        //{

        //    Moves moves = new Moves(this);
        //    FigureMoving fm = new FigureMoving(fs, none);
        //    if (moves.CanMove(fm))
        //        return true;
        //}

        public Figure GetFigureAt(Square square)
        {
            if (figures[square.x, square.y] == Figure.none)
                return Figure.none;
            if (square.OnBoard())
                return figures[square.x, square.y];
            return Figure.none;
        }

        public void SetFiguresAt(Square square, Figure figure)
        {
            if (square.OnBoard())
                figures[square.x, square.y] = figure;
        }


        public Board Move(FigureMoving fm) //Тут необходимо пошаманить и допилить рокировку и взятие на проходе
                                           //добавить передачу флагов рокировки
        {
            // проверяем ходит ли ладья или король и ставил флаг в фолс, в ГенФен = применить флаги!!!
            Board next = new Board(fen);
            if (fm.figure == Figure.whiteRook && fm.from == a1)//длинная рок белых
                next.LW = false;
            if (fm.figure == Figure.whiteRook && fm.from == h1)//короткая рок белых
                next.SW = false;
            if (fm.figure == Figure.blackRook && fm.from == a8)//длинная рок черных
                next.LB = false;
            if (fm.figure == Figure.blackRook && fm.from == h8)//короткая рок черных
                next.SB = false;
            if (fm.figure == Figure.blackKing)//рок черных
            {
                next.SB = false;
                next.LB = false;
            }
            if (fm.figure == Figure.whiteKing)//рок черных
            {
                next.SW = false;
                next.LW = false;
            }
            if (moveColor == Color.white)
                if (fm.figure == Figure.whitePawn)
                    if (fm.from.y == 6)
                        if (fm.promotion == Figure.none)
                            return this;
            if (moveColor == Color.black)
                if (fm.figure == Figure.blackPawn)
                    if (fm.from.y == 1)
                        if (fm.promotion == Figure.none)
                            return this;
            if (fm.promotion == Figure.blackKing || fm.promotion == Figure.whiteKing)
                return this;

            next.SetFiguresAt(fm.from, Figure.none);
            next.SetFiguresAt(fm.to, fm.promotion == Figure.none ? fm.figure : fm.promotion);
            if (moveColor == Color.black)
                next.moveNumber++;


            next.WasJump = this.WasJump;
            next.TurnCounter = this.TurnCounter;
           // Debug.Log("3 - " + moveColor);
            next.moveColor = moveColor.FlipColor();
          //  Debug.Log("4 - " + moveColor);
            next.GenerateFEN();

            //next.LB = this.LB;
            //next.LW = this.LW;
            //next.LB = this.LB;
            //next.SB = this.SB;

            next.ChangeFenPart3();
            return next;
        }
        void ChangeFenPart3()
        {
            if (TurnCounter == 2)
            {
                TurnCounter = 0;
                WasJump = false;
                string[] parts = fen.Split();
                if (parts.Length != 6) return;
                this.fen = parts[0] + " " + parts[1] + " " + parts[2] + " -" + " " + parts[4] + " " + parts[5];//добавить ничью для правила 50 ходов и рокировку
            }
        }
        public Board Rogue(string move)
        {
            //LW = false;// Long White
            //SW = false;// Short White
            //LB = false;// Long Black
            //SB = false;// Short Black
            if (move == "0-0-0")
            {

                if (moveColor == Color.white)//белая длинная рокировка
                {
                    FigureMoving LWRogueRook = new FigureMoving("Ra1d1");
                    FigureMoving LWRougeKing = new FigureMoving("Ke1c1");

                    Board next = new Board(fen);
                    next.SetFiguresAt(LWRogueRook.from, Figure.none);//ход левой ладьи при рокировке
                    next.SetFiguresAt(LWRogueRook.to, LWRogueRook.promotion == Figure.none ? LWRogueRook.figure : LWRogueRook.promotion);

                    next.SetFiguresAt(LWRougeKing.from, Figure.none);//ход короля при длинной рокировке
                    next.SetFiguresAt(LWRougeKing.to, LWRougeKing.promotion == Figure.none ? LWRougeKing.figure : LWRougeKing.promotion);

                    // if (moveColor == Color.black)
                    //   next.moveNumber++;

                    next.moveColor = moveColor.FlipColor();

                    next.LW = false;
                    next.SW = false;

                    next.GenerateFEN();

                    //next.LB = this.LB;
                    //next.LW = this.LW;
                    //next.LB = this.LB;
                    //next.SB = this.SB;
                    next.WasJump = this.WasJump;
                    next.TurnCounter = this.TurnCounter;

                    return next;
                }
                if (moveColor == Color.black)//черная длинная рокировка
                {
                    FigureMoving LBRogueRook = new FigureMoving("ra8d8");
                    FigureMoving LBRougeKing = new FigureMoving("ke8c8");

                    Board next = new Board(fen);
                    next.SetFiguresAt(LBRogueRook.from, Figure.none);//ход левой ладьи при рокировке
                    next.SetFiguresAt(LBRogueRook.to, LBRogueRook.promotion == Figure.none ? LBRogueRook.figure : LBRogueRook.promotion);

                    next.SetFiguresAt(LBRougeKing.from, Figure.none);//ход короля при длинной рокировке
                    next.SetFiguresAt(LBRougeKing.to, LBRougeKing.promotion == Figure.none ? LBRougeKing.figure : LBRougeKing.promotion);

                    // if (moveColor == Color.black)
                    next.moveNumber++;

                    next.moveColor = moveColor.FlipColor();

                    next.LB = false;
                    next.SB = false;

                    next.GenerateFEN();

                    //next.LB = this.LB;
                    //next.LW = this.LW;
                    //next.LB = this.LB;
                    //next.SB = this.SB;
                    next.WasJump = this.WasJump;
                    next.TurnCounter = this.TurnCounter;

                    return next;
                }
            }
            if (move == "0-0")
            {
                if (moveColor == Color.white)//белая короткая рокировка
                {
                    FigureMoving SWRogueRook = new FigureMoving("Rh1f1");
                    FigureMoving SWRougeKing = new FigureMoving("Ke1g1");

                    Board next = new Board(fen);
                    next.SetFiguresAt(SWRogueRook.from, Figure.none);//ход правой ладьи при рокировке
                    next.SetFiguresAt(SWRogueRook.to, SWRogueRook.promotion == Figure.none ? SWRogueRook.figure : SWRogueRook.promotion);

                    next.SetFiguresAt(SWRougeKing.from, Figure.none);//ход короля при короткой рокировке
                    next.SetFiguresAt(SWRougeKing.to, SWRougeKing.promotion == Figure.none ? SWRougeKing.figure : SWRougeKing.promotion);

                    // if (moveColor == Color.black)
                    //   next.moveNumber++;

                    next.moveColor = moveColor.FlipColor();

                    //   next.LB = this.LB;
                    //  next.LW = this.LW;
                    //  next.LB = this.LB;
                    // next.SB = this.SB;
                    next.SW = false;
                    next.LW = false;

                    next.GenerateFEN();

                    //next.LB = this.LB;
                    //next.LW = this.LW;
                    //next.LB = this.LB;
                    //next.SB = this.SB;
                    next.WasJump = this.WasJump;
                    next.TurnCounter = this.TurnCounter;
                    //next.SW = false;
                    //next.LW = false;
                    return next;
                }
                if (moveColor == Color.black)//черная корткая рокировка
                {
                    FigureMoving SBRogueRook = new FigureMoving("rh8f8");
                    FigureMoving SBRougeKing = new FigureMoving("ke8g8");

                    Board next = new Board(fen);
                    next.SetFiguresAt(SBRogueRook.from, Figure.none);//ход правой ладьи при рокировке
                    next.SetFiguresAt(SBRogueRook.to, SBRogueRook.promotion == Figure.none ? SBRogueRook.figure : SBRogueRook.promotion);

                    next.SetFiguresAt(SBRougeKing.from, Figure.none);//ход короля при короткой рокировке
                    next.SetFiguresAt(SBRougeKing.to, SBRougeKing.promotion == Figure.none ? SBRougeKing.figure : SBRougeKing.promotion);

                    // if (moveColor == Color.black)
                    next.moveNumber++;

                    next.moveColor = moveColor.FlipColor();

                    next.SB = false;
                    next.LB = false;

                    next.GenerateFEN();

                    //next.LB = this.LB;
                    //next.LW = this.LW;
                    //next.LB = this.LB;
                    //next.SB = this.SB;
                    next.WasJump = this.WasJump;
                    next.TurnCounter = this.TurnCounter;

                    return next;
                }
            }
            return this;
        }

        bool CanEatKing()
        {
            Square badKing = FindBadKing();
            Moves moves = new Moves(this);
            foreach (FigureOnSquare fs in YieldFigures())
            {
                FigureMoving fm = new FigureMoving(fs, badKing);
                if (moves.CanMove(fm))
                    return true;
            }
            return false;
        }

        private Square FindBadKing()
        {
            Figure badKing = moveColor == Color.black ? Figure.whiteKing : Figure.blackKing;
            foreach (Square square in Square.YieldSquares())
                if (GetFigureAt(square) == badKing)
                    return square;
            return Square.none;
        }

        public bool IsCheckAfterMove(FigureMoving fm)
        {
            Board after = Move(fm);
            return after.CanEatKing();
        }

        public bool IsCheck()
        {
            Board after = new Board(fen);
            after.moveColor = moveColor.FlipColor();
            return after.CanEatKing();
        }
    }
}
