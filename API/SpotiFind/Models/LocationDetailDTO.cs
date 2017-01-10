using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotiFind.Models
{
    public class LocationDetailDTO
    {
        public int Id { get; set; }
        
        public string PlaylistId { get; set; }
        public string PlaylistName { get; set; }

        public string PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string PlaceAddress { get; set; }
        public double PlaceLatitude { get; set; }
        public double PlaceLongitude { get; set; }

    }
}