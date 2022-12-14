using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chess_Learning_Application
{
    public partial class Engine
    {
        public static readonly Dictionary<Chessboard.FigureType, int> ValueOfFigure = new()
        {
            [Chessboard.FigureType.Pawn] = 9,
            [Chessboard.FigureType.Knight] = 29,
            [Chessboard.FigureType.Bishop] = 30,
            [Chessboard.FigureType.Rook] = 50,
            [Chessboard.FigureType.Queen] = 90,
            [Chessboard.FigureType.King] = 9999
        };

        #region Position Values
        public static readonly int[] PawnPositionValue = new int[]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, -1, -1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 2, 2, 2, 2, 1, 1,
            2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 3, 3, 3, 3, 2, 2,
            3, 3, 3, 3, 3, 3, 3, 3,
            4, 4, 4, 4, 4, 4, 4, 4,
        };

        public static readonly int[] KingEarlyPositionValue = new int[]
        {
            1, 2, 4, 0, 0, 0, 4, 2,
            -2, -2, -2, -2, -2, -2, -2, -2,
            -2, -2, -2, -2, -2, -2, -2, -2,
            -2, -2, -2, -2, -2, -2, -2, -2,
            -4, -4, -4, -4, -4, -4, -4, -4,
            -4, -4, -4, -4, -4, -4, -4, -4,
            -4, -4, -4, -4, -4, -4, -4, -4,
            -4, -4, -4, -4, -4, -4, -4, -4
        };

        public static readonly int[] BishopPositionValue = new int[]
        {
            0, -1, 0, -2, -2, 0, -1, 0,
            2, 3, 0, 1, 1, 0, 3, 2,
            1, 3, 1, 3, 2, 1, 3, 1,
            1, 0, 3, 0, 0, 3, 0, 1,
            0, 3, 0, 0, 0, 0, 3, 0,
            1, 0, 0, 0, 0, 0, 0, 1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -2, -2, -2, -2, -2, -2, -2, -2,
        };

        public static readonly int[] KnightPositionValue = new int[]
        {
            -4, 0, 0, 0, 0, 0, 0, -4,
            -1, 1, 1, 2, 2, 1, 1, -1,
            0, 1, 3, 2, 2, 3, 1, 0,
            -2, 1, 2, 4, 4, 2, 1, -2,
            -2, 1, 2, 4, 4, 2, 1, -2,
            -1, 1, 3, 2, 2, 3, 1, -1,
            -1, 1, 1, 1, 1, 1, 1, -1,
            -4, 0, 0, 0, 0, 0, 0, -4,
        };

        public static readonly int[] QueenPositionValue = new int[]
        {
            -1, -1, -1, 1, -1, -1, -1, -1,
            0, 0, 3, 3, 3, 0, 0, 0,
            0, 3, 0, 3, 0, 3, 0, 0,
            2, 0, 0, 2, 0, 0, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,


        };

        public static readonly int[] RookPositionValue = new int[]
        {
            3, 3, 3, 3, 3, 3, 3, 3,
            2, 2, 2, 2, 2, 2, 2, 2,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            4, 4, 4, 4, 4, 4, 4, 4,
            2, 2, 2, 2, 2, 2, 2, 2
        };

        #endregion

        public Book book;

        public Chessboard.FigureColor PlayAsColor { get; set; }
        public int Depth { get; set; } = 4;

        public Engine()
        {
            ValueForMoves = new();
            book = Book.LoadBookFromTxtFile();
        }

        public ConcurrentDictionary<Chessboard.Move, int> ValueForMoves;
        private List<Chessboard.Move> GenerateMoves(Chessboard chessboard)
        {
            List<Chessboard.Move> moves = new();
            #region setting moves
            foreach (Chessboard.Figure piece in chessboard.Figures)
            {
                if (piece.Color == chessboard.Turn)
                {
                    //piece.SetLegalMoves();
                    moves.AddRange(piece.LegalMoves);
                }
            }
            #endregion
            return moves;
        }

        #region Working TestGen
        //public int TestGeneratingMoves(int depth)
        //{
        //    if (depth == 0)
        //        return 1;

        //    int totalNumberOfMoves = 0;
        //    int partialNumberOfMoves = 0;
        //    List<Chessboard.Move> moves = GenerateMoves();

        //    foreach (var move in moves)
        //    {
        //        move.SetAlgebraicNotation(Board);
        //        Board.square[move.SX, move.SY].Piece!.Move(move);

        //        Board.Turn = Board.Turn == Chessboard.FigureColor.White ? Chessboard.FigureColor.Black : Chessboard.FigureColor.White;
        //        //totalNumberOfMoves += TestGeneratingMoves(depth - 1);

        //        partialNumberOfMoves += TestGeneratingMoves(depth - 1);

        //        if (depth == this.Depth)
        //        {
        //            //Thread searchForMovesThread = new(() => TestGeneratingMoves(depth - 1));
        //            //searchForMovesThread.Start();
        //            ValueForMoves.Add(move, partialNumberOfMoves);
        //            totalNumberOfMoves += partialNumberOfMoves;
        //            partialNumberOfMoves = 0;

        //        }
        //        Board.Undo();
        //    }

        //    //if (depth == this.Depth - 1)
        //    //{
        //    //    ValueForMoves.TryAdd()
        //    //}

        //    if (depth == this.Depth)
        //    { }
        //        return partialNumberOfMoves;
        //}
        #endregion

        /// <summary>
        /// Evaluates given position.
        /// </summary>
        /// <param name="chessboard">The board</param>
        /// <returns> Returns positive value if a color which has turn right now has adventage, otherwise negative.</returns>
        public int Evaluate(Chessboard chessboard)
        {
            int evaluation = Endgame(chessboard);
            Chessboard.Position whiteRookPos = new(-1, 0);
            Chessboard.Position blackRookPos = new (-1, 0);

            foreach (Chessboard.Figure piece in chessboard.Figures)
            {
                int value = ValueOfFigure[piece.Type];

                value += GetPositionValue(piece);

                #region checking if rooks are connected with each other
                if (piece.Type == Chessboard.FigureType.Rook)
                {
                    if (piece.Color == Chessboard.FigureColor.Black)
                    {
                        if (blackRookPos.X != -1)
                        {
                            if (blackRookPos.X == piece.pos.X || blackRookPos.Y == piece.pos.Y)
                                evaluation -= 1;
                        }
                        else blackRookPos = piece.pos;
                    }
                    else
                    {
                        if (whiteRookPos.X != -1)
                        {
                            if (whiteRookPos.X == piece.pos.X || whiteRookPos.Y == piece.pos.Y)
                                evaluation += 1;
                        }
                        else whiteRookPos = piece.pos;
                    }
                }
                #endregion

                if (piece.IsPinned)
                    value = (int)(value * 0.9);

                if (piece.Color == Chessboard.FigureColor.White)
                    evaluation += value;
                else
                    evaluation -= value;
            }
            return chessboard.Turn == Chessboard.FigureColor.White ? evaluation : -evaluation;
        }

        private int GetPositionValue(Chessboard.Figure piece)
        {
            
            int rank = piece.Color == Chessboard.FigureColor.White ? piece.pos.Y * 8 : (7 - piece.pos.Y) * 8;
            int position = piece.pos.X + rank;

            switch (piece.Type)
            {
                case Chessboard.FigureType.Pawn:
                    {
                        return  PawnPositionValue[position];
                    }
                case Chessboard.FigureType.Knight:
                    {
                        return  KnightPositionValue[position];
                    }
                case Chessboard.FigureType.Bishop:
                    {
                        return  BishopPositionValue[position];
                    }
                case Chessboard.FigureType.Rook:
                    {
                        return  RookPositionValue[position];
                    }
                case Chessboard.FigureType.Queen:
                    {
                        return  QueenPositionValue[position];
                    }
                case Chessboard.FigureType.King:
                    {
                        return  KingEarlyPositionValue[position];
                    }
                    default: break;
            }
            return 0;
        }

        public void SortMoves(ref List<Chessboard.Move> moves, Chessboard chessboard)
        {
            List<Chessboard.Position> squaresAttackedByPawns = chessboard.GetAttackedSquaresByPawns();
            Dictionary<Chessboard.Move, int> moveValuePairs = new();

            #region adding values to moves
            foreach (var move in moves)
            {
                int valueOfMove = 0;
                Chessboard.FigureType movingPieceType = chessboard.square[move.SX, move.SY].Piece!.Type;
          
                if (squaresAttackedByPawns.Contains(new Chessboard.Position(move.DX, move.DY)))
                    valueOfMove -= ValueOfFigure[movingPieceType];

                // returns the value of capture, i.e. enemy piece's value minus ours
                // 10 times to reduce the pawn - queen, which will give -80;
                if (chessboard.square[move.DX, move.DY].Piece != null)
                {
                    Chessboard.FigureType attackedPieceType = chessboard.square[move.DX, move.DY].Piece!.Type;
                    valueOfMove += 10 * ValueOfFigure[attackedPieceType] - ValueOfFigure[movingPieceType];
                }

                if (move.PromoteTo != Chessboard.FigureType.Pawn)
                    valueOfMove += ValueOfFigure[move.PromoteTo];

                moveValuePairs[move] = valueOfMove;
            }
            #endregion

            #region sorting the moves
            moves = moveValuePairs.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.ToList();
            #endregion
        }

        /// <summary>
        /// Responsible for moving enemy King to a corner and bringing friendly king
        /// to the center. Returns 0 if there is no endgame yet.
        /// </summary>
        private int Endgame(Chessboard chessboard)
        {
            // number of enemy figures, without pawns
            int enemyFigures = chessboard.Figures.Count(x => x.Type != Chessboard.FigureType.Pawn && x.Color == PlayAsColor);

            if (enemyFigures > 3)
                return 0;

            Chessboard.Position enemyKingPosition = this.PlayAsColor == Chessboard.FigureColor.White ?
                chessboard.BlackKingPosition : chessboard.WhiteKingPosition;

            Chessboard.Position friendlyKingPosition = this.PlayAsColor == Chessboard.FigureColor.Black ? 
                chessboard.BlackKingPosition : chessboard.WhiteKingPosition;

            int value = 0;
            value += enemyKingPosition.X < 4 ? Math.Abs(enemyKingPosition.X - 3) : Math.Abs(enemyKingPosition.X - 4);
            value += enemyKingPosition.Y < 4 ? Math.Abs(enemyKingPosition.Y - 3) : Math.Abs(enemyKingPosition.Y - 4);

            value += 7 - Math.Abs(friendlyKingPosition.X - enemyKingPosition.X);
            value += 7 - Math.Abs(friendlyKingPosition.Y - enemyKingPosition.Y);

            value *= 4 - enemyFigures;
            value = (int)(value / 2);
            return -value;
        }

        public async Task StartSearch(Chessboard chessboard)
        {
            int depth = 4;
            chessboard.PerformTurn();
            List<Chessboard.Move> moves = GenerateMoves(chessboard);
            SortMoves(ref moves, chessboard);

            int alpha = -999999;
            int beta = 999999;

            if (moves.Count == 0)
                return;

            await Parallel.ForEachAsync(moves, new ParallelOptions { MaxDegreeOfParallelism = 8 },
            async (move, ct) =>
            {
                
                Chessboard internalBoard = new();
                internalBoard.SetChessbordUsingFEN(chessboard.GetFENNotation());
                internalBoard.PerformTurn();
                move.SetAlgebraicNotation(internalBoard);

                internalBoard.square[move.SX, move.SY].Piece!.Move(move);
                internalBoard.Turn = internalBoard.Turn == Chessboard.FigureColor.White ? Chessboard.FigureColor.Black : Chessboard.FigureColor.White;

                await FindBestMovesAsync(depth - 1, -beta, -alpha, internalBoard, move);
            }
            );
        }

        public async Task<int> FindBestMovesAsync(int depth, int alpha, int beta, Chessboard chessboard, Chessboard.Move m = new())
        {
            if (depth == 0)
                return Evaluate(chessboard);

            chessboard.PerformTurn();

            List<Chessboard.Move> moves = GenerateMoves(chessboard);
            SortMoves(ref moves, chessboard);

            #region game end
            if (moves.Count == 0)
            {
                if (chessboard.IsCheck)
                {
                    if (depth == Depth - 1)
                    {
                        //var result = await Task.WhenAll(tasks);
                        //ValueForMoves[m] = result.Min();
                        ValueForMoves[m] = 99999;
                    }
                    return -99999; // end of the game, check
                }
                if (depth == Depth - 1)
                {
                    //var result = await Task.WhenAll(tasks);
                    //ValueForMoves[m] = result.Min();
                    ValueForMoves[m] = 0;
                }
                return 0; // end of the game, pat
            }
            #endregion

                foreach (var move in moves)
                {
                    chessboard.square[move.SX, move.SY].Piece!.Move(move);
                    chessboard.Turn = chessboard.Turn == Chessboard.FigureColor.White ? Chessboard.FigureColor.Black : Chessboard.FigureColor.White;
                    int res =- await FindBestMovesAsync(depth - 1, -beta, -alpha, chessboard, m);
                    chessboard.Undo();
                    if (res >= beta)
                        return beta;
                    alpha = alpha >= res ? alpha : res;
                }

                if (depth == Depth - 1)
                {
                    ValueForMoves[m] = -alpha;
                }

            return  alpha;
        }
    } // class
}
