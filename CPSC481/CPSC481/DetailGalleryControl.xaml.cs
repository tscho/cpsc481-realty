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
        private Image selected;
        private int selectedIndex;

        public DetailGalleryControl(House houseToDisplay)
        {
            this.DataContext = houseToDisplay;
            InitializeComponent();

            string SourceOfImages = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "Photos"); 

            foreach(var imgPath in houseToDisplay.images)
            {
                Image newImage = new Image() { Source = new BitmapImage(new Uri(imgPath)) };
                newImage.AddHandler(Image.MouseLeftButtonDownEvent, Delegate.CreateDelegate(Image.MouseLeftButtonDownEvent.HandlerType, this, "Image_MouseLeftButtonDown"));
                PicturePanel.Children.Add(newImage);
            }
            selected = (Image)PicturePanel.Children[0];
            selectedIndex = 0;
            DisplayImage.Source = selected.Source;
        }

        public DetailGalleryControl(House houseToDisplay, int tabIndex)
            : this(houseToDisplay)
        {
            modalTabs.SelectedIndex = tabIndex;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndex + 1 < PicturePanel.Children.Count)
            {
                selectedIndex++;
                DisplayImage.Source = ((Image)PicturePanel.Children[selectedIndex]).Source;
            }
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
                DisplayImage.Source = ((Image)PicturePanel.Children[selectedIndex]).Source;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var selectedImage = (Image)sender;
            selectedIndex = PicturePanel.Children.IndexOf(selectedImage);
            selected = selectedImage;
            DisplayImage.Source = ((Image)PicturePanel.Children[selectedIndex]).Source;
        }
    }
}
