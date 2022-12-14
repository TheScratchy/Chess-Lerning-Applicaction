
using System;

namespace Chess_Learning_Application
{
    public static class HelperCommands
    {
        /// <summary>
        /// Returns a letter equal to number of a column (0-a, 7-h).
        /// </summary>
        /// <param name="x">Number of a column, from 0 to 7</param>
        /// <returns>Letter pointing to a column (a-h).</returns>
        public static string GetFile(int x) 
        {
            string file = ((char)(x + 97)).ToString();
            return file;
        }
        /// <summary>
        /// Returns a number equal to letter of a column (0-a, 7-h).
        /// </summary>
        /// <param name="x">Single letter of a column, from 0 to 7</param>
        /// <returns>Number pointing to a column (0-7).</returns>
        public static int GetFile(char x) 
        {
            int file = x - 'a';
            return file;
        }

        /// <summary>
        /// Returns a string equal to number of a row (Rank).
        /// </summary>
        /// <param name="x">Single number of a row, from 0 to 7</param>
        /// <returns>Number as string pointing to a row (1-8).</returns>
        public static string GetRank(int x)
        {
            return (x + 1).ToString();
        }

        /// <summary>
        /// Returns a number equal to number of a row (Rank).
        /// </summary>
        /// <param name="x">Single char number of a row, from 1 to 8</param>
        /// <returns>Number pointing to a row (0-7).</returns>
        public static int GetRank(char x)
        {
            return Int32.Parse(x.ToString()) - 1;
        }

        /// <summary>
        /// Creates a figure for a given chessboard
        /// </summary>
        /// <param name="fig">Data about figure</param>
        /// <param name="Board">Chessboard to witch a newborn figure shall be added</param>
        /// <returns></returns>
        public static Chessboard.Figure? CreateFigure(FigureToBeCreated fig, Chessboard Board)
        {
            switch (fig.Type)
            {

                case Chessboard.FigureType.Knight:
                    {
                        return new Chessboard.Knight(Board, fig.Position.X, fig.Position.Y, fig.Color);
                    }
                case Chessboard.FigureType.Bishop:
                    {
                        return new Chessboard.Bishop(Board, fig.Position.X, fig.Position.Y, fig.Color);
                    }
                case Chessboard.FigureType.Rook:
                    {
                        return new Chessboard.Rook(Board, fig.Position.X, fig.Position.Y, fig.Color);
                    }
                case Chessboard.FigureType.Queen:
                    {
                        return new Chessboard.Queen(Board, fig.Position.X, fig.Position.Y, fig.Color);
                    }
                case Chessboard.FigureType.King:
                    {
                        return new Chessboard.King(Board, fig.Position.X, fig.Position.Y, fig.Color);
                    }
                case Chessboard.FigureType.Pawn:
                    {
                        return new Chessboard.Pawn(Board, fig.Position.X, fig.Position.Y, fig.Color);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        /// <summary>
        /// Stores data for a figure ready to be created. Contains Figure's type, color and position.
        /// </summary>
        public readonly struct FigureToBeCreated
        {
            public readonly Chessboard.FigureType Type;
            public readonly Chessboard.FigureColor Color;
            public readonly Chessboard.Position Position;
            public FigureToBeCreated(Chessboard.FigureType type, Chessboard.FigureColor color, Chessboard.Position position)
            {
                Type = type;
                Color = color;
                Position = position;
            }
        }
    } // class
}
