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

        public static FakeDbConnection SetUpForExecuteNonQuery(this FakeDbConnection fakeDbConnection, int rowsAffected, int timesConsecutively=1)
        {
            for (int i = 1; i <= timesConsecutively; i++)
            {
                fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteNonQuery(rowsAffected));
            }
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpForQueryScalar<T>(this FakeDbConnection fakeDbConnection, T dataToReturn)
        {
            fakeDbConnection.SetUpForQuery(new[] {dataToReturn});
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpForQuery<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteSingleColumnQuery(dataToReturn) );
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpForQuery<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn, string[] propertyNames)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteQuery(dataToReturn,propertyNames) );
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpForQuery(this FakeDbConnection fakeDbConnection, IEnumerable<object[]> dataToReturn, params string[] propertyNames)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, propertyNames));
            return fakeDbConnection;
        }

        public static FakeDbConnection SetUpForQuery(this FakeDbConnection fakeDbConnection, IEnumerable<object[]> dataToReturn, FakeDbResultSet.MetaData[] metaData)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, metaData));
            return fakeDbConnection;
        }
    }
}