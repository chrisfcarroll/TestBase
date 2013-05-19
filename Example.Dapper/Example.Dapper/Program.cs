using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Example.Dapper
{
    public class Program
    {
        public static readonly string ConnectionStringFromConfig = TryGet.OrDefault(()=>ConfigurationManager.ConnectionStrings["ApplicationData"].ConnectionString);

        private readonly string connectionString;

        public Program(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private IDbConnection OpenConnection()
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        static void Main(string[] args)
        {
            var program = new Program(ConnectionStringFromConfig);
            program.GetSomeData();
            program.SaveSomeData("Some Product");
        }

        public IEnumerable<Product> GetSomeData()
        {
            using (var connection = OpenConnection())
            {
                return connection.Query<Product>("Select Id, Description From Products Where Id=@Id", new {Id = 1});
            }
        }

        public int SaveSomeData(string productDescription)
        {
            using (var conn = OpenConnection())
            {
                return conn.Execute("Insert into Products (Description) Values (@Description)", new {Description = productDescription});
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public static class TryGet
    {
        public static T OrDefault<T>(Func<T> function)
        {
            try{ return function(); } catch(Exception){ return default(T); }
        }
    }

}
