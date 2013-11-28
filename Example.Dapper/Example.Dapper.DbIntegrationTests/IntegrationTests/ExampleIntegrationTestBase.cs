using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase;

namespace Example.Dapper.Tests.IntegrationTests
{
    public class ExampleIntegrationTestBase : IntegrationTestBaseForSqlDb<Repository>
    {
        protected static string ConnectionString = "Server=.;Trusted_Connection=True;Database=IntegrationTestsForExampleDapper";
        // ReSharper disable InconsistentNaming
        // TODO: testbase parameter matching should be supple enough for resharper/fxcop naming rules
        protected static IDbConnection dbConnection = new SqlConnection(ConnectionString);
        // ReSharper restore InconsistentNaming

        protected static string[] DatabaseSetupCommands = new[]
                                    {
                                        "Use tempdb",
                                        "If DB_ID('IntegrationTestsForExampleDapper') is not null " +
                                        "Begin Drop Database IntegrationTestsForExampleDapper WaitFor Delay '00:00:02.000' End",
                                        "Create Database IntegrationTestsForExampleDapper",
                                        "Use IntegrationTestsForExampleDapper",
                                        "Create Table Products ( Id int not null identity, Description nvarchar(200) not null)",
                                    };
        protected static string[] DatabaseTeardownCommands = new[]
                                    {
                                        "Use tempdb",
                                        "Drop Database IntegrationTestsForExampleDapper",
                                    };
        [ClassInitialize]
        public static void TryCreateDb()
        {
            try
            {
                RunDbCommands(DatabaseSetupCommands);
            }
            catch (Exception e)
            {
                TryDropDbAndDisposeConnection();
                throw new ApplicationException("Exception was thrown when trying to initialise DB", e);
            }
        }

        [ClassCleanup]
        public static void TryDropDbAndDisposeConnection()
        {
            try
            {
                dbConnection.Close();
                dbConnection.Dispose();
                RunDbCommands(DatabaseTeardownCommands);
            }
            catch (Exception e)
            {
                Console.WriteLine("TestCleanup TryDropExampleDapperTestsDb failed with exception: ", e);
            }
        }
    }

    public class IntegrationTestBaseForSqlDb<T>: TestBase<T> where T : class
    {
        protected const string connectionStringForInitializeAndCleanup = "Server=.;Trusted_Connection=True;";

        protected static void RunDbCommands(IEnumerable<string> commands)
        {
            using (var conn = new SqlConnection(connectionStringForInitializeAndCleanup))
            {
                conn.Open();
                foreach (var cmdString in commands)
                {
                    using (var cmd = new SqlCommand(cmdString, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}