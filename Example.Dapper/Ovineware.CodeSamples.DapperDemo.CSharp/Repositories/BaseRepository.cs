using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using Dapper;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Repositories
{
    public abstract class BaseRepository
    {
        protected static void SetIdentity<T>(IDbConnection connection, Action<T> setId)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Id").Single();
            T newId = (T)identity.Id;
            setId(newId);
        }

        protected static IDbConnection OpenConnection()
        {
            IDbConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString);
            connection.Open();
            return connection;
        }
    }
}