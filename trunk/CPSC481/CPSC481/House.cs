using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPSC481
{

    public class House
    {
        public String houseNumber { get; set; }
        public String street { get; set; }
        public String quandrant { get; set; }
        public String address { get; set; }
        public String price { get; set; }
        public String area { get; set; }
        public String city { get; set; }
        public String province { get; set; }
        public String source { get; set;}
        public String bedrooms { get; set; }
        public String bathrooms { get; set; }

        public House(String houseNumber, String street, String quandrant, String area, String city, String province, String price, String bedrooms, String bathrooms)
        {
            this.address = houseNumber + " " + street + " " + quandrant;
            this.houseNumber = houseNumber;
            this.street = street;
            this.quandrant = quandrant;
            this.area = area;
            this.city = city;
            this.province = province;
            this.price = price;
            this.source = source;
            this.bathrooms = bathrooms;
            this.bedrooms = bedrooms;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", 
                      address, price, source);
        }

    }
}
