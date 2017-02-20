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

    internal sealed class Configuration : DbMigrationsConfiguration<SpotiFind.Models.SpotiFindContext>
    {
        private string _apiKey = "AIzaSyBlBngVE6JZlHI649il6Lx3AKtiNolG2-Q";

        private SpotiFindContext db = new SpotiFindContext();
        private string _userId = "danielberkeley";
        private static string _clientId = "e8fa55dbd4f74d68802fb6c67ab04105";
        private static string _clientSecret = "47448d979a7d42419cdbee8f7c2df8d4";
        private static string _refreshToken = "AQAxhwzbuLRQWdgVwuK3LmaKfOGUiAGVDLJhWDqNWMAubcY_wvrQwSKCew6ZhUtm4r2ug8uKWFLckroTjELJkTely_Ck_W2BovjhvEmfkDqRVgpMPrMhD4YDNNhX1agxP_U";
        private static string _state = "XSS";
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SpotiFind.Models.SpotiFindContext context)
        {

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            /*
            BusinessLogic businessLogic = new BusinessLogic();
            string accessToken = businessLogic.GetAccessToken();
            SpotifyWebAPI _spotify = businessLogic.GetSpotifyResponseWithAccessToken(accessToken);

            string connectionString = @"Data Source=SEAOTTER\SQLEXPRESS;Initial Catalog=dbspotifind;User ID=ug;Password=ug";
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "SELECT * FROM[dbspotifind].[dbo].[GeoNames] WHERE ABS(latitude - 37.4275) < (0.015) AND ABS(longitude - -122.1697) < (0.015)";
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
                    context.Locations.AddOrUpdate(
                        p => p.geonameid,
                        new Location() { PlaceId = dataReader.GetValue(0).ToString(), playlistid = playlist.Id }
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
            */
            

        }
    }
}
