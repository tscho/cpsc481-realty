using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
using AxShockwaveFlashObjects;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CPSC481
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        ObservableCollection<House> results = new ObservableCollection<House>();
        private string currLat = "";
        private string currLon = "";


        public MainWindow()
        {
            InitializeComponent();

            string desc = "Treat yourself to a bright, lovely and well maintained 2 storey condo in charming Bow Ridge Landing. This home is an end unit with an extra main floor window to let in the natural light. The cosy kitchen has oak cupboards and laminate counter tops with a ceramic tile backsplash in calm, neutral tones. From the kitchen, a back door leads to a huge deck surrounded by lots of grass and trees, creating a private place to enjoy the outdoors. A living/dining room and 2 piece bathroom with stacked washer and dryer complete the main level. Upstairs there are 3 spacious bedrooms and a 4 piece bathroom. The master bedroom has a 4 piece ensuite and walk-in closet. The basement is partially developed with plenty of storage shelves and a workbench. There is an attached single car garage long enough to contain another work bench. The drive way is generous enough to park another car. What are you waiting for? View this home today!";
            
            results.Add(new House("2708", "Conrad Drive", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2", desc));
            results.Add(new House("300", "Meredith Road", "NE", "Brentwood,", "Calgary, ", "Alberta", "$400,000", "2", "2", desc));
            results.Add(new House("2508", "Centre Street", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2", desc));
            results.Add(new House("12", "Cheyenne Crescent", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2", desc));
            searchResults.ItemsSource = results;
            favourites.ItemsSource = results;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            AxShockwaveFlashObjects.AxShockwaveFlash axFlash = wfh.Child as AxShockwaveFlashObjects.AxShockwaveFlash;
            axFlash.FlashVars = "key= ABQIAAAAwuJcmtEbPbTOVHSybSa56BQqjzYXlVXz0uwSLGElMCZH0WZ4sRRwNXlCZ0os8CPeHow5izbL0WjQvw";
            axFlash.Movie = System.Windows.Forms.Application.StartupPath + "\\GoogleMaps.swf";

            axFlash.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(axFlash_FlashCall);
        }

        void axFlash_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            XDocument call = XDocument.Parse(e.request);

            var q = from c in call.Elements("invoke")
                    select new
                    {
                        Name = c.Attribute("name").Value,
                        Arguments = c.Element("arguments").Descendants()
                    };
            foreach (var i in q)
            {
                if (i.Name == "setPosition")
                {
                    currLat = i.Arguments.ElementAt(0).Value;
                    currLon = i.Arguments.ElementAt(1).Value;
                }
                //add other methods handlers here
            }

        }

        private void removeMarkers()
        {
                XElement call = new XElement("invoke",
                   new XAttribute("name", "remove"),
                   new XAttribute("returntype", "xml")
               );
                axFlash.CallFunction(call.ToString(SaveOptions.DisableFormatting)); 
        }

        private void clickSearch(object sender, RoutedEventArgs e)
        {

            //find home that meet search results
            
            foreach (House currHome in results)
            {
                MessageBox.Show(currHome.ToString());
                XElement call = new XElement("invoke",
                   new XAttribute("name", "add"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", currHome.Latitude),
                       new XElement("string", currHome.Longitude),
                       new XElement("string", currHome.address),
                       new XElement("string", currHome.price),
                       new XElement("string", currHome.area),
                       new XElement("string", currHome.city),
                       new XElement("string", currHome.province)
                   )
               );
                axFlash.CallFunction(call.ToString(SaveOptions.DisableFormatting));
            }

            XElement callArea = new XElement("invoke",
                   new XAttribute("name", "area"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", "NW")
                   )
            );
            axFlash.CallFunction(callArea.ToString(SaveOptions.DisableFormatting));
        }

        private void searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as House;
            if (item == null) return;

            XElement call = new XElement("invoke",
                   new XAttribute("name", "centre"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", item.Latitude),
                       new XElement("string", item.Longitude),
                       new XElement("string", item.address),
                       new XElement("string", item.price),
                       new XElement("string", item.area),
                       new XElement("string", item.city),
                       new XElement("string", item.province)
                   )
            );
            axFlash.CallFunction(call.ToString(SaveOptions.DisableFormatting));
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //Search_Click(sender, null);
        }

        private void addToFavs(object sender, RoutedEventArgs e)
        {

        }

        private void viewDetails(object sender, RoutedEventArgs e)
        {
            removeMarkers();
            var house = (House)((Button)sender).DataContext;
            Modal modal = new Modal(new DetailGalleryControl(house));
            modal.Owner = this;
            modal.ShowDialog();

        }

        private void viewGallery(object sender, RoutedEventArgs e)
        {
            var house = (House)((Button)sender).DataContext;
            Modal modal = new Modal(new DetailGalleryControl(house, 1));
            modal.Owner = this;
            modal.ShowDialog();
        }

        private void clickViewings(object sender, RoutedEventArgs e)
        {
            var info = new ContactInfo();
            Modal modal = new Modal();
            modal.setModalControl(new ContactInfoControl(modal, info));
            modal.Owner = this;
            modal.ShowDialog();
            modal = new Modal(new ConfirmationControl());
            modal.Owner = this;
            modal.ShowDialog();
        }
    }
}
