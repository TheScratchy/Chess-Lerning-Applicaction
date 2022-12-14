using Chess_Learning_Application;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Wpf_GUI.View;
using System.Timers;
using System.Windows.Input;

namespace Wpf_GUI
{
    public class ViewModelChessboard : INotifyPropertyChanged
    {
        private Game _game;

        private PromotionView? PromotionWindow;

        private Chessboard.Position _pawnsPositionReadyToPromotion;

        private List<ChessboardSquare> _squaresDuringWhiteTurn;
        private List<ChessboardSquare> _squaresDuringBlackTurn;
        public List<ChessboardSquare> Squares 
        {
            get
            {
                if (_chosenDisplay == "Auto")
                        {
                            if (PlayVsAI)
                                return AIColor == Chessboard.FigureColor.White ? _squaresDuringBlackTurn : _squaresDuringWhiteTurn;
                            return _game.Turn == Chessboard.FigureColor.White ? _squaresDuringWhiteTurn : _squaresDuringBlackTurn;
                        }
                if (_chosenDisplay == "White")
                        {
                            return _squaresDuringWhiteTurn;
                        }
                return _squaresDuringBlackTurn;
            }
        }
        public string AIDiff { get;  set; }
        public bool PlayVsAI { get; set; } = true;
        public Chessboard.FigureColor AIColor { get; set; } = Chessboard.FigureColor.White;
        public bool IsTimeEnabled { get;  set; }
        public int Time { get;  set; }
        public int TimeIncrementation { get; set; }
        public Timer WhiteTimer { get { return _game.WhiteTimer; } }
        public Timer BlackTimer { get { return _game.BlackTimer; } }
        public string SecondsRemainingWhite 
        { 
            get 
            {
                int tempInt = Time * 60 - _game.WhiteSecRemaining;
                string temp = IsTimeEnabled ?
                    string.Format($"{(Time * 60 - _game.WhiteSecRemaining) / 60:D2}:{(Time * 60 - _game.WhiteSecRemaining) % 60:D2}")
                    : "";
                return temp; 
            }
        }
        public string SecondsRemainingBlack
        {
            get
            {
                int tempInt = Time * 60 - _game.BlackSecRemaining;
                string temp = IsTimeEnabled ?
                    string.Format($"{(Time * 60 - _game.BlackSecRemaining) / 60:D2}:{(Time * 60 - _game.BlackSecRemaining) % 60:D2}")
                    : "";
                return temp;
            }
        }
        private readonly ObservableCollection<string> _forceDisplay = new()
        {
                "Auto",
                "White",
                "Black"
        };
        public ObservableCollection<string> ForceDisplay
        {
            get { return _forceDisplay; }
        }
        private string _chosenDisplay = "Auto";
        public string ChosenDisplay
        {
            get
            {
                return _chosenDisplay;
            }
            set
            {
                if (_chosenDisplay == value)
                    return;
                _chosenDisplay = value;
                OnPropertyChanged("Squares");
            }
        }

        public ObservableCollection<string> Moves
        {
            get
            {
                ObservableCollection<string> moves = new ();

                for (int i = 0; i < _game.Board.MovesHistory.Count; i += 2)
                {
                    string m = _game.Board.MovesHistory[i].AlgebraicNotation!;
                    if (_game.Board.MovesHistory.Count > i + 1)
                        m += "  " + _game.Board.MovesHistory[i + 1].AlgebraicNotation;
                    else m += "       ";
                    moves.Add(m);
                }
                return moves;

            }
        }

        public ObservableCollection<Chessboard.Move> WhiteMoves 
        { 
            get 
            {
                ObservableCollection<Chessboard.Move> moves = new();
                for (int i = 0; i < _game.Board.MovesHistory.Count; i+=2)
                {
                    moves.Add(_game.Board.MovesHistory[i]);
                }
                return moves;
            }
        }

        public ObservableCollection<Chessboard.Move> BlackMoves
        {
            get
            { 
            ObservableCollection<Chessboard.Move> moves = new ();
                for (int i = 1; i<_game.Board.MovesHistory.Count; i+=2)
                {
                    moves.Add(_game.Board.MovesHistory[i]);
                }
                return moves;
            }
        }

        public ICommand SquareClickedCommand { get; private set; }
        public ICommand UndoMoveCommand { get; private set; }

        private Chessboard.Position _lastClickedSquare;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public ViewModelChessboard(bool playVsAI, string aiDiff, Chessboard.FigureColor aiColor, bool isTimeEnabled, int time, int timeInc)
        {
            PlayVsAI = playVsAI;
            AIDiff = aiDiff;
            AIColor = aiColor;
            IsTimeEnabled = isTimeEnabled;
            Time = time;
            TimeIncrementation = timeInc;

            //Squares = new List<ChessboardSquare>();
            _squaresDuringWhiteTurn = new();
            _squaresDuringBlackTurn = new();
            _game = new Game(PlayVsAI, AIDiff, AIColor, IsTimeEnabled, Time, TimeIncrementation);
            _lastClickedSquare.X = 7;
            _lastClickedSquare.Y = 7;

            for (int j = 7; j >= 0; j--)
            {
                for (int i = 0; i < 8; i++)
                {
                    _squaresDuringWhiteTurn.Add(new ChessboardSquare(_game.Board.square[i, j])
                    {
                        Column = _game.Board.square[i, j].SquarePosition.X,
                        Row = _game.Board.square[i, j].SquarePosition.Y,
                        SquareColor = _game.Board.square[i, j].SquareColor
                    });

                    _squaresDuringBlackTurn.Add(new ChessboardSquare(_game.Board.square[7 - i, 7- j])
                    {
                        Column = _game.Board.square[7 - i, 7 - j].SquarePosition.X,
                        Row = _game.Board.square[7 - i, 7 - j].SquarePosition.Y,
                        SquareColor = _game.Board.square[7 - i, 7 - j].SquareColor
                    });
                }
            }

            SquareClickedCommand = new RelayCommand<ChessboardSquare>(o => SquareClicked(o));
            UndoMoveCommand = new RelayCommand(o => UndoMove());

            #region Events
            _game.Board.PawnPromotionEvent += _chessboard_PawnPromotionEvent;
            _game.GameHasEndedWithCheckEvent += _game_GameHasEndedEvent;
            _game.PatEvent += _game_PatEvent;
            _game.AIHasMovedEvent += _game_AIHasMovedEvent;
            _game.SecondPassedEvent += _game_SecondPassedEvent;
            #endregion

        }

