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
using System.Data.SqlClient;



namespace SpotiFind.BusinessLogic
{
    public class BusinessLogic
    {
        //private static SpotifyWebAPI _spotify;

        private string _apiKey = "AIzaSyBlBngVE6JZlHI649il6Lx3AKtiNolG2-Q";

        private SpotiFindContext db = new SpotiFindContext();
        private string _userId = "danielberkeley";
        private static string _clientId = "e8fa55dbd4f74d68802fb6c67ab04105";
        private static string _clientSecret = "47448d979a7d42419cdbee8f7c2df8d4";
        private static string _refreshToken = "AQAxhwzbuLRQWdgVwuK3LmaKfOGUiAGVDLJhWDqNWMAubcY_wvrQwSKCew6ZhUtm4r2ug8uKWFLckroTjELJkTely_Ck_W2BovjhvEmfkDqRVgpMPrMhD4YDNNhX1agxP_U";
        private static string _state = "XSS";

        //private static string _connectionString = @"Server=tcp:spotifind.database.windows.net,1433;Initial Catalog=locationdb3;Persist Security Info=False;User ID=danieldzhang;Password=Dannyz123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private static string _connectionString = @"Data Source=SEAOTTER\SQLEXPRESS;Initial Catalog=dbspotifind;User ID=ug;Password=ug";

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

        public Location GetClosestLocation(float lat, float lon)
        {
            List<Location> locationList = new List<Location>();
            locationList = GetLocationByLatLong(lat, lon);
            double minDistance = 1000;
            int leastLocationId = locationList[0].Id;
            foreach (Location place in locationList)
            {
                Place currentPlace = GetPlaceById(place.Id);
                double distance = Math.Sqrt(Math.Pow(lat - currentPlace.Latitude, 2) + Math.Pow(lon - currentPlace.Longitude, 2));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    leastLocationId = place.Id;
                }
            }

            return GetLocationById(leastLocationId);
        }

        public List<Location> GetLocationByLatLong(float lat, float lon)
        {
            string sql = "SELECT * FROM[dbspotifind].[dbo].[GeoNames] WHERE ABS(latitude - " + lat + ") < (0.005) AND ABS(longitude - " + lon + ") < (0.005)";
            var list = QueryDB(sql);
            
            List<Location> locationList = new List<Location>();
            foreach (Place place in list)
            {
                var select = db.Locations.FirstOrDefault(l => l.PlaceId == place.Id);

                if (select != null)
                {
                    var location = new Location
                    {
                        Id = select.Id,
                        PlaceId = select.PlaceId,
                        PlaylistId = select.PlaylistId
                    };

                    locationList.Add(location);
                }
                else
                {
                    
                }


            }

            return locationList;





        }

        public List<Place> QueryDB(string sqlString)
        {
            List <Place> list = new List<Place>();
            string connectionString = _connectionString;
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = sqlString;
            connection = new SqlConnection(connectionString);
            string temp_latitude = null;
            string temp_longitude = null;
            float latitude = 0;
            float longitude = 0;
            string name = null;
            string address = null;
            string id = null;
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));

                    id = dataReader.GetValue(0).ToString();

                    name = dataReader.GetValue(1).ToString();
                    temp_latitude = dataReader.GetValue(4).ToString();
                    temp_longitude = dataReader.GetValue(5).ToString();

                    latitude = float.Parse(temp_latitude);
                    longitude = float.Parse(temp_longitude);

                    var place = new Place
                    {
                        Id = id,
                        
                        Name = name,
                        Address = address,

                        Latitude = latitude,
                        Longitude = longitude
                    };

                    list.Add(place);


                }
                dataReader.Close();
                command.Dispose();
                connection.Close();

            }
            catch (Exception ex)
            {
                Console.Write("Can't open connection!");
            }
            

            return list;
        }

        public Place GetPlaceById(int id)
        {
            string apiKey = _apiKey; // Your api key

            Location l = GetLocationById(id);
            var placeId = l.PlaceId;

            string sql = "SELECT * FROM[dbspotifind].[dbo].[GeoNames] WHERE geonameId = " + placeId;


            var list = QueryDB(sql);

            return list[0];
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

        public string PostTrackToLocation(string locationId, string trackId, string accessToken)
        {
            SpotifyWebAPI _spotify = GetSpotifyResponseWithAccessToken(accessToken);
            var select = db.Locations.FirstOrDefault(l => l.PlaceId == locationId);
            var uri = "spotify:track:" + trackId;
            if (select != null)
            {
                var playlistId = select.PlaylistId;
                ErrorResponse response = _spotify.AddPlaylistTrack(_userId, playlistId, uri);
                if (!response.HasError())
                {
                    return "Success";
                }
                else
                {
                    return response.Error.Message;
                }
            }
            else
            {
                return "Location not found";
            }
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