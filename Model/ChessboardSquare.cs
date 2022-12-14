
namespace Chess_Learning_Application
{
    partial class Chessboard
    {
        /// <summary>
        /// Square on chessboard. Contains a reference to a figure.
        /// </summary>
        public class Square
        {
            private readonly Position _squarePosition;
            public Position SquarePosition { get { return _squarePosition; } }

            public Figure? Piece = null;

            private readonly FigureColor _squareColor;
            public FigureColor SquareColor { get { return _squareColor; } }

            public bool IsBeingAttacked { get; set; } = false;

            public Square(int x, int y, FigureColor color)
            {
                _squarePosition = new Position(x, y);
                _squareColor = color;
            }
        }
    }
}
