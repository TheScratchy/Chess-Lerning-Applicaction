
namespace Chess_Learning_Application
{
    partial class Chessboard
    {
        public class Knight : Figure
        {
            public Knight(Chessboard chessboard, int x, int y, FigureColor color, int numberOfMoves = 0) : base(chessboard, x, y, color, numberOfMoves)
            {
                _type = FigureType.Knight;
                AlgebraicNotFirstLetter = "N";
            }

            public override void SearchForPseudoLegalMoves()
            {
                _pseudoLegalMoves.Clear();
                for (int offsetY = -2; offsetY <= 2; offsetY += 4)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                    {
                        Position squarePosition = pos; // Position of a square we are examining
                        squarePosition.X += offsetX;
                        squarePosition.Y += offsetY;
                        if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                        {
                            if (!(_chessboard.square[squarePosition.X, squarePosition.Y].Piece is not null && _chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color))
                            {
                                Move move = new(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                                _pseudoLegalMoves.Add(move);
                            }
                        }
                    }
                }

                for (int offsetY = -1; offsetY <= 1; offsetY += 2)
                {
                    for (int offsetX = -2; offsetX <= 2; offsetX += 4)
                    {
                        Position squarePosition = pos; // Position of a square we are examining
                        squarePosition.X += offsetX;
                        squarePosition.Y += offsetY;
                        if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                        {
                            if (!(_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color == Color))
                            {
                                Move move = new Move(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                                _pseudoLegalMoves.Add(move);
                            }
                        }
                    }
                }
            }

            public override void SetVectorOfMoves()
            {
                Vectors.Clear();
                Position enemyKingPos = Color == FigureColor.White ? _chessboard.BlackKingPosition : _chessboard.WhiteKingPosition;

                for (int offsetY = -2; offsetY <= 2; offsetY += 4)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                    {
                        VectorOfMoves v = new();
                        Position squarePosition = pos; // Position of a square we are examining


                        v.Moves.Add(new Move(pos.X, pos.Y, pos.X, pos.Y));

                        squarePosition.X += offsetX;
                        squarePosition.Y += offsetY;

                        if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                        {
                            _chessboard.AttackedSquares.Add(squarePosition);
                            if (squarePosition.X == enemyKingPos.X && squarePosition.Y == enemyKingPos.Y)
                            {
                                if (_chessboard.IsCheck)
                                    _chessboard.IsDoubleCheck = true;

                                _chessboard.IsCheck = true;
                                v.IsChecking = true;

                                _chessboard.CheckingVector = v;
                            }
                            Move move = new(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                            v.Moves.Add(move);

                        }
                        Vectors.Add(v);
                    }
                }

                for (int offsetY = -1; offsetY <= 1; offsetY += 2)
                {
                    for (int offsetX = -2; offsetX <= 2; offsetX += 4)
                    {
                        VectorOfMoves v = new();
                        Position squarePosition = pos; // Position of a square we are examining

                        squarePosition.X += offsetX;
                        squarePosition.Y += offsetY;


                        v.Moves.Add(new Move(pos.X, pos.Y, pos.X, pos.Y));

                        if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                        {
                            _chessboard.AttackedSquares.Add(squarePosition);
                            if (squarePosition.X == enemyKingPos.X && squarePosition.Y == enemyKingPos.Y)
                            {
                                if (_chessboard.IsCheck)
                                    _chessboard.IsDoubleCheck = true;

                                _chessboard.IsCheck = true;
                                v.IsChecking = true;

                                _chessboard.CheckingVector = v;
                            }
                            Move move = new(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                            v.Moves.Add(move);

                        }
                        Vectors.Add(v);
                    }
                }
            }
        }
    }
}
