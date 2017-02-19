using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SpotiFind.BusinessLogic;
using SpotiFind.Models;

namespace SpotiFind.DatabaseSeeding
{
    public class BerkeleyInit
    {
        
        public static void main(String[] args)
        {
            SpotiFindContext spotifindDb = new SpotiFindContext();
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
                    Console.Write(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
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
    }
}