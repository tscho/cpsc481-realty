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
        private Brush inactiveBrush;

        public Modal(House houseToDisplay)
        {
            InitializeComponent();
            detailGrid.DataContext = houseToDisplay;
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CloseBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            inactiveBrush = CloseBtn.Background;
            CloseBtn.Background = SystemColors.ActiveBorderBrush;
        }

        private void CloseBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBtn.Background = inactiveBrush;
        } 
    }
}
