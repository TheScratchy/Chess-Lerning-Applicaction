﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_GUI.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new StartWindowViewModel();
        }
        //private void Start_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    //GameBoard_Window Chessboard_Window = new ();
        //    //Chessboard_Window.Show();
        //    //this.Close();
        //}
    }
}
