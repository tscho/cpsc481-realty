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
    /// Interaction logic for ContactInfoControl.xaml
    /// </summary>
    public partial class ContactInfoControl : UserControl
    {
        private Modal modal;
        public ContactInfoControl(Modal modal, ContactInfo info)
        {
            this.modal = modal;
            this.DataContext = info;
            InitializeComponent();
        }

        private void submitViewing_click(object sender, RoutedEventArgs e)
        {
            ContactInfo info = (ContactInfo)DataContext;

            if (!String.IsNullOrEmpty(info.Name) && (!String.IsNullOrEmpty(info.Email) || !String.IsNullOrEmpty(info.Phone)))
            {
                modal.DialogResult = true;
                info.save();
                modal.Close();
            }
            else
            {
                Modal modal = new Modal(new Message("Name and one of Phone or E-Mail must be provided"));
                modal.Owner = Window.GetWindow(this);
                modal.ShowDialog();
            }
        }
    }
}
