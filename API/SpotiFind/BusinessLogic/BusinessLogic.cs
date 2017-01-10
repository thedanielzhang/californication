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
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using System.Windows.Forms;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Specialized;



namespace SpotiFind.BusinessLogic
{
    public class BusinessLogic
    {
        //private static SpotifyWebAPI _spotify;

        private string _apiKey = "AIzaSyBlBngVE6JZlHI649il6Lx3AKtiNolG2-Q";

        private SpotiFindContext db = new SpotiFindContext();
        private string _userId = "12133684664";
        private static string _clientId = "e8fa55dbd4f74d68802fb6c67ab04105";
        private static string _clientSecret = "47448d979a7d42419cdbee8f7c2df8d4";
        private static string _refreshToken = "AQAxhwzbuLRQWdgVwuK3LmaKfOGUiAGVDLJhWDqNWMAubcY_wvrQwSKCew6ZhUtm4r2ug8uKWFLckroTjELJkTely_Ck_W2BovjhvEmfkDqRVgpMPrMhD4YDNNhX1agxP_U";
        private static string _state = "XSS";

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

        public FullPlaylist GetPlaylistById(int id, string accessToken)
        {
            Location l = GetLocationById(id);
            var playlistId = l.PlaylistId;

            SpotifyWebAPI _spotify = GetSpotifyResponseWithAccessToken(accessToken);
            
            FullPlaylist playlist = _spotify.GetPlaylist(_userId, playlistId);
            return playlist;
        }

        public SearchItem GetSongBySearch(string search, string accessToken)
        {
            SpotifyWebAPI _spotify = GetSpotifyResponseWithAccessToken(accessToken);
            SearchItem result = _spotify.SearchItems(search, SearchType.Track);
            return result;
        }

        //public async void ImplicitGrantAuth()
        //{
        //    WebAPIFactory webApiFactory = new WebAPIFactory(
        //    "http://localhost",
        //    8888,
        //    "e8fa55dbd4f74d68802fb6c67ab04105",
        //    Scope.UserReadPrivate,
        //    TimeSpan.FromSeconds(20)
        //    );

        //    try
        //    {
        //        //This will open the user's browser and returns once
        //        //the user is authorized.
        //        _spotify = await webApiFactory.GetWebApi();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }


        //}

        public async void AuthorizationCodeAuthentication()
        {
            SpotifyWebAPI _spotify;
            ApplicationAuthentication.WebAPIFactory webApiFactory = new ApplicationAuthentication.WebAPIFactory(
            "http://localhost",
            8888,
            "e8fa55dbd4f74d68802fb6c67ab04105",
            Scope.PlaylistModifyPrivate | Scope.UserReadPrivate,
            TimeSpan.FromSeconds(20)
            );

            try
            {
                //This will open the user's browser and returns once
                //the user is authorized.
                _spotify = await webApiFactory.GetWebApi();
                _refreshToken = webApiFactory.refreshToken;
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        public SpotifyWebAPI GetSpotifyResponseExplicitly(string state, Token token)
        {
            if (state != "XSS")
                throw new SpotifyWebApiException($"Wrong state '{state}' received.");

            if (token.Error != null)
                throw new SpotifyWebApiException($"Error: {token.Error}");

            var spotifyWebApi = new SpotifyWebAPI
            {
                UseAuth = true,
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };

            return spotifyWebApi;
        }

        public SpotifyWebAPI GetSpotifyResponseWithAccessToken(string accessToken)
        {
            var spotifyWebApi = new SpotifyWebAPI
            {
                UseAuth = true,
                AccessToken = accessToken,
                TokenType = "Bearer"
            };

            return spotifyWebApi;
        }

        public string GetAccessToken()
        {
            Token token = RefreshToken(_refreshToken, _clientSecret);
            string accessToken = token.AccessToken;
            return accessToken;
        }

        public Token RefreshToken(string refreshToken, string clientSecret)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers.Add("Authorization",
                    "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_clientId + ":" + clientSecret)));
                NameValueCollection col = new NameValueCollection
                {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", refreshToken}
                };

                string response;
                try
                {
                    byte[] data = wc.UploadValues("https://accounts.spotify.com/api/token", "POST", col);
                    response = Encoding.UTF8.GetString(data);
                }
                catch (WebException e)
                {
                    using (StreamReader reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        response = reader.ReadToEnd();
                    }
                }
                return JsonConvert.DeserializeObject<Token>(response);
            }
        }


    }
}