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

            results.Add(new House("2708", "Conrad Drive", "NW", "Brentwood", "Calgary", "Alberta", "200,000"));
            results.Add(new House("2600", "Conrad Drive", "NW", "Brentwood", "Calgary", "Alberta", "200,000"));

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
            AxShockwaveFlashObjects.AxShockwaveFlash axFlash = wfh.Child as AxShockwaveFlashObjects.AxShockwaveFlash;
           
            for (int i = 0; i < results.Count; i++)
            {
                XElement call = new XElement("invoke",
                   new XAttribute("name", "Add"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", "2708 Conrad Dr. N.W."),
                       new XElement("string", "200,000")
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

    }
}
