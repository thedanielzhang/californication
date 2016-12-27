using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoogleMapsApi;

namespace SpotiFind.Models
{
    public class Place
    {
        public string Name { get; set; }
        public string Address { get; set; }
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }


    }
}