using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SpotiFind.Models;
using System.Xml.Linq;
using SpotiFind.ExtensionMethods;
using System.Web.Script.Serialization;



namespace SpotiFind.BusinessLogic
{
    public class BusinessLogic
    {
        private string _apiKey = "AIzaSyBlBngVE6JZlHI649il6Lx3AKtiNolG2-Q";
        private SpotiFindContext db = new SpotiFindContext();

        public List<Location> GetLocations()
        {
            //create a Location Model List
            var locations = new List<Location>();

            //for each location in database
            foreach (var location in db.Locations)
            {
                locations.Add(new Location
                {
                    Id = location.Id,
                    PlaceId = location.PlaceId,
                    PlaylistId = location.PlaylistId
                });
            }



            return locations;
        }

        public Location GetLocationById(int id)
        {
            var select = db.Locations.FirstOrDefault(l => l.Id == id);

            if (select != null)
            {
                var location = new Location
                                {
                                    Id = select.Id,
                                    PlaceId = select.PlaceId,
                                    PlaylistId = select.PlaylistId
                                };

                return location;
            }
            else
            {
                return null;
            }
            
        }

        public Place GetPlaceById(int id)
        {
            string apiKey = _apiKey; // Your api key

            Location l = GetLocationById(id);
            var placeId = l.PlaceId;

            string url = string.Format(@"https://maps.googleapis.com/maps/api/place/details/json?placeid={0}&key={1}", placeId, apiKey);
            var json = new WebClient().DownloadString(url);
            
            JavaScriptSerializer oJS = new JavaScriptSerializer();
            SpotiFind.Models.APIModels.RootObject oRootObject = new SpotiFind.Models.APIModels.RootObject();
            oRootObject = oJS.Deserialize<SpotiFind.Models.APIModels.RootObject>(json);

            var place = new Place
            {
                Name = oRootObject.result.name,
                Address = oRootObject.result.formatted_address,

                Latitude = oRootObject.result.geometry.location.lat,
                Longitude = oRootObject.result.geometry.location.lng
            };

            return place;
        }

        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}