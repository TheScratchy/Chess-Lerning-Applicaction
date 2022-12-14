using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Chess_Learning_Application
{

    public class Game
    {
        public Chessboard Board;
        public Chessboard.FigureColor Turn 
        {
            get { return Board.Turn; }
            set { Board.Turn = value; }
        }

        private Engine _engine;

        public event EventHandler<Chessboard.FigureColor>? GameHasEndedWithCheckEvent;
        public event EventHandler PatEvent;
        public event EventHandler AIHasMovedEvent;
        public event EventHandler SecondPassedEvent;

        public string AIDiff { get;  set; }
        public bool PlayVsAI { get; set; }
        public Chessboard.FigureColor AIColor { get; set; } = Chessboard.FigureColor.White;
        public bool IsTimeEnabled { get;  set; }
        public int Time { get;  set; }
        public int TimeIncrementation { get;  set; }

        private Timer _whiteTimer;
        public Timer WhiteTimer { get { return _whiteTimer; } }
        private Timer _blackTimer;
        public Timer BlackTimer { get { return _blackTimer; } }
        private Timer _timer;
        public Timer Timer { get { return _timer; } }
        private int _whiteSec;
        public int WhiteSecRemaining { get { return _whiteSec; } }
        private int _blackSec;
        public int BlackSecRemaining { get { return _blackSec; } }


        private int GetAIDifficulty(string aiDiff)
        {
            int diff = 0;

            switch (aiDiff)
            {
                case "easy":
                    {
                        return 3;
                    }
                case "medium":
                    {
                        return 1;
                    }
                case "hard":
                    {
                        return 0;
                    }
            }
            return diff;
        }
        private void StartTimer(int minutes)
        {
            _whiteSec = 0;
            _blackSec = 0;
            _whiteTimer = new Timer(1000);
            _blackTimer = new Timer(1000);
            _timer = new Timer(1000);

            _timer.AutoReset = true;
            _whiteTimer.AutoReset = true;
            _blackTimer.AutoReset = true;
            _whiteTimer.Elapsed += TimerOutWhite;
            _blackTimer.Elapsed += TimerOutBlack;
            _timer.Elapsed += TimerSecondPassed;
        }

        private void TimerOut()
        {
            _whiteTimer.Stop();
            _blackTimer.Stop();
            if (Turn == Chessboard.FigureColor.White)
                GameHasEndedWithCheckEvent?.Invoke(this, Chessboard.FigureColor.White);
            else GameHasEndedWithCheckEvent?.Invoke(this, Chessboard.FigureColor.Black);
            return;
        }

        private void TimerOutWhite(object? sender, ElapsedEventArgs e)
        {
            _whiteSec++;
            //_whiteSec -= TimeIncrementation;
            if (_whiteSec == Time * 60)
                TimerOut();
        }

        private void TimerOutBlack(object? sender, ElapsedEventArgs e)
        {
            _blackSec++;
            //_blackSec += TimeIncrementation;
            if (_blackSec == Time * 60)
                TimerOut();
        }

        private void TimerSecondPassed(object? sender, ElapsedEventArgs e)
        {
            SecondPassedEvent?.Invoke(this, EventArgs.Empty);
        }
        private async Task PerformTurnAIAsync()
        {
            Chessboard.Move move = new();
            string moves = "";
            foreach (var m in Board.MovesHistory)
            {
                moves += m.AlgebraicNotation + " ";
            }

            if (_engine.book.Responses.ContainsKey(moves))
            {
                var availableOpenings = _engine.book.Responses[moves];
                Random rnd = new();
                int randIndex = rnd.Next(0, availableOpenings.Count);
                move = availableOpenings.ElementAt(randIndex);
            }
            else
            {
                _engine.ValueForMoves.Clear();
                await _engine.StartSearch(Board);
                var highestScore = _engine.ValueForMoves.Aggregate((x, y) => x.Value > y.Value ? x : y).Value;


                //move = _engine.ValueForMoves.Aggregate((x, y) => x.Value > y.Value + GetAIDifficulty(AIDiff) ? x : y).Key;
                move = _engine.ValueForMoves.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                for (int i = 0; i < GetAIDifficulty(AIDiff); i++)
                {
                    _engine.ValueForMoves[move] = _engine.ValueForMoves[move] - 3;
                    move = _engine.ValueForMoves.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                }
            }
            Board.MoveFigure(move);
            if (move.WasPromotion == false)
                Board.PerformTurn();
            AIHasMovedEvent?.Invoke(this, EventArgs.Empty);
        }

        public async Task PerfromTurnAsync()
        {
            _blackTimer.Stop();
            _whiteTimer.Stop();
            _timer.Stop();
            bool condition = false;
            Board.PerformTurn();

            #region end of the game

            #region Check if pieces can move
            foreach (Chessboard.Figure piece in Board.Figures)
            { 
                if (piece.Color != Turn)
                        continue;

                if (piece.LegalMoves.Count > 0)
                    condition = true;
                }

            #endregion

            
            if (!condition) // No piece can move, ergo end of the game
            {
                if (Board.IsCheck)
                    GameHasEndedWithCheckEvent?.Invoke(this, Board.Turn == Chessboard.FigureColor.White ? Chessboard.FigureColor.Black : Chessboard.FigureColor.White);

                else PatEvent?.Invoke(this, EventArgs.Empty);
                return;
            }
            #endregion

            if (IsTimeEnabled)
            {
                if (Turn == Chessboard.FigureColor.White)
                {
                    _whiteTimer.Start();
                    _whiteSec -= TimeIncrementation;
                }
                else
                {
                    _blackTimer.Start();
                    _blackSec -= TimeIncrementation;
                }
                _timer.Start();
            }
            if (PlayVsAI && _engine.PlayAsColor == Turn)
                await PerformTurnAIAsync();
        }

        public Game(bool playVsAI, string aiDiff, Chessboard.FigureColor aiColor, bool isTimeEnabled, int time, int timeInc)
        {
            PlayVsAI = playVsAI;
            AIDiff = aiDiff;
            AIColor = aiColor;
            IsTimeEnabled = isTimeEnabled;
            Time = time;
            TimeIncrementation = timeInc;
            
            StartTimer(Time);

            Board = new Chessboard();
            _engine = new()
            {
                PlayAsColor = AIColor,
                Depth = 4
            };

            Board.SetChessbordUsingFEN(Chessboard.StartingFEN);

            PerfromTurnAsync();
            Board.MoveWasPerformedEvent += _chessboard_MoveWasPerformedEvent;

        }

        private void _chessboard_MoveWasPerformedEvent(object? sender, Chessboard.Move e)
        {
            this.PerfromTurnAsync();
        }
    }
}
