using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpotiFind.Models
{
    public class Location
    {
        [Key]
        public int geonameid { get; set; }
        [Required]
        
        public string playlistid { get; set; }
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}