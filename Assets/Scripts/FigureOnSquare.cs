using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class FigureOnSquare
    {
        public Figure figure {get; private set;}
        public Square square {get; private set;}

        public FigureOnSquare (Figure figure, Square square)
        {
            this.figure = figure;
            this.square = square;
        }
    }
}
