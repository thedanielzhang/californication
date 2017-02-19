namespace SpotiFind.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SpotiFind.Models;
    using System.Data.SqlClient;
    using SpotifyAPI.Web; //Base Namespace
    using SpotifyAPI.Web.Auth; //All Authentication-related classes
    using SpotifyAPI.Web.Enums; //Enums
    using SpotifyAPI.Web.Models; //Models for the JSON-responses
    using BusinessLogic;
    using System.Data.Entity.Migrations;
    
    public partial class updating : DbMigration
    {
        private string _apiKey = "AIzaSyBlBngVE6JZlHI649il6Lx3AKtiNolG2-Q";

        private SpotiFindContext db = new SpotiFindContext();
        private string _userId = "danielberkeley";
        private static string _clientId = "e8fa55dbd4f74d68802fb6c67ab04105";
        private static string _clientSecret = "47448d979a7d42419cdbee8f7c2df8d4";
        private static string _refreshToken = "AQAxhwzbuLRQWdgVwuK3LmaKfOGUiAGVDLJhWDqNWMAubcY_wvrQwSKCew6ZhUtm4r2ug8uKWFLckroTjELJkTely_Ck_W2BovjhvEmfkDqRVgpMPrMhD4YDNNhX1agxP_U";
        private static string _state = "XSS";
        public override void Up()
        {
            BusinessLogic businessLogic = new BusinessLogic();
            string accessToken = businessLogic.GetAccessToken();
            SpotifyWebAPI _spotify = businessLogic.GetSpotifyResponseWithAccessToken(accessToken);

            string connectionString = @"Data Source=SEAOTTER\SQLEXPRESS;Initial Catalog=dbspotifind;User ID=ug;Password=ug";
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "SELECT * FROM[dbspotifind].[dbo].[GeoNames] WHERE ABS(latitude - 37.8719) < (0.015) AND ABS(longitude - -122.2585) < (0.015)";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                    FullPlaylist playlist = _spotify.CreatePlaylist(_userId, dataReader.GetValue(1).ToString());
                    db.Locations.AddOrUpdate(
                        p => p.Id,
                        new Location() { PlaceId = dataReader.GetValue(0).ToString(), PlaylistId = playlist.Id }
                    );
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();

            }
            catch (Exception ex)
            {
                Console.Write("Can't open connection!");
            }
        }
        
        public override void Down()
        {
        }
    }
}
