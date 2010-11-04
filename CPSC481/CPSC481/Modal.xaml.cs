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

        public Modal()
        {
            InitializeComponent();
        }

        public Modal(UserControl control) : this()
        {
            setModalControl(control);
        }

        public void setModalControl(UserControl control)
        {
            Grid.SetRow(control, 1);
            control.Margin = new Thickness(18, 0, 18, 18);
            grid.Children.Add(control);
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
