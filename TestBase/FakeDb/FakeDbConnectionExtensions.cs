using System.Collections.Generic;

namespace TestBase.FakeDb
{
    public static class FakeDbConnectionExtensions
    {
        public static FakeDbConnection SetUpForExecuteScalar(this FakeDbConnection fakeDbConnection, object scalar)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.forExecuteScalarResult(scalar));
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpForExecuteNonQuery(this FakeDbConnection fakeDbConnection, int rowsAffected)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteNonQuery(rowsAffected));
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpFor<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteQuery(dataToReturn) );
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpFor<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn, string[] propertyNames)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteQuery(dataToReturn,propertyNames) );
            return fakeDbConnection;
        }
    }
}