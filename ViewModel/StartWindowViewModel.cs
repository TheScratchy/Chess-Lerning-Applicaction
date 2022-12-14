using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess_Learning_Application;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf_GUI.View;
using System.Windows;
using System.IO;

namespace Wpf_GUI
{
    public class StartWindowViewModel : INotifyPropertyChanged
    {
        #region Properties
        private bool _isTimeEWnabled;
        public bool IsTimeEnabled 
        {
            get { return _isTimeEWnabled; }
            set
            {
                if (_isTimeEWnabled == value)
                    return;

                _isTimeEWnabled = value;
                OnPropertyChanged("_isTimeEnabled");
            }
        }

        private readonly ObservableCollection<int>_timeLimit  = new()
        {
            1,
            3,
            5,
            10,
            15,
            30,
            60
        };
        public ObservableCollection<int> TimeLimit
        {
            get { return _timeLimit; }
        }

        private readonly ObservableCollection<int> _timeInc = new()
        {
            0,
            1,
            3,
            5,
            15,
            30
        };
        public ObservableCollection<int> TimeInc
        {
            get { return _timeInc; }
        }

        public int Time { get; set; } = 10;
        public int TimeIncrementation { get; set; } = 1;
        private bool _playVsAI = true;
        public bool PlayVsAI
        {
            get { return _playVsAI; }
            set
            {
                if (_playVsAI == value)
                    return;

                _playVsAI = value;
                OnPropertyChanged("_playVsAI");
            }
        }
        public Chessboard.FigureColor ChosenAIColor { get; set; } = Chessboard.FigureColor.Black;
        public string AIDifficulty { get; set; } = "easy";
        private readonly ObservableCollection<string> _aiDiff = new()
        {
            "easy",
            "medium",
            "hard"
        };
        public ObservableCollection<string> AIDiff
        {
            get { return _aiDiff; }
        }

        private readonly ObservableCollection<Chessboard.FigureColor> _aiColor = new()
        {
            Chessboard.FigureColor.White,
            Chessboard.FigureColor.Black
        };
        public ObservableCollection<Chessboard.FigureColor> AIColor
        {
            get { return _aiColor; }
        }
        #endregion

        public ICommand StartGameCommand { get; private set; }
        public ICommand CreateNewBookCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public StartWindowViewModel()
        {
            StartGameCommand = new RelayCommand(o => StartGame(PlayVsAI, AIDifficulty, ChosenAIColor ,IsTimeEnabled, Time, TimeIncrementation));
            CreateNewBookCommand = new RelayCommand(o => CreateNewBook());
        }

        private void StartGame(bool playVsAI, string aiDiff, Chessboard.FigureColor aiColor, bool isTimeEnabled, int time, int timeInc )
        {
            if (Directory.Exists("Assets") == false)
            {
                MessageBox.Show("No directory named \"Assets\"");
                return;
            }

            try
            {
                GameBoard_Window Chessboard_Window = new(playVsAI, aiDiff, aiColor, isTimeEnabled, time, timeInc);

                Chessboard_Window.Show();
                //this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred. Check the \"Assets\" folder if it contains an image file" +
                    " for every figure named properly, i.e. color_figure.png");
            }
        }

        private async void CreateNewBook()
        {
            await Engine.Book.CreateBookFile();
            MessageBox.Show("Done");
        }
    }
}