        private void _game_SecondPassedEvent(object? sender, EventArgs e)
        {
            OnPropertyChanged("SecondsRemainingBlack");
            OnPropertyChanged("SecondsRemainingWhite");
        }

        private void _game_AIHasMovedEvent(object? sender, EventArgs e)
        {
            UpdateProperties();
        }

        private void _chessboard_PawnPromotionEvent(object? sender, Chessboard.Position e)
        {
            PromotionWindow = new();
            PromotionWindow.DataContext = new PromotionViewModel();
            ((PromotionViewModel)PromotionWindow.DataContext).PromotionWasChosenEvent += ViewModelChessboard_PromotionWasChosenEvent;
            _pawnsPositionReadyToPromotion = e;
            PromotionWindow.Show();
        }

        private void ViewModelChessboard_PromotionWasChosenEvent(object? sender, string e)
        {
            _game.Board.Promote(_pawnsPositionReadyToPromotion, e);
            PromotionWindow?.Close();
            UpdateProperties();

        }

        private void _game_PatEvent(object? sender, EventArgs e)
        {
            MessageBox.Show("PAT! Draw");
        }

        private void _game_GameHasEndedEvent(object? sender, Chessboard.FigureColor color)
        {
            foreach (var piece in _game.Board.Figures)
            {
                piece.LegalMoves.Clear();
            }
            MessageBox.Show($"The game has ended! {color.ToString()} has won");
        }
        private void SquareClicked(ChessboardSquare square)
        {
            #region Refreshing Being Attacked property
            foreach (var sq in _game.Board.square)
            {
                sq.IsBeingAttacked = false;
                UpdateProperties();
            }
            #endregion

            if (_game.Board.square[square.Column, square.Row].Piece?.Color == _game.Turn && !(_game.Board.square[square.Column, square.Row].Piece?.Color == _game.AIColor &&  _game.PlayVsAI))
            {
                _lastClickedSquare.X = square.Column; 
                _lastClickedSquare.Y = square.Row;

               // sets attacking squares
                foreach (Chessboard.Move m in _game.Board.square[_lastClickedSquare.X, _lastClickedSquare.Y].Piece!.LegalMoves)
                {
                    _game.Board.square[m.DX, m.DY].IsBeingAttacked = true;
                }
                UpdateProperties();
                return;
            } // if it is our piece

            if (_game.Board.square[_lastClickedSquare.X, _lastClickedSquare.Y].Piece?.Color != _game.Turn) // last piece is a n enemy piece
                return;

            //if (_game.Board.square[_lastClickedSquare.X, _lastClickedSquare.Y].Piece?.Color == _game.AIColor && _game.PlayVsAI) // that's computer's piece, don't move it
            //    return;


            Chessboard.Move move = new(_lastClickedSquare.X, _lastClickedSquare.Y, square.Column, square.Row);
            bool condition = false;
            foreach (Chessboard.Move m in _game.Board.square[_lastClickedSquare.X, _lastClickedSquare.Y].Piece?.LegalMoves!)
            {
                if (m.DX == move.DX && m.DY == move.DY && m.SX == move.SX && m.SY == move.SY)
                {
                    condition = true;
                    break;
                }
            }
            if (!condition) // no Move was found
                return;
            _game.Board.MoveFigure(move);
            if (move.WasPromotion)
                foreach (var piece in _game.Board.Figures)
                {
                    piece.LegalMoves.Clear();
                }

            UpdateProperties();
        }
        private void UndoMove()
        {
            if (_game.Board.MovesHistory.Count == 0)
                return;
            if ((PlayVsAI && _game.Turn == AIColor) == false)
            {
                if (PlayVsAI)
                    _game.Board.Undo();
                _game.Board.Undo();
                _game.Board.PerformTurn();

            }
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            #region Updating Squares
            foreach (var sq in Squares)
            {
                #region White
                sq.OnPropertyChanged("IsWhitePawn");
                sq.OnPropertyChanged("IsWhiteKnight");
                sq.OnPropertyChanged("IsWhiteBishop");
                sq.OnPropertyChanged("IsWhiteRook");
                sq.OnPropertyChanged("IsWhiteQueen");
                sq.OnPropertyChanged("IsWhiteKing");
                #endregion
                #region Black
                sq.OnPropertyChanged("IsBlackPawn");
                sq.OnPropertyChanged("IsBlackKnight");
                sq.OnPropertyChanged("IsBlackBishop");
                sq.OnPropertyChanged("IsBlackRook");
                sq.OnPropertyChanged("IsBlackQueen");
                sq.OnPropertyChanged("IsBlackKing");
                #endregion
                sq.OnPropertyChanged("IsBeingAttacked");
            }
            #endregion
            OnPropertyChanged("WhiteMoves");
            OnPropertyChanged("BlackMoves");
            OnPropertyChanged("Squares");
            OnPropertyChanged("Moves");


        }

    }
}
