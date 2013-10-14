using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Example.Dapper
{
    public class Repository
    {
        private readonly IDbConnection dbConnection;

        public Repository(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public IEnumerable<Product> GetSomeData()
        {
            return dbConnection.Query<Product>("Select Id, Description From Products Where Id=@Id", new { Id = 1 });
        }

        public int SaveSomeData(string productDescription)
        {
            return dbConnection.Execute("Insert into Products (Description) Values (@Description)", new { Description = productDescription });
        }
    }
}
