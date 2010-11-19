using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Web;

namespace CPSC481
{

    public class House
    {
        public String houseNumber { get; set; }
        public String street { get; set; }
        public String quadrant { get; set; }
        public String address { get; set; }
        public String price { get; set; }
        public String area { get; set; }
        public String city { get; set; }
        public String province { get; set; }
        public String source { get; set;}
        public String bedrooms { get; set; }
        public String bathrooms { get; set; }
        public String detailText { get; set; }
        //public GeocoderLocation LatLng;
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        private LatLngAccurateToTypes _LatLngAccuracy = 0;
       
        public House(String houseNumber, String street, String quandrant, String area, String city, String province, String price, String bedrooms, String bathrooms, string detailText)
        {
            this.address = houseNumber + " " + street + " " + quandrant;
            this.houseNumber = houseNumber;
            this.street = street;
            this.quadrant = quandrant;
            this.area = area;
            this.city = city;
            this.province = province;
            this.price = price;
            this.source = source;
            this.bathrooms = bathrooms;
            this.bedrooms = bedrooms;
            this.detailText = detailText;
            GeoCode();
        }

        
	public LatLngAccurateToTypes LatLngAccuracy
	{
		get { return _LatLngAccuracy; }
	}

	public override string ToString()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append(address);
		sb.Append(", ");
		sb.Append(city);
		sb.Append(", ");
		sb.Append(province);
		sb.Append(" ");
        sb.Append(city);
        sb.Append("{");
        sb.Append(Latitude);
	    sb.Append(",");
        sb.Append(Longitude);
        sb.Append("}");
		return sb.ToString();
	}

	public void GeoCode()
	{
		//get the maps key in the web config file
        string mapskey = "ABQIAAAAwuJcmtEbPbTOVHSybSa56BQqjzYXlVXz0uwSLGElMCZH0WZ4sRRwNXlCZ0os8CPeHow5izbL0WjQvw";
		//setup a streamreader for retrieving data from Google.
		StreamReader sr = null;

		//Check to see if our maps key exists
		if (string.IsNullOrEmpty(mapskey))
		{
			throw new Exception("No valid google maps api key to use for geocoding.  Please add an app key named \"GoogleMapsAPIKey\" to the web.config file.");
		}

		//Create the url string to send our request to googs.
		string url = string.Format("http://maps.google.com/maps/geo?q={0} +{1} +{2}&output=xml&oe=utf8&sensor=false&key={3}", this.address, this.city + ", ", this.province, mapskey);

		//Create a web request client.
		WebClient wc = new WebClient();

		try
		{
			//retrieve our result and put it in a streamreader
			sr = new StreamReader(wc.OpenRead(url));
		}
		catch (Exception ex)
		{
			throw new Exception("An error occured while retrieving GeoCoded results from Google, the error was: " + ex.Message);
		}

		try
		{
			//now lets parse the returned data as an xml
			XmlTextReader xtr = new XmlTextReader(sr);
			bool coordread = false;
			bool accread = false;
			while (xtr.Read())
			{
				xtr.MoveToElement();
				switch (xtr.Name)
				{
					case "AddressDetails": //Check for the address details node
						while (xtr.MoveToNextAttribute())
						{
							switch (xtr.Name)
							{
								case "Accuracy": //move into the accuracy attrib and....
									if (!accread)
									{
										//get the accuracy, convert it to our enum and save it to the record
										this._LatLngAccuracy = (LatLngAccurateToTypes)Convert.ToInt32(xtr.Value);
										accread = true;
									}
									break;
							}
						}
						break;
					case "coordinates": //the coordinates element
						if (!coordread)
						{
							//move into the element value
							xtr.Read();

							//split the coords and then..
							string[] coords = xtr.Value.Split(new char[] { ',' });

							//save the values
							Longitude = coords[0];
							Latitude = coords[1];

							//finally, once this has been done, we don't want the process to run again on the same file
							coordread = true;
						}
						break;
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("An error occured while parsing GeoCoded results from Google, the error was: " + ex.Message);
		}
	}

	public enum LatLngAccurateToTypes : int
	{
		Unknown = 0,
		Country = 1,
		Region = 2,
		SubRegion = 3,
		Town = 4,
		PostCode = 5,
		Street = 6,
		Intersection = 7,
		Address = 8,
		Premises = 9
	}


    }

    [Serializable]
    public class GeocoderLocation
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public GeocoderLocation(string lon, string lat)
        {
            Longitude = lon;
            Latitude = lat;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}", Latitude, Longitude);
        }
    }
}
