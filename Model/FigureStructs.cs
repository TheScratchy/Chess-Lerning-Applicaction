using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess_Learning_Application
{
    public partial class Chessboard
    {
        public struct Move
        {
            public Move(int sx, int sy, int dx, int dy)
            {
                SX = sx;
                SY = sy;
                DX = dx;
                DY = dy;
            }

            //source
            public int SX { get; private set; }
            public int SY { get; private set; }
            //destination
            public int DX { get; private set; }
            public int DY { get; private set; }

            public FigureType PromoteTo { get; set; } = FigureType.Pawn;
            public bool WasCapturing { get; set; } = false;
            public bool WasPromotion { get; set; } = false;
            public bool WasCastling { get; set; } = false;

            // FIDE notation
            public string? AlgebraicNotation { get; set; } = null;

            public void SetAlgebraicNotation(Chessboard chessboard)
            {
                Figure movingFigure = chessboard.square[SX, SY].Piece!;

                switch (movingFigure.Type)
                {
                    case FigureType.Pawn:
                        {
                            if (DX != SX) // 
                                {
                                    AlgebraicNotation = HelperCommands.GetFile(SX) + "x" + HelperCommands.GetFile(DX) + (DY+1).ToString();
                                return;
                                }
                            break;
                        }
                    case FigureType.King:
                        {
                            if (Math.Abs(DX - SX) == 2)
                            {
                                if (DX == 2)
                                    AlgebraicNotation = "O - O - O";

                                if (DX == 6)
                                    AlgebraicNotation = "O - O";

                                return;
                            }
                            break;
                        }
                }

                string notation = HelperCommands.GetFile(DX) + (DY+1).ToString();
                // movingFigure.AlgebraicNotFirstLetter

                if (chessboard.square[DX, DY].Piece is not null) // capturing
                    notation = "x" + notation;
                foreach (Figure piece in chessboard.Figures)
                {
                    if (!(piece.Type == movingFigure.Type && piece.Color == movingFigure.Color))
                        continue;

                    foreach (Move m in piece.LegalMoves)
                    {
                        if (!(m.DY == this.DY && m.DX == this.DX) || (m.SX == this.SX && m.SY == this.SY) ) // a friendly piece can move here
                            continue;

                        if (m.SX == this.SX) // the same file (column)
                            notation = (SY+1).ToString() + notation;

                        else if(m.SY == this.SY) 
                            notation = HelperCommands.GetFile(SX) + notation;
                        break;
                    }
                }

                notation = movingFigure.AlgebraicNotFirstLetter + notation;

                AlgebraicNotation = notation;
            }

            public static Move GetLastMoveFromPNG(string moves)
            {
                Move finalMove = new();
                Chessboard board = new();
                board.SetChessbordUsingFEN(StartingFEN);

                string[] move = moves.Split(" ");

                foreach (var m in move)
                {
                    if (m == "")
                        break;
                    Move mov = Move.GetMoveFromPNG(board, m);
                    board.square[mov.SX, mov.SY].Piece!.Move(mov);
                    board.Turn = board.Turn == FigureColor.White ? FigureColor.Black : FigureColor.White;
                    finalMove = mov;
                }
                return finalMove;
            }


            /// <summary>
            /// Returns Move for given chessboard and PNG notation.
            /// Does not allow promotions.
            /// </summary>
            /// <param name="board"></param>
            /// <param name="move"></param>
            /// <returns></returns>
            public static Move GetMoveFromPNG(Chessboard board, string move)
            {
                if (move.Last() == '+')
                {
                    string temp = move.Split('+')[0];
                    move = temp;
                }


                if (move == "O-O")
                {
                    Position kingPos = board.Turn == FigureColor.White ? board.WhiteKingPosition : board.BlackKingPosition;
                    return new(kingPos.X, kingPos.Y, kingPos.X + 2, kingPos.Y);
                }

                if (move == "O-O-O")
                {
                    Position kingPos = board.Turn == FigureColor.White ? board.WhiteKingPosition : board.BlackKingPosition;
                    return new(kingPos.X, kingPos.Y, kingPos.X - 2, kingPos.Y);
                }

                char m = move[move.Length - 2];
                if (m == '=')
                    m = move[move.Length - 4];

                int DFile = HelperCommands.GetFile(m);

                m = move[move.Length - 1];
                if (char.IsLetter(m))
                    m = move[move.Length - 3];

                int DRank = HelperCommands.GetRank(m);

                FigureColor colorToMove = board.Turn;
                int offsetY = colorToMove == FigureColor.White ? -1 : 1;

                int SFile = -1, SRank = -1;

                #region Pawn Straight
                if (move.Length == 2)
                {
                    if (board.square[DFile, DRank + offsetY].Piece != null)
                    {
                        SFile = DFile;
                        SRank = DRank + offsetY;
                        return new(SFile, SRank, DFile, DRank);
                    }

                    // two squares move
                    SFile = DFile;
                    SRank = DRank + (2 * offsetY);
                    return new(SFile, SRank, DFile, DRank);
                }
                #endregion

                #region Pawn capture
                if (move[1] == 'x' && char.IsLower(move[0]))
                {
                    SFile = HelperCommands.GetFile(move[0]);
                    SRank = DRank + offsetY;

                    return new(SFile, SRank, DFile, DRank);
                }
                #endregion

                if (move.Length >=4 && move[1] != 'x')
                {
                    if (char.IsLetter(move[1]))
                    {
                        SFile = HelperCommands.GetFile(move[1]);
                    }
                    if (char.IsDigit(move[1]))
                    {
                        SRank = HelperCommands.GetRank(move[1]);
                    }
                }

                #region Figures
                switch (move[0])
                {
                    case 'K':
                        {
                            Position kingPos = board.Turn == FigureColor.White ?
                                board.WhiteKingPosition : board.BlackKingPosition;

                            return new(kingPos.X, kingPos.Y, DFile, DRank);
                        }
                    case 'N':
                        {
                            for (int offset = -2; offset <= 2; offset += 4)
                            {
                                for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                                {
                                    Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                    squarePosition.X += offsetX;
                                    squarePosition.Y += offset;
                                    if ((SFile != -1 && squarePosition.X != SFile) || 
                                        (SRank != -1 && squarePosition.Y != SRank))
                                        continue;

                                    if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                    {
                                        if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Knight &&
                                            board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                        {
                                            return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                        }
                                    }
                                }
                            }

                            for (int offset = -1; offset <= 1; offset += 2)
                            {
                                for (int offsetX = -2; offsetX <= 2; offsetX += 4)
                                {
                                    Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                    squarePosition.X += offsetX;
                                    squarePosition.Y += offset;
                                    if ((SFile != -1 && squarePosition.X != SFile) ||
                                        (SRank != -1 && squarePosition.Y != SRank))
                                        continue;

                                    if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                    {
                                        if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Knight &&
                                            board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                        {
                                            return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                        }
                                    }
                                }
                            }
                            return new();
                        }
                    case 'B':
                        {
                            for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                            {
                                for (int offset = -1; offset <= 1; offset += 2)
                                {
                                    Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                    squarePosition.X += offsetX;
                                    squarePosition.Y += offset;

                                    if ((SFile != -1 && squarePosition.X != SFile) ||
                                        (SRank != -1 && squarePosition.Y != SRank))
                                        continue;

                                    while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                    {
                                        if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Bishop &&
                                            board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                        {
                                            return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                        }
                                        squarePosition.X += offsetX;
                                        squarePosition.Y += offset;
                                    }
                                }
                            }
                            return new();
                        }
                    case 'R':
                        {
                            for (int offset = -1; offset <= 1; offset += 2)
                            {
                                Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                squarePosition.Y += offset;

                                if ((SFile != -1 && squarePosition.X != SFile) ||
                                    (SRank != -1 && squarePosition.Y != SRank))
                                    continue;

                                while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                {
                                    if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Rook &&
                                        board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                    {
                                        return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                    }
                                    squarePosition.Y += offset;
                                }
                            }

                            for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                            {
                                Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                squarePosition.X += offsetX;

                                if ((SFile != -1 && squarePosition.X != SFile) ||
                                    (SRank != -1 && squarePosition.Y != SRank))
                                    continue;

                                while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                {
                                    if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Rook &&
                                        board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                    {
                                        return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                    }
                                    squarePosition.X += offsetX;
                                }
                            }
                            return new();
                        }
                    case 'Q':
                        {
                            for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                            {
                                for (int offset = -1; offset <= 1; offset += 2)
                                {
                                    Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                    squarePosition.X += offsetX;
                                    squarePosition.Y += offset;

                                    if ((SFile != -1 && squarePosition.X != SFile) ||
                                        (SRank != -1 && squarePosition.Y != SRank))
                                        continue;

                                    while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                    {
                                        if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Queen &&
                                            board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                        {
                                            return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                        }
                                        squarePosition.X += offsetX;
                                        squarePosition.Y += offset;
                                    }
                                }
                            }

                            for (int offset = -1; offset <= 1; offset += 2)
                            {
                                Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                squarePosition.Y += offset;

                                if ((SFile != -1 && squarePosition.X != SFile) ||
                                    (SRank != -1 && squarePosition.Y != SRank))
                                    continue;

                                while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                {
                                    if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Queen &&
                                        board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                    {
                                        return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                    }
                                    squarePosition.Y += offset;
                                }
                            }

                            for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                            {
                                Position squarePosition = new(DFile, DRank); // Position of a square we are examining
                                squarePosition.X += offsetX;

                                if ((SFile != -1 && squarePosition.X != SFile) ||
                                    (SRank != -1 && squarePosition.Y != SRank))
                                    continue;

                                while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                                {
                                    if (board.square[squarePosition.X, squarePosition.Y].Piece?.Type == FigureType.Queen &&
                                        board.square[squarePosition.X, squarePosition.Y].Piece?.Color == colorToMove)
                                    {
                                        return new(squarePosition.X, squarePosition.Y, DFile, DRank);
                                    }
                                    squarePosition.X += offsetX;
                                }
                            }
                            return new();
                        }
                }
                #endregion

                return new();
            }
        }

        public struct Position
        { /* set to 0 as default = out of chessboard*/
            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }
            public Position() { }
            public int X { readonly get; set; } = 0;
            public int Y { readonly get; set; } = 0;
        }

        public struct VectorOfMoves
        {
            public VectorOfMoves() { }
            public List<Move> Moves = new();
            public bool IsChecking { get; set; } = false;
            public bool Pinns { get; set; } = false;
        }

        public enum FigureType
        {
            Pawn = 1,
            Knight = 2,
            Bishop = 3,
            Rook = 4,
            Queen = 5,
            King = 6
        }

        public enum FigureColor
        {
            White = 0,
            Black = 1
        }
    }
}

