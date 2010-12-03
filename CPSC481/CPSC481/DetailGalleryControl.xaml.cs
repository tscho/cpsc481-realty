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
using System.Windows.Media.Effects;
using System.Windows.Navigation;

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

            DisplayImage.Source = houseToDisplay.images[0].Source;
        }

        public DetailGalleryControl(House houseToDisplay, int tabIndex)
            : this(houseToDisplay)
        {
            modalTabs.SelectedIndex = tabIndex;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if(listPhotos.SelectedIndex + 1 < listPhotos.Items.Count)
                listPhotos.SelectedIndex++;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if(listPhotos.SelectedIndex > 0)
                listPhotos.SelectedIndex--;
        }

        private void listPhotos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Image selected = (Image)listPhotos.SelectedItem;
            if (selected != null)
                DisplayImage.Source = selected.Source;
        }

        //add to favourites click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Modal)Window.GetWindow(this)).Owner).addToFavs(sender, e);
        }
    }
}
