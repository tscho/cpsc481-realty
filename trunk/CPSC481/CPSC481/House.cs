using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPSC481
{

    class House
    {
        public string houseNumber { get; set; }
        public string street { get; set; }
        public string quandrant { get; set; }
        public string address { get; set; }
        public string price {get; set;}
        public string area { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string source { get; set;}

        public House(String houseNumber, String street, String quandrant, String area, String city, String province, String price)
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
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", 
                      address, price, source);
        }

    }
}
