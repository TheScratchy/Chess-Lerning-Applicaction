
using System;
using System.Linq;

namespace Chess_Learning_Application
{
    partial class Chessboard
    {
        public class Pawn : Figure
        {
            public Pawn(Chessboard chessboard, int x, int y, FigureColor color, int numberOfMoves = 0) : base(chessboard, x, y, color, numberOfMoves)
            {
                AlgebraicNotFirstLetter = "";
                _type = FigureType.Pawn;
            }

            public override void SearchForPseudoLegalMoves()
            {
                _pseudoLegalMoves.Clear();
                // forward
                int offsetY = 1 - (2 * (int)this.Color); // 1 for white, -1 for black
                if (pos.Y + offsetY >= 0 && pos.Y + offsetY < 8)
                {
                    if (_chessboard.square[pos.X, pos.Y + offsetY].Piece is null)
                    {
                        Move move = new(pos.X, pos.Y, pos.X, pos.Y + offsetY);

                        if ((pos.Y == 1 && Color == FigureColor.Black) || (pos.Y == 6 && Color == FigureColor.White))
                        {

                            move.WasPromotion = true;
                            move.PromoteTo = FigureType.Queen;
                            _pseudoLegalMoves.Add(move);

                            // Off to optimalize

                            //move.PromoteTo = FigureType.Knight;
                            //_pseudoLegalMoves.Add(move);
                            //move.PromoteTo = FigureType.Bishop;
                            //_pseudoLegalMoves.Add(move);
                            //move.PromoteTo = FigureType.Rook;
                            //_pseudoLegalMoves.Add(move);
                            move.PromoteTo = FigureType.Pawn;

                        } // promotion

                        else _pseudoLegalMoves.Add(move);

                        if ((pos.Y == 1 && Color == FigureColor.White) || (pos.Y == 6 && Color == FigureColor.Black))
                        {
                            if (_chessboard.square[pos.X, pos.Y + (offsetY * 2)].Piece is null)
                            {
                                Move bigMove = new(pos.X, pos.Y, pos.X, pos.Y + (offsetY * 2));
                                _pseudoLegalMoves.Add(bigMove);
                            }
                        } // two squares move
                    }


                    // capturing
                    bool isEPAlreadyAdded = false;
                    for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                    {
                        if (pos.X + offsetX >= 0 && pos.X + offsetX < 8)
                        {
                            if (_chessboard.square[pos.X + offsetX, pos.Y + offsetY].Piece is not null && _chessboard.square[pos.X + offsetX, pos.Y + offsetY].Piece?.Color != Color)
                            {
                                Move move = new(pos.X, pos.Y, pos.X + offsetX, pos.Y + offsetY);

                                if ((pos.Y == 1 && Color == FigureColor.Black) || (pos.Y == 6 && Color == FigureColor.White))
                                {
                                    move.WasPromotion = true;

                                    move.PromoteTo = FigureType.Queen;
                                    _pseudoLegalMoves.Add(move);

                                    // off to optimalize

                                    //move.PromoteTo = FigureType.Knight;
                                    //_pseudoLegalMoves.Add(move);
                                    //move.PromoteTo = FigureType.Bishop;
                                    //_pseudoLegalMoves.Add(move);
                                    //move.PromoteTo = FigureType.Rook;
                                    //_pseudoLegalMoves.Add(move);

                                    move.PromoteTo = FigureType.Pawn;
                                } // promotion

                                else _pseudoLegalMoves.Add(move);
                            }
                            // en passant
                            if (_chessboard.square[pos.X + offsetX, pos.Y + offsetY].Piece is null && _chessboard.MovesHistory.Any())
                            {
                                Move lastMove = _chessboard.MovesHistory.Last();
                                if (_chessboard.square[lastMove.DX, lastMove.DY].Piece is not null && _chessboard.square[lastMove.DX, lastMove.DY].Piece?.Type == FigureType.Pawn)
                                {
                                    if (Math.Abs(lastMove.DY - lastMove.SY) == 2 && Math.Abs(lastMove.DX - pos.X) == 1 && lastMove.DY == pos.Y)
                                    {
                                        Move enPassant = new(pos.X, pos.Y, lastMove.SX, pos.Y + offsetY);
                                        if (isEPAlreadyAdded)
                                            continue;

                                        _pseudoLegalMoves.Add(enPassant);
                                        isEPAlreadyAdded = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public override void Move(Move m)
            {

                m.SetAlgebraicNotation(this._chessboard);
                NumberOfMoves++;
                if (_chessboard.square[m.DX, m.DY].Piece is null && m.SX != m.DX)
                {
                    Move lastMove = _chessboard.MovesHistory.Last();
                    _chessboard._capturedFigures.Add(_chessboard.square[lastMove.DX, lastMove.DY].Piece!);
                    _chessboard.Figures.Remove(_chessboard.square[lastMove.DX, lastMove.DY].Piece!);
                    _chessboard.square[lastMove.DX, lastMove.DY].Piece = null;
                    m.WasCapturing = true;
                }
                if (_chessboard.square[m.DX, m.DY].Piece is not null)
                {
                    _chessboard._capturedFigures.Add(_chessboard.square[m.DX, m.DY].Piece!);
                    _chessboard.Figures.Remove(_chessboard.square[m.DX, m.DY].Piece!);
                    m.WasCapturing = true;
                }



                _chessboard.square[m.DX, m.DY].Piece = this;
                pos.X = m.DX;
                pos.Y = m.DY;
                _chessboard.PositionsOfFigures.Remove(pos);
                _chessboard.PositionsOfFigures.Add(pos);
                _chessboard.square[m.SX, m.SY].Piece = null;

                if ((Color == FigureColor.White && this.pos.Y == 7) || (Color == FigureColor.Black && this.pos.Y == 0))
                {
                    m.WasPromotion = true;
                    if (m.PromoteTo == FigureType.Pawn) // promotion to be chosen by the player
                    {
                        _chessboard.PawnPromotionEvent?.Invoke(this, this.pos);
                        return;
                    }
                    else
                    {
                        // promotion chosen by AI
                        _chessboard.Promote(this.pos, m.PromoteTo.ToString());
                    }

                    //        _chessboard.Figures.Remove(this);
                    //        m.WasPromotion = true;
                    //        _chessboard._capturedFigures.Add(this);
                    //        _chessboard.square[this.pos.X, this.pos.Y].Piece = null;

                    //HelperCommands.FigureToBeCreated newFig = new(m.PromoteTo, this.Color, this.pos);
                    //HelperCommands.CreateFigure(newFig, _chessboard);
                }
                _chessboard.MovesHistory.Add(m);
            }

            public override void SetVectorOfMoves()
            {
                Vectors.Clear();

                Position enemyKingPos = Color == FigureColor.White ? _chessboard.BlackKingPosition : _chessboard.WhiteKingPosition;

                int offsetY = 1 - (2 * (int)this.Color); // 1 for white, -1 for black
                for (int offsetX = -1; offsetX <= 1; offsetX += 2)
                {
                    VectorOfMoves v = new();
                    v.Moves.Add(new Move(pos.X, pos.Y, pos.X, pos.Y));

                    Position squarePosition = pos;
                    squarePosition.X += offsetX;
                    squarePosition.Y += offsetY;

                    if (squarePosition.X >= 0 && squarePosition.Y >= 0 && squarePosition.X < 8 && squarePosition.Y < 8)
                    {
                        _chessboard.AttackedSquares.Add(squarePosition);
                        if (squarePosition.X == enemyKingPos.X && squarePosition.Y == enemyKingPos.Y)
                        {
                            if (_chessboard.IsCheck)
                                _chessboard.IsDoubleCheck = true;
                            _chessboard.CheckingVector = v;
                            v.IsChecking = true;

                            _chessboard.IsCheck = true;
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
