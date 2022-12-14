

namespace Chess_Learning_Application
{
    partial class Chessboard
    {
        public class Queen : Figure
        {
            public Queen(Chessboard chessboard, int x, int y, FigureColor color, int numberOfMoves = 0) : base(chessboard, x, y, color, numberOfMoves)
            {
                AlgebraicNotFirstLetter = "Q";
                _type = FigureType.Queen;
            }

            public override void SetVectorOfMoves()
            {
                Vectors.Clear();
                Position enemyKingPos = Color == FigureColor.White ? _chessboard.BlackKingPosition : _chessboard.WhiteKingPosition;

                for (int offsetY = -1; offsetY <= 1; offsetY += 2)
                {
                    VectorOfMoves v = new();
                    Position squarePosition = pos; // Position of a square we are examining

                    v.Moves.Add(new Chessboard.Move(squarePosition.X, squarePosition.Y, squarePosition.X, squarePosition.Y));

                    Figure? alreadyAttackedFigure = null;
                    bool AddToVector = true;
                    squarePosition.Y += offsetY;
                    while (squarePosition.Y >= 0 && squarePosition.Y < 8)
                    {
                        if (!(v.IsChecking == false && alreadyAttackedFigure is not null))
                            _chessboard.AttackedSquares.Add(squarePosition);
                        if (AddToVector == false)
                        {
                            squarePosition.Y += offsetY;
                            continue;
                        }
                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color) { break; }
                        Move move = new Move(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                        v.Moves.Add(move);

                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null)
                        {
                            if (alreadyAttackedFigure is not null)
                            {
                                if (enemyKingPos.X == squarePosition.X && enemyKingPos.Y == squarePosition.Y)
                                {
                                    alreadyAttackedFigure!.IsPinned = true;
                                    alreadyAttackedFigure!.IsPinnedBy = this;
                                    v.Pinns = true;
                                }
                                break;
                            }
                            // first attacked Piece in a vector
                            if (enemyKingPos.X == squarePosition.X && enemyKingPos.Y == squarePosition.Y)
                            {
                                if (_chessboard.IsCheck)
                                    _chessboard.IsDoubleCheck = true;
                                _chessboard.IsCheck = true;

                                _chessboard.CheckingVector = v;
                                v.IsChecking = true;
                                AddToVector = false;
                            }
                            alreadyAttackedFigure = _chessboard.square[squarePosition.X, squarePosition.Y].Piece;
                        } // square not null
                        squarePosition.Y += offsetY;
                    }
                    Vectors.Add(v);
                }


                for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                {
                    VectorOfMoves v = new();
                    Position squarePosition = pos; // Position of a square we are examining

                    v.Moves.Add(new Chessboard.Move(squarePosition.X, squarePosition.Y, squarePosition.X, squarePosition.Y));

                    Figure? alreadyAttackedFigure = null;
                    squarePosition.X += offsetX;
                    bool AddToVector = true;
                    while (squarePosition.X >= 0 && squarePosition.X < 8)
                    {
                        if (!(v.IsChecking == false && alreadyAttackedFigure is not null))
                            _chessboard.AttackedSquares.Add(squarePosition);
                        if (AddToVector == false)
                        {
                            squarePosition.X += offsetX;
                            continue;
                        }
                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color) { break; }
                        Move move = new Move(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                        v.Moves.Add(move);

                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null)
                        {
                            if (alreadyAttackedFigure is not null)
                            {
                                if (enemyKingPos.X == squarePosition.X && enemyKingPos.Y == squarePosition.Y)
                                {
                                    alreadyAttackedFigure!.IsPinned = true;
                                    alreadyAttackedFigure!.IsPinnedBy = this;
                                    v.Pinns = true;
                                }
                                break;
                            }

                            if (enemyKingPos.X == squarePosition.X && enemyKingPos.Y == squarePosition.Y)
                            {
                                if (_chessboard.IsCheck)
                                    _chessboard.IsDoubleCheck = true;
                                _chessboard.IsCheck = true;

                                _chessboard.CheckingVector = v;
                                v.IsChecking = true;

                                AddToVector = false;
                            }
                            alreadyAttackedFigure = _chessboard.square[squarePosition.X, squarePosition.Y].Piece;
                        } // square not null
                        squarePosition.X += offsetX;
                    }
                    Vectors.Add(v);
                }

                for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                {
                    for (int offsetY = -1; offsetY <= 1; offsetY += 2)
                    {
                        VectorOfMoves v = new();
                        Position squarePosition = pos; // Position of a square we are examining

                        v.Moves.Add(new Chessboard.Move(squarePosition.X, squarePosition.Y, squarePosition.X, squarePosition.Y));

                        squarePosition.X += offsetX;
                        squarePosition.Y += offsetY;

                        Figure? alreadyAttackedFigure = null;
                        bool AddToVector = true;
                        while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                        {
                            if (!(v.IsChecking == false && alreadyAttackedFigure is not null))
                                _chessboard.AttackedSquares.Add(squarePosition);
                            if (AddToVector == false)
                            {
                                squarePosition.X += offsetX;
                                squarePosition.Y += offsetY;
                                continue;
                            }
                            if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color) { break; }
                            Move move = new Move(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                            v.Moves.Add(move);

                            if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null)
                            {
                                if (alreadyAttackedFigure is not null)
                                {
                                    if (enemyKingPos.X == squarePosition.X && enemyKingPos.Y == squarePosition.Y)
                                    {
                                        alreadyAttackedFigure!.IsPinned = true;
                                        alreadyAttackedFigure!.IsPinnedBy = this;
                                        v.Pinns = true;
                                    }
                                    break;
                                }

                                if (enemyKingPos.X == squarePosition.X && enemyKingPos.Y == squarePosition.Y)
                                {
                                    if (_chessboard.IsCheck)
                                        _chessboard.IsDoubleCheck = true;
                                    _chessboard.IsCheck = true;

                                    _chessboard.CheckingVector = v;
                                    v.IsChecking = true;
                                    AddToVector = false;
                                }
                                alreadyAttackedFigure = _chessboard.square[squarePosition.X, squarePosition.Y].Piece;
                            } // square not null
                            squarePosition.X += offsetX;
                            squarePosition.Y += offsetY;
                        }
                        Vectors.Add(v);
                    }
                }

            } // SetVectorOfMoves();

            public override void SearchForPseudoLegalMoves()
            {
                _pseudoLegalMoves.Clear();
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        if (offsetX != 0 || offsetY != 0) // Do not check the position of the queen
                        {
                            Position squarePosition = pos;
                            squarePosition.X += offsetX;
                            squarePosition.Y += offsetY;
                            while (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                            {
                                if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color) { break; }
                                else
                                {
                                    Move move = new(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                                    _pseudoLegalMoves.Add(move);
                                    if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null) { break; }
                                    squarePosition.X += offsetX;
                                    squarePosition.Y += offsetY;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
