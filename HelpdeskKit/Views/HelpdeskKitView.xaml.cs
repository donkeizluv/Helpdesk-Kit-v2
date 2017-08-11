﻿using HelpdeskKit.Views;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelpdeskKit.ViewModels;

namespace HelpdeskKit
{
    /// <summary>
    /// Interaction logic for HelpdeskKitView.xaml
    /// </summary>
    public partial class HelpdeskKitView : Window
    {
        //public HelpdeskKitViewModel ViewModel { get; private set; }
        public HelpdeskKitView()
        {
            InitializeComponent();
        }

        private void MenuItemListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
    }
}
