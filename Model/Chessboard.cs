using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chess_Learning_Application
{
    /// <summary>
    /// Chessboard itself. Contains figures, squares, moves.
    /// </summary>
    public partial class Chessboard
    {
        public Square[,] square = new Square[8, 8];

        public ObservableCollection<Move> MovesHistory = new();

        public List<Figure> Figures = new List<Figure>();

        private List<Figure> _capturedFigures = new();

        public HashSet<Position> PositionsOfFigures = new(); // no duplicates allowed

        public Position WhiteKingPosition = new(-1,-1);

        public Position BlackKingPosition = new(-1,-1);

        public bool IsCheck { get; private set; } = false;
        public bool IsDoubleCheck { get; private set; } = false;
        public List<Position> AttackedSquares { get; private set; } = new();

        public VectorOfMoves CheckingVector { get; private set; }

        public static readonly string StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public static readonly string TestingFEN = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
        public static readonly string TestingPerformanceFEN = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
        public static readonly string TestingCheck = "1r2k1nr/p1pp1pb1/B3p1p1/3PN3/4P3/q1P5/P1P2PQP/K2R3R b kq - 0 1";
        public static readonly string TestingCheckTwoRooks = "7r/8/7k/8/8/4K3/8/8 w - - 0 1";
        public static readonly string TestingCheckPromotion = "8/3K4/4P3/8/8/8/6k1/7q w - - 0 1";
        public static readonly string Test1 = "r1b1r3/ppppqkpQ/2n1p3/8/2PP4/5N2/P2B1PPP/1B3RK w - - 0 1";

        #region Available Castlings
        private bool _whiteShortCastling = true;
        private bool _blackShortCastling = true;
        private bool _whiteLongCastling = true;
        private bool _blackLongCastling = true;
        #endregion

        public FigureColor Turn { get; set; } = FigureColor.White;

        public event EventHandler<Move>? MoveWasPerformedEvent;
        public event EventHandler<Position>? PawnPromotionEvent;

        public Chessboard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    FigureColor color = (FigureColor)((i + j + 1) % 2);
                    square[i, j] = new Square(i, j, color);
                }
            }

        }

        public void MoveFigure(Move move)
        {
            if ((square[move.SX, move.SY].Piece?.Type == Chessboard.FigureType.Pawn && (move.DY == 7 || move.DY == 0)))
            {
                move.WasPromotion = true;
            }
            Turn = Turn == FigureColor.White ? FigureColor.Black : FigureColor.White;
            square[move.SX, move.SY].Piece?.Move(move);
            if (move.WasPromotion == false)
            {
                // if it is not a promotion, since the promotion itself handles it
                MoveWasPerformedEvent?.Invoke(this, move);
            }
        }

        public void SetEnemyAttacks()
        {
            foreach (var piece in Figures)
            {
                if (piece.Color != Turn)
                    piece.SetVectorOfMoves();
            }
        }

        public void SetLegalMoves()
        {
            foreach (var piece in Figures)
            {
                if (piece.Color == Turn)
                    piece.SetLegalMoves();
            }
        }

        public void PerformTurn()
        {
            SetDefaultParameters();

            SetEnemyAttacks();

            SetLegalMoves();
        }

        private void SetDefaultParameters()
        {
            
            AttackedSquares.Clear();
            foreach (var piece in Figures)
            {
                piece.IsPinned = false;
                piece.IsPinnedBy = null;
            }
            IsCheck = false;
            IsDoubleCheck = false;
            CheckingVector = new();
        }

        public List<Position> GetAttackedSquares()
        {
            HashSet<Position> attackedSquares = new();

            foreach (var piece in Figures)
            {
                //var piece = square[pos.X, pos.Y].Piece!;

                if (piece.Color == Turn)
                    continue;

                piece.SearchForPseudoLegalMoves();

                foreach (Move m in piece.PseudoLegalMoves)
                {
                    attackedSquares.Add(new Position(m.DX, m.DY));
                }
            }

            return attackedSquares.ToList();
        }

        public List<Position> GetAttackedSquaresByPawns()
        {
            HashSet<Position> attackedSquares = new();

            foreach (var piece in Figures)
            {
                //var piece = square[pos.X, pos.Y].Piece!;

                if (piece.Color == Turn || piece.Type != FigureType.Pawn)
                    continue;

                int offsetY = (int)(piece.Color) * (-2) + 1; // 1 for white, -1 for blacks
                for (int offsetX = -1; offsetX <=1; offsetX+= 2)
                {
                    if (offsetX >= 0 && offsetX < 8) // offsetY can be skipped since it cannot be on squares that may danger the program
                        attackedSquares.Add(new Position(piece.pos.X + offsetX, piece.pos.Y));
                }
            }

            return attackedSquares.ToList();
        }

        public void Promote(Position pos, string promoteToFigure)
        {
            FigureColor color = square[pos.X, pos.Y].Piece!.Color;
            Figures.Remove(square[pos.X, pos.Y].Piece!);
            _capturedFigures.Add(square[pos.X, pos.Y].Piece!);
            square[pos.X, pos.Y].Piece = null;

            switch (promoteToFigure)
            {
                case "Queen":
                    {
                        Queen newQueen = new Queen(this, pos.X, pos.Y, color);
                        //Figures.Add(newQueen);
                        square[pos.X, pos.Y].Piece = newQueen;
                        break;
                    }
                case "Rook":
                    {
                        Rook newRook = new Rook(this, pos.X, pos.Y, color);
                        //Figures.Add(newQueen);
                        square[pos.X, pos.Y].Piece = newRook;
                        break;
                    }
                case "Bishop":
                    {
                        Bishop newBishop = new Bishop(this, pos.X, pos.Y, color);
                        //Figures.Add(newQueen);
                        square[pos.X, pos.Y].Piece = newBishop;
                        break;
                    }
                case "Knight":
                    {
                        Knight newKnight = new Knight(this, pos.X, pos.Y, color);
                        //Figures.Add(newQueen);
                        square[pos.X, pos.Y].Piece = newKnight;
                        break;
                    }
            }
                MoveWasPerformedEvent?.Invoke(this, new Move(pos.X, pos.Y, pos.X, pos.Y));
        }

        public void Undo()
        {
            if(MovesHistory.Last().WasPromotion)
            {
                //Figures.Remove(square[MovesHistory.Last().DX, MovesHistory.Last().DY].Piece!);
                Figures.Remove(square[MovesHistory.Last().DX, MovesHistory.Last().DY].Piece!);
                //Figures.RemoveAt(Figures.Count - 1);


                Figures.Add(_capturedFigures.Last());

                square[MovesHistory.Last().DX, MovesHistory.Last().DY].Piece = null; //  this should delete the whole promoted Piece 
                square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece = _capturedFigures.Last();

                _capturedFigures.RemoveAt(_capturedFigures.Count - 1);
            }
            else
            {
                square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece = square[MovesHistory.Last().DX, MovesHistory.Last().DY].Piece!;
                square[MovesHistory.Last().DX, MovesHistory.Last().DY].Piece = null;
            }

            square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece!.pos.Y = MovesHistory.Last().SY;
            square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece!.pos.X = MovesHistory.Last().SX;

            if (square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece?.Type == FigureType.King)
            {
                if (square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece?.Color == FigureColor.Black)
                    BlackKingPosition = square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece!.pos;
                else WhiteKingPosition = square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece!.pos;
            }

            if(MovesHistory.Last().WasCastling)
            {
                if(MovesHistory.Last().DX == 2)
                {
                    square[0, MovesHistory.Last().DY].Piece = square[3, MovesHistory.Last().DY].Piece;
                    square[3, MovesHistory.Last().DY].Piece = null;

                    square[0, MovesHistory.Last().DY].Piece!.pos.X = 0;
                    square[0, MovesHistory.Last().DY].Piece!.pos.Y = MovesHistory.Last().SY;
                }
                if (MovesHistory.Last().DX == 6)
                {
                    square[7, MovesHistory.Last().DY].Piece = square[5, MovesHistory.Last().DY].Piece;
                    square[5, MovesHistory.Last().DY].Piece = null;

                    square[7, MovesHistory.Last().DY].Piece!.pos.X = 7;
                    square[7, MovesHistory.Last().DY].Piece!.pos.Y = MovesHistory.Last().SY;
                }
            }

            square[MovesHistory.Last().SX, MovesHistory.Last().SY].Piece!.NumberOfMoves--;

            PositionsOfFigures.Remove(new Position(MovesHistory.Last().DX, MovesHistory.Last().DY));
            PositionsOfFigures.Add(new Position(MovesHistory.Last().SX, MovesHistory.Last().SY));

            if (MovesHistory.Last().WasCapturing)
            {
                Figures.Add(_capturedFigures.Last());

                square[_capturedFigures.Last().pos.X, _capturedFigures.Last().pos.Y].Piece = Figures.Last();
                _capturedFigures.RemoveAt(_capturedFigures.Count - 1);
                PositionsOfFigures.Add(new Position(MovesHistory.Last().DX, MovesHistory.Last().DY));
            }

            MovesHistory.RemoveAt(MovesHistory.Count - 1);

            Turn = Turn == FigureColor.White ? FigureColor.Black : FigureColor.White;
        }

        public void SetCastlings()
        {
            if (_whiteLongCastling)
            {
                if (square[0, 0].Piece != null)
                {
                    if (!(WhiteKingPosition.X == 4 && WhiteKingPosition.Y == 0 && square[0, 0].Piece?.Color == FigureColor.White && square[0, 0].Piece?.Type == FigureType.Rook))
                        _whiteLongCastling = false;
                    else if (!(square[4, 0].Piece!.NumberOfMoves == 0 && square[0, 0].Piece!.NumberOfMoves == 0))
                        _whiteLongCastling = false;
                }
                else _whiteLongCastling = false;
            }
            if (_whiteShortCastling)
            {
                if (square[0, 7].Piece is not null)
                {
                    if (!(WhiteKingPosition.X == 4 && WhiteKingPosition.Y == 0 && square[7, 0].Piece?.Color == FigureColor.White && square[7, 0].Piece?.Type == FigureType.Rook))
                        _whiteShortCastling = false;
                    else if (!(square[4, 0].Piece!.NumberOfMoves == 0 && square[7, 0].Piece!.NumberOfMoves == 0))
                        _whiteShortCastling = false;
                }
                else _whiteShortCastling = false;
            }
            if (_blackLongCastling)
            {
                if (square[0, 7].Piece != null)
                {
                    if (!(BlackKingPosition.X == 4 && BlackKingPosition.Y == 7 && square[0, 7].Piece?.Color == FigureColor.Black && square[0, 7].Piece?.Type == FigureType.Rook))
                        _blackLongCastling = false;
                    else if (!(square[4, 7].Piece?.NumberOfMoves == 0 && square[0, 7].Piece?.NumberOfMoves == 0))
                        _blackLongCastling = false;
                } else _blackLongCastling = false;
            }
            if (_blackShortCastling)
            {
                if (square[0, 0].Piece != null)
                {
                    if (!(BlackKingPosition.X == 4 && BlackKingPosition.Y == 7 && square[7, 7].Piece?.Color == FigureColor.Black && square[7, 7].Piece?.Type == FigureType.Rook))
                        _blackShortCastling = false;
                    else if (!(square[4, 7].Piece?.NumberOfMoves == 0 && square[7, 7].Piece?.NumberOfMoves == 0))
                        _blackShortCastling = false;
                } else _blackShortCastling= false;
            }
        }

        #region copying and setting Chessbaord using FEN
        
        /// <summary>
        /// Setting chessboard using FEN notation (AI module)
        /// </summary>
        /// <param name="FEN"> FEN notation</param>
        public void SetChessbordUsingFEN(string FEN)
        {
            var FEN2FigureType = new Dictionary<char, FigureType>()
            {
                ['k'] = FigureType.King,
                ['q'] = FigureType.Queen,
                ['r'] = FigureType.Rook,
                ['b'] = FigureType.Bishop,
                ['n'] = FigureType.Knight,
                ['p'] = FigureType.Pawn
            };

            string board = FEN.Split(" ")[0];
            string turn = FEN.Split(" ")[1];
            string castlings = FEN.Split(" ")[2];
            string enPassant = FEN.Split(" ")[3];

            #region Castlings

            int leftWhiteRookMoves = 0;
            int rightWhiteRookMoves = 0;
            int leftBlackRookMoves = 0;
            int rightBlackRookMoves = 0;
            if (!castlings.Contains('K'))
                leftWhiteRookMoves = 1;
            if (!castlings.Contains('k'))
                leftBlackRookMoves = 1;
            if (!castlings.Contains('Q'))
                rightWhiteRookMoves = 1;
            if (!castlings.Contains('q'))
                rightBlackRookMoves = 1;

           

            #endregion

            #region board
            int file = -1;
            int rank = 7;
            
            foreach (char c in board)
            {
                if (c == '/')
                {
                    file = -1;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(c))
                        file += (int)char.GetNumericValue(c);
                    
                    else
                    {
                        file++;
                        FigureColor color = char.IsUpper(c) ? FigureColor.White : FigureColor.Black;
                        FigureType type = FEN2FigureType[char.ToLower(c)];
                        Position pos = new(file, rank);

                        HelperCommands.FigureToBeCreated fig = new(type, color, pos);

                        if (type == FigureType.Rook && color == FigureColor.White && pos.X == 0)
                        {
                            new Chessboard.Rook(this, fig.Position.X, fig.Position.Y, fig.Color, leftWhiteRookMoves);
                            continue;
                        }
                        if (type == FigureType.Rook && color == FigureColor.White && pos.X == 7)
                        {
                            new Chessboard.Rook(this, fig.Position.X, fig.Position.Y, fig.Color, rightWhiteRookMoves);
                            continue;
                        }
                        if (type == FigureType.Rook && color == FigureColor.Black && pos.X == 0)
                        {
                            new Chessboard.Rook(this, fig.Position.X, fig.Position.Y, fig.Color, leftBlackRookMoves);
                            continue;
                        }
                        if (type == FigureType.Rook && color == FigureColor.Black && pos.X == 7)
                        {
                            new Chessboard.Rook(this, fig.Position.X, fig.Position.Y, fig.Color, rightBlackRookMoves);
                            continue;
                        }

                        HelperCommands.CreateFigure(fig, this);
                    }
                }
            } // foreach c in board
            #endregion

            #region turn
            if (turn.Contains('w'))// turn
                this.Turn = FigureColor.White;
            else this.Turn = FigureColor.Black;
            #endregion

            #region enPassant
            if (enPassant == "-")
                return;

            int epRank = Int32.Parse(enPassant[1].ToString());
            int epFile = ((int)enPassant[0] % 32) - 1;
            int startingEPRank;
            if (epRank == 4)
                startingEPRank = 1;
            else startingEPRank = 6;

            MovesHistory.Add(new(epFile, startingEPRank, epFile, epRank - 1));
            #endregion


        } // SetChessboardUsingFEN()

        /// <summary>
        /// Get FEN notation from current chessboard state
        /// </summary>
        /// <returns></returns>
        public string GetFENNotation()
        {
            string FEN = "";
            var FigureType2FEN = new Dictionary<FigureType, char>()
            {
                [FigureType.King] = 'k',
                [FigureType.Queen] =  'q',
                [FigureType.Rook] = 'r',
                [FigureType.Bishop] = 'b',
                [FigureType.Knight] = 'n',
                [FigureType.Pawn] = 'p'
            };

            #region Chessboard itself
            for (int rank = 7; rank >=0; rank--)
            {
                int blankFiles = 0;
                for (int file = 0; file < 8; file++ )
                {
                    if (this.square[file, rank].Piece == null)
                    { blankFiles++;
                        if (file == 7 && blankFiles > 0)
                                FEN += blankFiles.ToString();
                        continue; 
                    }

                    bool IsBlack = (int)(this.square[file, rank].Piece!.Color) == 1;
                    if (blankFiles > 0)
                        FEN += blankFiles.ToString();

                    FEN += IsBlack ? FigureType2FEN[this.square[file, rank].Piece!.Type] : char.ToUpper(FigureType2FEN[this.square[file, rank].Piece!.Type]);
                    blankFiles = 0;
                }
                FEN +="/";
            }
            #endregion

            #region Turn
            if (this.Turn == FigureColor.White)
                FEN += " w ";
            else FEN += " b ";
            #endregion

            #region Castling
            SetCastlings();

            if (_whiteShortCastling)
                FEN += "K";
            if (_whiteLongCastling)
                FEN += "Q";
            if (_blackShortCastling)
                FEN += "k";
            if (_blackLongCastling)
                FEN += "q";

            if (!(_whiteLongCastling || _blackLongCastling || _whiteShortCastling || _blackShortCastling))
                FEN += "-";
            #endregion

            #region en passant
            if (MovesHistory.Any())
            {
                if (square[MovesHistory.Last().DX, MovesHistory.Last().DY].Piece?.Type == FigureType.Pawn && Math.Abs(MovesHistory.Last().SY - MovesHistory.Last().DY) == 2)
                    FEN += " " + HelperCommands.GetFile(MovesHistory.Last().DX) + (MovesHistory.Last().DY + 1).ToString() + " ";

                else FEN += " - ";
            }
            else FEN += " - ";
            #endregion

            int fullMoves = 0;
            int halfMoves = 0;

            foreach (Figure piece in Figures)
            {
                if (piece.Type == FigureType.Pawn)
                    halfMoves += piece.NumberOfMoves;

                else fullMoves += piece.NumberOfMoves;
            }

            FEN += halfMoves.ToString();
            FEN += " " + fullMoves.ToString(); 

            return FEN;
        }
        #endregion
    }
}
