
namespace Chess_Learning_Application
{
    partial class Chessboard
    {
        public class Rook : Figure
        {
            public Rook(Chessboard chessboard, int x, int y, FigureColor color, int numberOfMoves = 0) : base(chessboard, x, y, color, numberOfMoves)
            {
                _type = FigureType.Rook;
                AlgebraicNotFirstLetter = "R";
            }

            public override void SearchForPseudoLegalMoves()
            {
                _pseudoLegalMoves.Clear();
                for (int offsetY = -1; offsetY <= 1; offsetY += 2)
                {
                    Position squarePosition = pos; // Position of a square we are examining
                    squarePosition.Y += offsetY;
                    while (squarePosition.Y >= 0 && squarePosition.Y < 8)
                    {
                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color) { break; }
                        Move move = new Move(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                        _pseudoLegalMoves.Add(move);
                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null) { break; }
                        squarePosition.Y += offsetY;
                    }
                }

                for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                {
                    Position squarePosition = pos; // Position of a square we are examining
                    squarePosition.X += offsetX;
                    while (squarePosition.X >= 0 && squarePosition.X < 8)
                    {
                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color) { break; }
                        Move move = new Move(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                        _pseudoLegalMoves.Add(move);
                        if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null) { break; }
                        squarePosition.X += offsetX;
                    }
                }
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

                    squarePosition.Y += offsetY;

                    bool AddToVector = true;
                    while (squarePosition.Y >= 0 && squarePosition.Y < 8)
                    {
                        if (!(v.IsChecking == false && alreadyAttackedFigure is not null))
                            _chessboard.AttackedSquares.Add(squarePosition);
                        if (AddToVector == false)
                        {
                            if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null)
                                break;
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
                            if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null)
                                break;
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
            } // SetVectorOfMoves();
        }
    }
}
