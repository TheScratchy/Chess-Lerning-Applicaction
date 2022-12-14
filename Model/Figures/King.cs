
namespace Chess_Learning_Application
{
    partial class Chessboard
    {
        public class King : Figure
        {
            public King(Chessboard chessboard, int x, int y, FigureColor color, int numberOfMoves = 0) : base(chessboard, x, y, color, numberOfMoves)
            {
                _type = FigureType.King;
                AlgebraicNotFirstLetter = "K";
                if (color == FigureColor.White) { _chessboard.WhiteKingPosition = pos; }
                else { _chessboard.BlackKingPosition = pos; }
            }

            public override void Move(Move m)
            {

                m.SetAlgebraicNotation(this._chessboard);
                NumberOfMoves++;

                if (_chessboard.square[m.DX, m.DY].Piece is not null)
                {
                    _chessboard._capturedFigures.Add(_chessboard.square[m.DX, m.DY].Piece!);
                    _chessboard.Figures.Remove(_chessboard.square[m.DX, m.DY].Piece!);
                    m.WasCapturing = true;
                }

                

                pos.X = m.DX;
                pos.Y = m.DY;

                _chessboard.square[m.DX, m.DY].Piece = this;
                _chessboard.square[m.SX, m.SY].Piece = null;

                _chessboard.PositionsOfFigures.Remove(pos);
                _chessboard.PositionsOfFigures.Add(pos);

                if (Color == FigureColor.White) { _chessboard.WhiteKingPosition = pos; }
                else { _chessboard.BlackKingPosition = pos; }

                if (m.DX - m.SX == 2)
                {
                    _chessboard.square[m.SX + 1, m.SY].Piece = _chessboard.square[m.SX + 3, m.DY].Piece;
                    _chessboard.square[m.SX + 3, m.DY].Piece = null;
                    _chessboard.square[m.SX + 1, m.SY].Piece!.pos = new(m.SX + 1, m.SY);
                    m.WasCastling = true;
                }
                if (m.DX - m.SX == -2)
                {
                    _chessboard.square[m.SX - 1, m.SY].Piece = _chessboard.square[m.SX - 4, m.DY].Piece;
                    _chessboard.square[m.SX - 4, m.DY].Piece = null;
                    _chessboard.square[m.SX - 1, m.SY].Piece!.pos = new(m.SX - 1, m.SY);
                    m.WasCastling = true;
                }
                _chessboard.MovesHistory.Add(m);
            }

            public override void SetVectorOfMoves()
            {
                Vectors.Clear();
                Position enemyKingPos = Color == FigureColor.White ? _chessboard.BlackKingPosition : _chessboard.WhiteKingPosition;
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        if (offsetX == 0 && offsetY == 0) // Do not check the position of the king
                            continue;

                        VectorOfMoves v = new();
                        Position squarePosition = pos; // Position of a square we are examining
                        v.Moves.Add(new Move(pos.X, pos.Y, pos.X, pos.Y));

                        squarePosition.X += offsetX;
                        squarePosition.Y += offsetY;

                        if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                        {
                            _chessboard.AttackedSquares.Add(squarePosition);
                            Move move = new(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                            v.Moves.Add(move);

                        }
                        Vectors.Add(v);

                    }
                }
            }

            public override void SetLegalMoves()
            {
                SearchForPseudoLegalMoves();
                _legalMoves.Clear();

                #region Castling
                if (NumberOfMoves == 0 && !(IsCheck()) && pos.X == 4 && (pos.Y == 0 || pos.Y == 7))
                {
                    // short castling

                    if (_chessboard.square[pos.X + 1, pos.Y].Piece is null && _chessboard.square[pos.X + 2, pos.Y].Piece is null)
                    {
                        if (_chessboard.square[pos.X + 3, pos.Y].Piece?.NumberOfMoves == 0 && _chessboard.square[pos.X + 3, pos.Y].Piece?.Type == FigureType.Rook && _chessboard.square[pos.X + 3, pos.Y].Piece?.Color == Color)
                        {
                            Move move = new(pos.X, pos.Y, pos.X + 1, pos.Y);

                            pos.X = move.DX;
                            pos.Y = move.DY;

                            _chessboard.square[move.DX, move.DY].Piece = this;
                            _chessboard.square[move.SX, move.SY].Piece = null;

                            if (Color == FigureColor.White) { _chessboard.WhiteKingPosition = pos; }
                            else { _chessboard.BlackKingPosition = pos; }

                            if (!IsCheck())
                            {
                                Move castling = new(4, pos.Y, 6, pos.Y);
                                castling.WasCastling = true;
                                _pseudoLegalMoves.Add(castling);

                            }

                            pos.X = move.SX;
                            pos.Y = move.SY;
                            _chessboard.square[move.SX, move.SY].Piece = this;
                            _chessboard.square[move.DX, move.DY].Piece = null;
                        }
                    }

                    // long castling
                    if (_chessboard.square[pos.X - 1, pos.Y].Piece is null && _chessboard.square[pos.X - 2, pos.Y].Piece is null && _chessboard.square[pos.X - 3, pos.Y].Piece is null)
                    {
                        if (_chessboard.square[pos.X - 4, pos.Y].Piece?.NumberOfMoves == 0 && _chessboard.square[pos.X - 4, pos.Y].Piece?.Type == FigureType.Rook && _chessboard.square[pos.X - 4, pos.Y].Piece?.Color == Color)
                        {
                            Move move = new(pos.X, pos.Y, pos.X - 1, pos.Y);
                            _chessboard.square[move.DX, move.DY].Piece = this; // this instead of this.Move(m);
                            pos.X = move.DX;
                            pos.Y = move.DY;
                            _chessboard.square[move.SX, move.SY].Piece = null;

                            if (Color == FigureColor.White) { _chessboard.WhiteKingPosition = pos; }
                            else { _chessboard.BlackKingPosition = pos; }

                            if (!IsCheck())
                            {
                                Move castling = new(4, pos.Y, 2, pos.Y);
                                castling.WasCastling = true;
                                _pseudoLegalMoves.Add(castling);
                            }

                            pos.X = move.SX;
                            pos.Y = move.SY;
                            _chessboard.square[move.SX, move.SY].Piece = this;
                            _chessboard.square[move.DX, move.DY].Piece = null;
                        }
                    }

                }
                #endregion

                if (Color == FigureColor.White) { _chessboard.WhiteKingPosition = pos; }
                else { _chessboard.BlackKingPosition = pos; }

                foreach (Move m in _pseudoLegalMoves)
                {
                    if (!_chessboard.AttackedSquares.Contains(new Position(m.DX, m.DY)))
                        _legalMoves.Add(m);
                }
            }


            public override void SearchForPseudoLegalMoves()
            {
                _pseudoLegalMoves.Clear();
                // regular moves
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        if (offsetX != 0 || offsetY != 0) // Do not check the position of the king
                        {
                            Position squarePosition = pos;
                            squarePosition.X += offsetX;
                            squarePosition.Y += offsetY;
                            if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                            {
                                if (_chessboard.square[squarePosition.X, squarePosition.Y].Piece?.Color != Color)
                                {
                                    Move move = new Move(pos.X, pos.Y, squarePosition.X, squarePosition.Y);
                                    _pseudoLegalMoves.Add(move);
                                }
                            }
                        }
                    }
                }


            }
        }
    }
}
