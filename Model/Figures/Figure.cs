
using System.Collections.Generic;

namespace Chess_Learning_Application
{
partial class Chessboard
    {
        public abstract class Figure
        {
            protected Chessboard _chessboard;
            public int NumberOfMoves { get; set; }
            protected List<Move> _pseudoLegalMoves = new();
            public List<Move> PseudoLegalMoves { get { return _pseudoLegalMoves; } }
            protected List<Move> _legalMoves = new();
            public List<Move> LegalMoves { get { return _legalMoves; } }
            public List<VectorOfMoves> Vectors { get; protected set; } = new();
            public Position pos = new Position();
            protected FigureType _type;
            public bool IsPinned { get; set; } = false;
            public Figure? IsPinnedBy { get; set; } = null;
            public FigureType Type { get { return _type; } }
            protected FigureColor _color;
            public FigureColor Color { get { return _color; } }
            public string? AlgebraicNotFirstLetter { get; protected set; }
            public Figure(Chessboard chessboard, int x, int y, FigureColor color, int numberOfMoves = 0)
            {
                _chessboard = chessboard;
                pos.X = x;
                pos.Y = y;
                _color = color;
                _chessboard.square[pos.X, pos.Y].Piece = this;
                _chessboard.Figures.Add(this);
                NumberOfMoves = numberOfMoves;
            }
            public virtual void Move(Move m)
            {
                m.SetAlgebraicNotation(this._chessboard);
                NumberOfMoves++;
                if (_chessboard.square[m.DX, m.DY].Piece is not null)
                {
                    _chessboard._capturedFigures.Add(_chessboard.square[m.DX, m.DY].Piece!);
                    _chessboard.Figures.Remove(_chessboard.square[m.DX, m.DY].Piece!);
                    m.WasCapturing = true;
                }
                _chessboard.MovesHistory.Add(m);
                _chessboard.square[m.DX, m.DY].Piece = this;
                pos.X = m.DX;
                pos.Y = m.DY;
                _chessboard.square[m.SX, m.SY].Piece = null;

                _chessboard.PositionsOfFigures.Remove(pos);
                _chessboard.PositionsOfFigures.Add(pos);

            }

            public virtual void SetLegalMoves()
            {
                SearchForPseudoLegalMoves();
                _legalMoves.Clear();

                if (_chessboard.IsDoubleCheck) // Only King can move
                    return;

                if (IsPinned)
                {
                    if (_chessboard.IsCheck)
                        return; // no move here

                    VectorOfMoves pinningVector = new();
                    foreach (var pinningVectors in IsPinnedBy!.Vectors)
                    {
                        if (pinningVectors.Pinns)
                            pinningVector = pinningVectors;
                    }

                    foreach (Move m in _pseudoLegalMoves)
                    {
                        foreach (var vm in pinningVector.Moves)
                        {
                            if (m.DX == vm.DX && m.DY == vm.DY)
                                _legalMoves.Add(m);
                        }
                    }
                    return;
                }

                if (_chessboard.IsCheck)
                {
                    VectorOfMoves CheckingVector = _chessboard.CheckingVector;

                    foreach (Move m in _pseudoLegalMoves)
                    {
                        foreach (var vm in CheckingVector.Moves)
                        {
                            if (m.DX == vm.DX && m.DY == vm.DY)
                                _legalMoves.Add(m);
                        }
                    }
                    return;
                }

                _legalMoves.AddRange(_pseudoLegalMoves);
            }
            public abstract void SearchForPseudoLegalMoves();

            public abstract void SetVectorOfMoves();
            protected bool IsCheck()
            {
                foreach (var piece in _chessboard.Figures)
                {
                    //var piece = _chessboard.square[sq.X, sq.Y].Piece!;
                    if (piece.Color != this.Color)
                    {
                        piece.SearchForPseudoLegalMoves();
                        foreach (Move move in piece._pseudoLegalMoves)
                        {
                            if (this.Color == FigureColor.White)
                            {
                                if (move.DX == _chessboard.WhiteKingPosition.X && move.DY == _chessboard.WhiteKingPosition.Y)
                                { return true; }
                            }
                            else
                            {
                                if (move.DX == _chessboard.BlackKingPosition.X && move.DY == _chessboard.BlackKingPosition.Y)
                                { return true; }
                            }
                        }
                    }
                }
                return false;

                //    foreach (


                //        // in case it doesnt work - create a List<Move> with all square being attacked by enemy pieces, add Pawns and search for King
                //        Position _kingPosition;
                //    if (this.Color == FigureColor.White)  { _kingPosition = new Position(Board.WhiteKingPosition.X, Board.WhiteKingPosition.Y); }
                //    else { _kingPosition = new Position(Board.BlackKingPosition.X, Board.BlackKingPosition.Y); }

                //    Bishop bishop = new(Board, _kingPosition.X, _kingPosition.Y, this.Color, true);
                //    bishop.SearchForPseudoLegalMoves();
                //    foreach(Move m in bishop.LegalMoves)
                //    {
                //        if (bishop.Board.square[m.DX, m.DY].Piece?.Type == FigureType.Bishop || bishop.Board.square[m.DX, m.DY].Piece?.Type == FigureType.Queen)
                //        {
                //            if (bishop.Board.square[m.DX, m.DY].Piece?.Color != Color)
                //            {
                //                return true;
                //            }
                //        }
                //    }

                //    Rook rook = new(Board, _kingPosition.X, _kingPosition.Y, this.Color, true);
                //    rook.SearchForPseudoLegalMoves();
                //    foreach (Move m in rook.LegalMoves)
                //    {
                //        if (rook.Board.square[m.DX, m.DY].Piece?.Type == FigureType.Rook || rook.Board.square[m.DX, m.DY].Piece?.Type == FigureType.Queen)
                //        {
                //            if (rook.Board.square[m.DX, m.DY].Piece?.Color != Color)
                //            {
                //                return true;
                //            }
                //        }
                //    }

                //    Knight knight = new(Board, _kingPosition.X, _kingPosition.Y, this.Color, true);
                //    knight.SearchForPseudoLegalMoves();
                //    foreach (Move m in knight.LegalMoves)
                //    {
                //        if (knight.Board.square[m.DX, m.DY].Piece?.Type == FigureType.Knight)
                //        {
                //            if (knight.Board.square[m.DX, m.DY].Piece?.Color != Color)
                //            {
                //                return true;
                //            }
                //        }
                //    }


                //    // King
                //    foreach (Move m in Board.square[_kingPosition.X, _kingPosition.Y].Piece!.LegalMoves)
                //        {
                //            if (knight.Board.square[m.DX, m.DY].Piece?.Type == FigureType.King)
                //            {
                //                return true;
                //            }
                //        }

                //    // Pawn
                //    int temp = 1 + (-2 * (int)Color); // 1 for white, -1 for black
                //    if (_kingPosition.Y >= 0 && _kingPosition.Y < 7 && _kingPosition.Y >= 0 - (temp * 2) && _kingPosition.Y < 8 - (temp * 2))
                //    {
                //        for (int i = -1; i <= 1; i += 2)
                //        {
                //            if (_kingPosition.X + i >= 0 && _kingPosition.X + i < 8)
                //            {

                //                if (Board.square[_kingPosition.X + i, _kingPosition.Y + temp].Piece?.Type == FigureType.Pawn && Board.square[_kingPosition.X + i, _kingPosition.Y + temp].Piece?.Color != Color)
                //                {
                //                    return true;
                //                }
                //            }
                //        }
                //    }

                //    return false;
                //}
            }
        }
    }
}
