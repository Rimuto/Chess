using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class Moves
    {
        FigureMoving fm;
        Board board;

        public Moves(Board board)
        {
            this.board = board;
        }
        public bool CanMove(FigureMoving fm)
        {
            this.fm = fm;
            return
                CanMoveFrom() &&
                CanMoveTo() &&
                CanFigureMove();
        }
        bool CanMoveFrom()
        {
            return fm.from.OnBoard() &&
                fm.figure.GetColor() == board.moveColor;
        }
        bool CanMoveTo()
        {
            return fm.to.OnBoard() &&
                fm.from != fm.to &&
                    board.GetFigureAt(fm.to).GetColor() != board.moveColor;
        }
        bool CanFigureMove()
        {
            switch (fm.figure)
            {
                case Figure.whiteKing:
                case Figure.blackKing:
                    return CanKingMove();
                    //break;

                case Figure.whiteQueen:
                case Figure.blackQueen:
                    return CanStraightMove();
                    
                // break;

                case Figure.whiteRook:
                case Figure.blackRook:
                    return (fm.SignX == 0 || fm.SignY==0) && CanStraightMove();
                // break;

                case Figure.whiteBishop:
                case Figure.blackBishop:
                    return (fm.SignX != 0 && fm.SignY != 0) && CanStraightMove();
                //break;

                case Figure.whiteKnight:
                case Figure.blackKnight:
                    return CanKnightMove();
                    //break;

                case Figure.whitePawn://взятие на проходе
                case Figure.blackPawn:
                    return CanPawnMove();
                //break;
                default: return false;
            }
        }
         
        bool CanPawnMove()
        {
            if (fm.from.y < 1 || fm.from.y > 6)
                return false;
            int stepY = fm.figure.GetColor() == Color.white ? 1 : -1;
            if (CanPawnGo(stepY))
                return true;
            if (CanPawnJump(stepY))
                return true;
            if (CanPawnEat(stepY))
                return true;
            return false;
            //return
            //    CanPawnGo(stepY) || CanPawnJump(stepY) || CanPawnEat(stepY);//взятие на проходе
        }
        //bool CanKingRogue()
        //{



        //}
        
        private bool CanPawnGo(int stepY)
        {
            if (board.GetFigureAt(fm.to) == Figure.none)
                if (fm.DeltaX == 0)
                    if (fm.DeltaY == stepY)
                        return true;
            return false;
        }

        private bool CanPawnJump(int stepY)
        {
            //if (fm.figure==Figure.blackPawn || fm.figure == Figure.whitePawn)
            if (board.GetFigureAt(fm.to) == Figure.none)
                if (fm.DeltaX == 0)
                    if (fm.DeltaY == 2 * stepY)
                        if (fm.from.y == 1 || fm.from.y == 6)
                            if (board.GetFigureAt(new Square(fm.from.x, fm.from.y + stepY)) == Figure.none)//если прыжок возможен парсить в FEN значение поля
                            {
                                //UpdateFen();
                                return true;
                            }
            return false;
        }

        public bool TestPawnJump(FigureMoving fm)
        {
            int stepY = fm.figure.GetColor() == Color.white ? 1 : -1;
            if (fm.figure == Figure.whitePawn || fm.figure == Figure.blackPawn)
                if (CanPawnJump(stepY))
                    return true;
            return false;
        }

        private void UpdateFen()// clean
        {
           string Updt;
           string[] parts = board.fen.Split();
           board.TurnCounter = 0;
           board.WasJump = true;
           if (parts.Length != 6) return;
           Updt =  Convert.ToChar('a' + fm.from.x).ToString();
           Updt+= Convert.ToChar('1' + fm.from.y).ToString();
           parts[3] = Updt;
           board.fen = parts[0] +" "+ parts[1]+ " " + parts[2]+ " " + parts[3]+ " " + parts[4]+ " " + parts[5];
        }

        private bool CanPawnEat(int stepY)
        {
            string[] parts = board.fen.Split();
            Square sq = new Square(parts[3]);
            if (board.GetFigureAt(fm.to) != Figure.none)//Если на предыдущем ходу отмечен ход через две клетки, Если fm.to =отмеченному полю, если смещение в рамках правил
                if (fm.AbsDeltaX == 1)
                    if (fm.DeltaY == stepY)
                        return true;
            if (fm.to == sq)
                if (fm.AbsDeltaX == 1)
                    if (fm.DeltaY == stepY)
                        return true;
            return false;
        }

        public bool TestPawnEat(FigureMoving fm)
        {
            int stepY = fm.figure.GetColor() == Color.white ? 1 : -1;
            if (fm.figure == Figure.whitePawn || fm.figure == Figure.blackPawn)
                if (CanPawnEat(stepY))
                    return true;
            return false;
        }

        private bool CanKnightMove()
        {
            if (fm.AbsDeltaX == 1 && fm.AbsDeltaY == 2)
                return true;
            if (fm.AbsDeltaX == 2 && fm.AbsDeltaY == 1)
                return true;
            return false;
        }

        private bool CanKingMove()
        {
            if (fm.AbsDeltaX <= 1 && fm.AbsDeltaY <= 1)
                return true;
            return false;
        }

        private bool CanStraightMove()
        {
            Square at = fm.from;
            do
            {
                at = new Square(at.x + fm.SignX, at.y + fm.SignY);//проблема для генерации ходов, как определить куда можно пойти?
                if (at == fm.to)
                    return true;
            } while (at.OnBoard() && board.GetFigureAt(at) == Figure.none);
            return false;
        }
    }
}
