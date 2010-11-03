using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CPSC481
{
    /// <summary>
    /// Interaction logic for Modal.xaml
    /// </summary>
    public partial class Modal : Window
    {
        private bool willClose;

        public Modal(House houseToDisplay)
        {
            InitializeComponent();
            willClose = false;
            detailGrid.DataContext = houseToDisplay;
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseBtn.Background = Brushes.YellowGreen;
            willClose = true;
        }

        private void CloseBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBtn.Background = SystemColors.ActiveBorderBrush;
        }

        private void CloseBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBtn.Background = Brushes.Transparent;
            willClose = false;
        }

        private void CloseBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CloseBtn.Background = Brushes.Transparent;
            if (willClose)
                this.Close();
        } 
    }
}
