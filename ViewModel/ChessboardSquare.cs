using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Chess_Learning_Application;

namespace Wpf_GUI
{
    public class ChessboardSquare : INotifyPropertyChanged
    {
        public ChessboardSquare(Chessboard.Square square)
        {
            _square = square;
        }
        private Chessboard.Square _square
        {
            get;
            set;
        }

        public int Column { get; set; }

        public int Row { get; set; }

        public bool IsBeingAttacked
        {
            get
            {
                return _square.IsBeingAttacked;
            }
        }

        public Chessboard.FigureType? FigureType 
        { 
            get
            {
                return _square.Piece?.Type;
            }
            set
            {

            }
         }

        public Chessboard.FigureColor? FigureColor
        {
            get
            {
                return _square.Piece?.Color;
            }
        }

        public Chessboard.FigureColor SquareColor { get; set; }

        #region Types
        #region Whites
        public bool IsWhitePawn
        {
            get { return FigureColor == Chessboard.FigureColor.White && FigureType == Chessboard.FigureType.Pawn ? true : false; }
        }
        public bool IsWhiteKnight
        {
            get { return FigureColor == Chessboard.FigureColor.White && FigureType == Chessboard.FigureType.Knight ? true : false; }
        }
        public bool IsWhiteBishop
        {
            get { return FigureColor == Chessboard.FigureColor.White && FigureType == Chessboard.FigureType.Bishop; }
        }
        public bool IsWhiteRook
        {
            get { return FigureColor == Chessboard.FigureColor.White && FigureType == Chessboard.FigureType.Rook ? true : false; }
        }
        public bool IsWhiteQueen
        {
            get { return FigureColor == Chessboard.FigureColor.White && FigureType == Chessboard.FigureType.Queen ? true : false; }
        }
        public bool IsWhiteKing
        {
            get { return FigureColor == Chessboard.FigureColor.White && FigureType == Chessboard.FigureType.King ? true : false; }
        }
        #endregion
        #region Blacks
        public bool IsBlackPawn
        {
            get { return FigureColor == Chessboard.FigureColor.Black && FigureType == Chessboard.FigureType.Pawn ? true : false; }
        }
        public bool IsBlackKnight
        {
            get { return FigureColor == Chessboard.FigureColor.Black && FigureType == Chessboard.FigureType.Knight ? true : false; }
        }
        public bool IsBlackBishop
        {
            get { return FigureColor == Chessboard.FigureColor.Black && FigureType == Chessboard.FigureType.Bishop ? true : false; }
        }
        public bool IsBlackRook
        {
            get { return FigureColor == Chessboard.FigureColor.Black && FigureType == Chessboard.FigureType.Rook ? true : false; }
        }
        public bool IsBlackQueen
        {
            get { return FigureColor == Chessboard.FigureColor.Black && FigureType == Chessboard.FigureType.Queen ? true : false; }
        }
        public bool IsBlackKing
        {
            get { return FigureColor == Chessboard.FigureColor.Black && FigureType == Chessboard.FigureType.King ? true : false; }
        }
        #endregion
        #endregion

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
