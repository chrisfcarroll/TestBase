using System.Configuration;
using System.Data.SqlClient;

namespace Example.Dapper
{
    public class Program
    {
        public static readonly string ConnectionStringFromConfig = TryGet.OrDefault(() => ConfigurationManager.ConnectionStrings["ApplicationData"].ConnectionString);

        private static void Main(string[] args)
        {
            var repository = new Repository(new SqlConnection( ConnectionStringFromConfig) );
            repository.GetSomeData();
            repository.SaveSomeData("Some Product");
        }
    }
}