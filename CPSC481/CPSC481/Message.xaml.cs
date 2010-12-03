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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CPSC481
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : UserControl
    {
        public String MessageText { get; set; }
        public Message(String messageText)
        {
            this.DataContext = this;
            this.MessageText = messageText;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
