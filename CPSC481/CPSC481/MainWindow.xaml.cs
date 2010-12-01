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
using AxShockwaveFlashObjects;
using System.Collections.ObjectModel;
using System.IO;

namespace CPSC481
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        ObservableCollection<House> CurrentPage = new ObservableCollection<House>();
        ObservableCollection<House> FavouriteListings = new ObservableCollection<House>();
        ObservableCollection<House> AllListings = new ObservableCollection<House>();
        private IEnumerable<House> Results;
        private string currLat = "";
        private string currLon = "";
        public string SourceOfImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "Photos");
        public string AreaTest;
        private const double listingsPerPage = 2;
        private DataItem[] pages;

        public MainWindow()
        {
            InitializeComponent();

            setupTestData();
            searchResults.ItemsSource = CurrentPage;
            favourites.ItemsSource = FavouriteListings;
            priceLow.ItemsSource = prices;
            priceHigh.ItemsSource = prices;
            buildingType.ItemsSource = buildingTypes;
            quandrant.ItemsSource = areas;
            SquareFeetLow.ItemsSource = sqFeet;
            SquareFeetHigh.ItemsSource = sqFeet;
            Bedrooms.ItemsSource = bedrooms;
            Bathrooms.ItemsSource = bathrooms;
            listingType.ItemsSource = listingTypes;
            Feature.ItemsSource = features;
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
            removeMarkers();

            var listingTypesSelected = new List<ListingType>();
            foreach (var item in listingType.SelectedItems)
            {
                listingTypesSelected.Add((ListingType)((DataItem)item).Value);
            }

            var buildingTypesSelected = new List<BuildingType>();
            foreach (var item in buildingType.SelectedItems)
            {
                buildingTypesSelected.Add((BuildingType)((DataItem)item).Value);
            }

            var quadrantsSelected = new List<Quadrant>();
            foreach (var item in quandrant.SelectedItems)
            {
                Quadrant toAdd = (Quadrant)((DataItem)item).Value;
                quadrantsSelected.Add(toAdd);
                XElement callArea = new XElement("invoke",
                   new XAttribute("name", "area"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", toAdd.ToString())
                   )
            );
                axFlash.CallFunction(callArea.ToString(SaveOptions.DisableFormatting));
            }

            int priceLowSelected = 0;
            int priceHighSelected = 0;
            if (priceLow.SelectedValue != null)
            {
                priceLowSelected = (int)priceLow.SelectedValue;
            }
            if (priceHigh.SelectedValue != null)
            {
                priceHighSelected = (int)priceHigh.SelectedValue;
            }

            int squareFeetLowSelected = 0;
            int squareFeetHighSelected = 0;
            if(SquareFeetLow.SelectedValue != null)
            {
                squareFeetLowSelected = (int)SquareFeetLow.SelectedValue;
            }
            if(SquareFeetHigh.SelectedValue != null)
            {
                squareFeetHighSelected = (int)SquareFeetHigh.SelectedValue;
            }

            int bedroomsSelected = 0;
            if (Bedrooms.SelectedValue != null)
            {
                bedroomsSelected = (int)Bedrooms.SelectedValue;
            }

            int bathroomsSelected = 0;
            if (Bathrooms.SelectedValue != null)
            {
                bathroomsSelected = (int)Bathrooms.SelectedValue;
            }

            Features searchFeatures = 0;
            foreach (var item in Feature.SelectedItems)
            {
                searchFeatures |= (Features)((DataItem)item).Value;
            }

            //find home that meet search results
            Results = new ObservableCollection<House>(AllListings.Where(x =>
            {
                return (quadrantsSelected.Count == 0 || quadrantsSelected.Contains(x.quadrant)) &&
                (listingTypesSelected.Count == 0 || listingTypesSelected.Contains(x.listingType)) &&
                (buildingTypesSelected.Capacity == 0 || buildingTypesSelected.Contains(x.buildingType)) &&
                (searchFeatures == 0 ||(x.features & searchFeatures) > 0) &&
                x.bathrooms >= bathroomsSelected &&
                x.bedrooms >= bedroomsSelected &&
                x.squareFeet >= squareFeetLowSelected &&
                x.squareFeet <= squareFeetHighSelected &&
                x.price >= priceLowSelected &&
                x.price <= priceHighSelected;
            }));

            
            generatePages();

            CurrentPage.Clear();
            foreach (var house in Results.Take((int)listingsPerPage))
            {
                CurrentPage.Add(house);
            }
            
            foreach (House currHome in Results)
            {
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
        }

        private void previousPage(object sender, RoutedEventArgs e)
        {
            DataItem item = (DataItem)listBoxPages.SelectedItem;
            int currIndex = listBoxPages.SelectedIndex;
            if((int)item.Value > 1)
            {
                listBoxPages.SelectedIndex = currIndex - 1;
                CurrentPage.Clear();
                foreach (var house in Results.Skip(((int)item.Value - 2) * (int)listingsPerPage).Take((int)listingsPerPage))
                {
                    CurrentPage.Add(house);
                }
            }
            else
            {
                MessageBox.Show("Already on page 1");
            }
        }

        private void nextPage(object sender, RoutedEventArgs e)
        {
            double numPages = Math.Ceiling(Results.Count() / listingsPerPage);
            DataItem item = (DataItem)listBoxPages.SelectedItem;
            int currIndex = listBoxPages.SelectedIndex;
            if ((int)item.Value < pages.Count())
            {
                listBoxPages.SelectedIndex = currIndex + 1;
                CurrentPage.Clear();
                foreach (var house in Results.Skip((currIndex + 1) * (int)listingsPerPage).Take((int)listingsPerPage))
                {
                    CurrentPage.Add(house);
                }
            }
            else
            {
                MessageBox.Show("Already on last page");
            }
        }

        private void generatePages()
        {
            double numPages = Math.Ceiling(Results.Count()/listingsPerPage);
            DataItem item;

            pages = new DataItem[(int)numPages];
            for (int i = 0; i < numPages; i++)
                {
                    item = new DataItem();
                    item.Display = (i + 1).ToString();
                    item.Value = i + 1;
                    pages[i] = item;
                }

            listBoxPages.ItemsSource = pages;
            listBoxPages.SelectedIndex = 0;
        }

        private void favourites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as House;
            if (item == null) return;
            else centreOnListing(item);
        }

        private void searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1) { return; }
            var item = e.AddedItems[0] as House;
            if (item == null) return;
            else centreOnListing(item);
        }

        private void centreOnListing(House centreHouse)
        {
            XElement call = new XElement("invoke",
                   new XAttribute("name", "centre"),
                   new XAttribute("returntype", "xml"),
                   new XElement("arguments",
                       new XElement("string", centreHouse.Latitude),
                       new XElement("string", centreHouse.Longitude),
                       new XElement("string", centreHouse.address),
                       new XElement("string", centreHouse.price),
                       new XElement("string", centreHouse.area),
                       new XElement("string", centreHouse.city),
                       new XElement("string", centreHouse.province)
                   )
            );
            axFlash.CallFunction(call.ToString(SaveOptions.DisableFormatting));
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //Search_Click(sender, null);
        }

        private void removeFromFavs(object sender, RoutedEventArgs e)
        {
            var house = (House)((Button)sender).DataContext;
            FavouriteListings.Remove(house);
        }


        private void addToFavs(object sender, RoutedEventArgs e)
        {
            var house = (House)((Button)sender).DataContext;
            if (!FavouriteListings.Contains(house))
            {
                FavouriteListings.Add(house);
            }
            else
            {
                MessageBox.Show("House already exists in your favourites.");
            }
        }

        private void viewDetails(object sender, RoutedEventArgs e)
        {
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

        public void setupTestData()
        {
            string imageTag = "house";
            string image = "";
            List<string> images = new List<string>();
            //images.Add(Path.Combine(SourceOfImages, "house1_1.jpg"));

                for (int j = 0; j < 4; j++)
                {
                    if (j == 0)
                        
                    imageTag = "1" + "_" + j.ToString();
                    images.Add(Path.Combine(SourceOfImages, imageTag));
                }
                image = Path.Combine(SourceOfImages, "house1_1.jpg");
                string desc = "Treat yourself to a bright, lovely and well maintained 2 storey condo in charming Bow Ridge Landing. This home is an end unit with an extra main floor window to let in the natural light. The cosy kitchen has oak cupboards and laminate counter tops with a ceramic tile backsplash in calm, neutral tones. From the kitchen, a back door leads to a huge deck surrounded by lots of grass and trees, creating a private place to enjoy the outdoors. A living/dining room and 2 piece bathroom with stacked washer and dryer complete the main level. Upstairs there are 3 spacious bedrooms and a 4 piece bathroom. The master bedroom has a 4 piece ensuite and walk-in closet. The basement is partially developed with plenty of storage shelves and a workbench. There is an attached single car garage long enough to contain another work bench. The drive way is generous enough to park another car. What are you waiting for? View this home today!";
                AllListings.Add(new House("2708", "Conrad Drive", Quadrant.NW, "Brentwood,", "Calgary, ", "Alberta", 200000, 2, 2, desc, image, images, 2300, Features.Fireplace, ListingType.Residential, BuildingType.House));

                for (int j = 0; j < 4; j++)
                {
                    if (j == 0)
                        image = Path.Combine(SourceOfImages, imageTag);
                    imageTag = "2" + "_" + j.ToString();
                    images.Add(Path.Combine(SourceOfImages, imageTag));
                }
                image = Path.Combine(SourceOfImages, "house2_1.jpg");
                desc = "ATTENTION INVESTORS, BUILDERS AND REVENUE GENERATORS. Here is a sub dividable, RC-2, 50' x 145' lot w/ a current tenant paying $3000 per month!! (modestly furnished). Tenants are willing to stay while you decide what to build and collect income while you wait--Or live in this cute bungalow until its time for the dream home(s) to be built. Apart from wall to wall hardwoods, sunny kitchen, south facing backyard & deck, garage, brand new 2009 High Efficiency furnace and separate basement entrance for optional (non-conforming) suite--this little gem is ready to be sold. Across from the Confederation golf course, this inner city property has tonnes of potential and come spring--you will be glad you have already purchased your project & investment... ";
                AllListings.Add(new House("2411", "4th Street", Quadrant.NW, "Brentwood,", "Calgary, ", "Alberta", 479000, 2, 2, desc, image, images, 1200, Features.Garage, ListingType.Residential, BuildingType.House));

                for (int j = 0; j < 4; j++)
                {
                    if (j == 0)
                        image = Path.Combine(SourceOfImages, imageTag);
                    imageTag = "3" + "_" + j.ToString();
                    images.Add(Path.Combine(SourceOfImages, imageTag));
                }
                image = Path.Combine(SourceOfImages, "house3_1.jpg");
                desc = "Come check out the Confederation by Birchwood's CityLife Homes...this brand new two storey infill on this quiet street across from Aberhart High School's playing field. This 1729sqft home offers 9ft ceilings & rich hardwood floors, which beautifully complement the contemporary earthtone decor. The stylish maple island kitchen has stainless steel appliances, a raised eating bar, quartz counters & glass backsplash. The open concept living/dining room is ideal for entertaining, & there's also a family room with a big South-facing window. The master bedroom is just a terrific size & enjoys a large walk-in closet & a luxurious ensuite with double sinks. Two more bedrooms share another full bathroom, plus there's a handy 2nd floor laundry room too. Complete with an inviting front porch, an unfinished lower level with 9ft ceilings & a sunny South backyard, this fantastic property will be the perfect home for your family...available for quick possession, minutes to the University, LRT & shopping. ";
                AllListings.Add(new House("5149", "Country Hills Blvd", Quadrant.NW, "Country Hills,", "Calgary, ", "Alberta", 599000, 2, 2, desc, image, images, 1500, Features.Garage, ListingType.Residential, BuildingType.House));


                for (int j = 0; j < 4; j++)
                {
                    if (j == 0)
                        image = Path.Combine(SourceOfImages, imageTag);
                    imageTag = "4" + "_" + j.ToString();
                    images.Add(Path.Combine(SourceOfImages, imageTag));
                }
                image = Path.Combine(SourceOfImages, "house4_1.jpg");
                desc = "One of the Best locations in Eau Claire..Walk to work and enjoy the local shops and restaurants. This Luxury Town House backs directly onto the River and Eau Claire path way. Recently professionally Renovated by Paul Lavoie interior Design Firm. Enjoy the beautiful unobstructed views from all 3 levels. This home was featured on the cover of Opulence Magazine after its first set of renovations. Outstanding floor plan with 10ft ceilings, gourmet Kitchen with massive center granite island great for entertaining. Spacious dining room + Living room combination. Enjoy the Open flame fireplace and central A/C. Master suite offers large walk in closet and huge deck overlooking the River. 2nd bedroom offers its own ensuite bath. Upper level offer plenty of options from home office to 3rd bedroom or home theatre area. Private access to your 2 underground parking stalls. Private gated front access. Enjoy the summer on your front patio with plenty of space to entertain. Units like this rarely come along. Call today";
                AllListings.Add(new House("400", "Eau Claire AV", Quadrant.SW, "Eau Claire,", "Calgary, ", "Alberta", 1125000, 2, 2, desc, image, images, 3000, Features.Pool, ListingType.Residential, BuildingType.Townhouse));


                for (int j = 0; j < 4; j++)
                {
                    if (j == 0)
                        image = Path.Combine(SourceOfImages, imageTag);
                    imageTag = "5" + "_" + j.ToString();
                    images.Add(Path.Combine(SourceOfImages, imageTag));
                }
                image = Path.Combine(SourceOfImages, "house5_1.jpg");
                desc = "Great opportunity for a seldom available main floor unit with a lovely south facing patio. Open floorplan featuring 2 bedrooms & den (den features a glass partition wall allowing lots of light to pass through and patio doors to a second private patio space. Recently installed expresso hardwood flooring, fresh paint, gas fireplace, new cabinet finishings, in suite laundry, lots of visitor parking (indoors) and a great amenities package including tennis courts, work out centre, social/party room and hot tub, make this a very convenient place to live. Not to mention the incredible proximity to the downtown, Bow river pathway system, Eau Claire market & the Kensington neighborhood just north across the river.. Vacant and ready to move into...Extremely well priced - compare the other similar ground floor unit ( gutted/renovated) listed for $300,000 more... THIS IS VALUE... ";
                AllListings.Add(new House("602", "1A STREET", Quadrant.SW, "Eau Claire,", "Calgary, ", "Alberta", 419000, 2, 2, desc, image, images, 2500, Features.AC | Features.Garage, ListingType.Residential, BuildingType.Apartment));


            //AllListings.Add(new House("300", "Meredith Road", "NE", "Brentwood,", "Calgary, ", "Alberta", "$400,000", "2", "2", desc));
            //AllListings.Add(new House("2508", "Centre Street", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2", desc));
            //AllListings.Add(new House("12", "Cheyenne Crescent", "NW", "Brentwood,", "Calgary, ", "Alberta", "$200,000", "2", "2", desc));
            //CurrentPage = Results.Take(5);
            //FavouriteListings = CurrentPage;
        }

        private static DataItem[] prices = new DataItem[] {
            new DataItem { Value=150000, Display="$150,000" },
            new DataItem { Value=200000, Display="$200,000" },
            new DataItem { Value=250000, Display="$250,000" },
            new DataItem { Value=300000, Display="$300,000" },
            new DataItem { Value=350000, Display="$350,000" },
            new DataItem { Value=400000, Display="$400,000" },
            new DataItem { Value=450000, Display="$450,000" },
            new DataItem { Value=500000, Display="$500,000" },
            new DataItem { Value=600000, Display="$600,000" },
            new DataItem { Value=700000, Display="$700,000" },
            new DataItem { Value=800000, Display="$800,000" },
            new DataItem { Value=900000, Display="$900,000" },
            new DataItem { Value=1000000, Display="$1,000,000" },
            new DataItem { Value=1500000, Display="$1,500,000" },
            new DataItem { Value=2000000, Display="$2,000,000" },
            new DataItem { Value=int.MaxValue, Display="Unlimited" }
        };

        private static DataItem[] buildingTypes = new DataItem[] {
            //new DataItem { Value = (int)BuildingType.NoPreference, Display="No Preference" },
            new DataItem { Value = (int)BuildingType.House, Display="House" },
            new DataItem { Value = (int)BuildingType.Apartment, Display="Apartment" },
            new DataItem { Value = (int)BuildingType.Townhouse, Display="Townhouse" },
            new DataItem { Value = (int)BuildingType.Duplex, Display="Duplex" }
        };

        private static DataItem[] areas = new DataItem[] {
            new DataItem { Value = (int)Quadrant.NW, Display=Quadrant.NW.ToString() },
            new DataItem { Value = (int)Quadrant.SW, Display=Quadrant.SW.ToString() },
            new DataItem { Value = (int)Quadrant.SE, Display=Quadrant.SE.ToString() },
            new DataItem { Value = (int)Quadrant.NE, Display=Quadrant.NE.ToString() }
        };

        private static DataItem[] listingTypes = new DataItem[] {
            new DataItem { Value = (int)ListingType.Residential, Display="Residential" },
            new DataItem { Value = (int)ListingType.Recreational, Display="Recreational" },
            new DataItem { Value = (int)ListingType.Agricultural, Display="Agricultural" },
            new DataItem { Value = (int)ListingType.Commercial, Display="Commercial" },
            new DataItem { Value = (int)ListingType.Land, Display="Land" }
        };

        private static DataItem[] sqFeet = new DataItem[] {
            new DataItem { Value = 300, Display="300" },
            new DataItem { Value=400, Display="400" },
            new DataItem { Value=500, Display="500" },
            new DataItem { Value=600, Display="600" },
            new DataItem { Value=700, Display="700" },
            new DataItem { Value=800, Display="800" },
            new DataItem { Value=900, Display="900" },
            new DataItem { Value=1000, Display="1,000" },
            new DataItem { Value=1200, Display="1,200" },
            new DataItem { Value=1400, Display="1,400" },
            new DataItem { Value=1600, Display="1,600" },
            new DataItem { Value=1800, Display="1,800" },
            new DataItem { Value=2000, Display="2,000" },
            new DataItem { Value=2500, Display="2,000" },
            new DataItem { Value=3000, Display="3,000" },
            new DataItem { Value=3500, Display="3,500" },
            new DataItem { Value=4000, Display="4,000" },
            new DataItem { Value=int.MaxValue, Display="Unlimited" }
        };

        private static DataItem[] bedrooms = new DataItem[] {
            new DataItem { Value=1, Display="1 or more" },
            new DataItem { Value=2, Display="2 or more" },
            new DataItem { Value=3, Display="3 or more" },
            new DataItem { Value=4, Display="4 or more" },
            new DataItem { Value=5, Display="5 or more" },
        };

        private static DataItem[] bathrooms = new DataItem[] {
            new DataItem { Value=1, Display="1 or more" },
            new DataItem { Value=2, Display="2 or more" },
            new DataItem { Value=3, Display="3 or more" },
            new DataItem { Value=4, Display="4 or more" },
            new DataItem { Value=5, Display="5 or more" },
        };

        private static DataItem[] features = new DataItem[] {
            new DataItem { Value=Features.AC, Display=Features.AC.ToString() },
            new DataItem { Value=Features.Fireplace, Display=Features.Fireplace.ToString() },
            new DataItem { Value=Features.Garage, Display=Features.Garage.ToString() },
            new DataItem { Value=Features.Pool, Display=Features.Pool.ToString() }
        };
    }
}
