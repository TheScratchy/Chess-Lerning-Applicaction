using System.Windows;
using Chess_Learning_Application;

namespace Wpf_GUI.View
{
    /// <summary>
    /// Logika interakcji dla klasy GameBoard_Window.xaml
    /// </summary>
    public partial class GameBoard_Window : Window
    {
        public GameBoard_Window(bool playVsAI, string aiDiff, Chessboard.FigureColor aiColor, bool isTimeEnabled, int time, int timeInc)
        {
            InitializeComponent();
            DataContext = new ViewModelChessboard( playVsAI,  aiDiff,  aiColor,  isTimeEnabled,  time,  timeInc);
        }
    }
}
