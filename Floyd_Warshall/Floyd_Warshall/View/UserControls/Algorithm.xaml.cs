﻿using System;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Floyd_Warshall.View.UserControls
{
    /// <summary>
    /// Interaction logic for Algorithm.xaml
    /// </summary>
    public partial class Algorithm : UserControl
    {
        public Algorithm()
        {
            InitializeComponent();
        }

        private void MatrixGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            rowDef1.Height = new GridLength(1, GridUnitType.Star);
            rowDef2.Height = new GridLength(1, GridUnitType.Star);
        }
    }
}
