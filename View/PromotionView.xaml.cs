using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf_GUI;

namespace Wpf_GUI.View
{
    /// <summary>
    /// Logika interakcji dla klasy PromotionView.xaml
    /// </summary>
    public partial class PromotionView : Window
    {
        public PromotionView()
        {
            InitializeComponent();
            //DataContext = new PromotionViewModel();
        }
    }
}
