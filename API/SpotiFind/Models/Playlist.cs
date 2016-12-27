using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses

namespace SpotiFind.Models
{
    public class Playlist
    {
        public string PlaylistId { get; set; }
        public Paging<PlaylistTrack> Songs { get; }
    }
}