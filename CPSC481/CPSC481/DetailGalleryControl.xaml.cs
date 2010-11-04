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
    /// Interaction logic for DetailGalleryControl.xaml
    /// </summary>
    public partial class DetailGalleryControl : UserControl
    {
        public DetailGalleryControl(House houseToDisplay)
        {
            this.DataContext = houseToDisplay;
            InitializeComponent();
        }
    }
}
