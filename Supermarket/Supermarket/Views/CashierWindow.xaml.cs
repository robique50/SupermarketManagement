using Supermarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Supermarket.Views
{
    public partial class CashierWindow : Window
    {
        public CashierWindow()
        {
            InitializeComponent();
            DataContext = new CashierVM();

        }
    }
}
