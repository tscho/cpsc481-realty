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
        public MainWindow()
        {
            InitializeComponent();

            results.Add(new House("2708", "Conrad Drive", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2"));
            results.Add(new House("300", "Meredith Road", "NE", "Brentwood,", "Calgary, ", "Alberta", "$400,000", "2", "2"));
            results.Add(new House("2508", "Centre Street", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2"));
            results.Add(new House("12", "Cheyenne Crescent", "Northwest", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2"));
            results.Add(new House("2708", "Conrad Drive", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2"));

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
                    //lat.Text = i.Arguments.ElementAt(0).Value;
                    //lng.Text = i.Arguments.ElementAt(1).Value;
                }
            }

        }



        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
                //Search_Click(sender, null);
        }

        private void clickStoreystInfo(object sender, RoutedEventArgs e)
        {

        }

        private void clickSquareFeetInfo(object sender, RoutedEventArgs e)
        {
        }

        private void clickSearch(object sender, RoutedEventArgs e)
        {            
            foreach(House currHome in results)
            {
                XElement call = new XElement("invoke",
                   new XAttribute("name", "add"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", currHome.address),
                       new XElement("string", currHome.price),
                       new XElement("string", currHome.area),
                       new XElement("string", currHome.city),
                       new XElement("string", currHome.province)
                   )
               );
                axFlash.CallFunction(call.ToString(SaveOptions.DisableFormatting));
            }
        }

        private void clickBedroomsInfo(object sender, RoutedEventArgs e)
        {
        }

        private void clickBathroomsInfo(object sender, RoutedEventArgs e)
        {

        }

        private void searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as House;
            if(item == null) return;

            XElement call = new XElement("invoke",
                   new XAttribute("name", "set"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", item.address)
                   )
            );
            axFlash.CallFunction(call.ToString(SaveOptions.DisableFormatting));
        }

        private void vieDetails(object sender, RoutedEventArgs e)
        {

        }

        private void addToFavs(object sender, RoutedEventArgs e)
        {

        }

        private void priceInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Modal modal = new Modal(results[0]);
            modal.Owner = this;
            modal.ShowDialog();
        }
    }
}
