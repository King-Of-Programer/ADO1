﻿using ADO1.MainWindows;
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

namespace ADO1
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void BasicsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new MainWindow().ShowDialog();
            this.Show();
        }

        private void OrmButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new ORMWindow().ShowDialog();
            this.Show();
        }
        private void DalButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new DalWindow().ShowDialog();
            this.Show();
        }
        private void EFButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new EFWindow().ShowDialog();
            this.Show();
        }
    }
}

