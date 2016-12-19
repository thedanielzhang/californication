using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpotiFind.Models
{
    public class Location
    {
        public int Id { get; set; }
        [Required]
        public string Place { get; set; }
        public string Playlist { get; set; }
    }
}