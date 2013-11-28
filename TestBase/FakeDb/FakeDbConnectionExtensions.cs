using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace TestBase.FakeDb
{
    public static class FakeDbConnectionExtensions
    {
        /// <summary>
        /// Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return the <see cref="scalar"/> when 
        /// <see cref="DbCommand.ExecuteScalar"/> is called on it.
        /// 
        /// The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForExecuteScalar(this FakeDbConnection fakeDbConnection, object scalar)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.forExecuteScalarResult(scalar));
            return fakeDbConnection;
        }

        /// <summary>
        /// Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return <see cref="rowsAffected"/> when 
        /// <see cref="DbCommand.ExecuteNonQuery"/> is called on it.
        /// 
        /// The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForExecuteNonQuery(this FakeDbConnection fakeDbConnection, int rowsAffected, int timesConsecutively = 1)
        {
            for (int i = 1; i <= timesConsecutively; i++)
            {
                fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteNonQuery(rowsAffected));
            }
            return fakeDbConnection;
        }

        /// <summary>
        /// Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return <see cref="dataToReturn"/> when 
        /// either the protected <see cref="DbCommand.ExecuteDbDataReader"/> or the public 
        /// <see cref="DbCommand.ExecuteReader()"/> is called on it.
        /// 
        /// This overload will return a result set with 1 column and 1 row.
        /// 
        /// The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQueryScalar<T>(this FakeDbConnection fakeDbConnection, T dataToReturn)
        {
            fakeDbConnection.SetUpForQuery(new[] {dataToReturn});
            return fakeDbConnection;
        }

        /// <summary>
        /// Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return <see cref="dataToReturn"/> when 
        /// either the protected <see cref="DbCommand.ExecuteDbDataReader"/> or the public 
        /// <see cref="DbCommand.ExecuteReader()"/> is called on it.
        /// 
        /// This overload will return a result set with just 1 column and <see cref="dataToReturn"/>.Count() rows.
        /// 
        /// The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteSingleColumnQuery(dataToReturn) );
            return fakeDbConnection;
        }

        /// <summary>
        /// Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return <see cref="dataToReturn"/> when 
        /// either the protected <see cref="DbCommand.ExecuteDbDataReader"/> or the public 
        /// <see cref="DbCommand.ExecuteReader()"/> is called on it.
        /// 
        /// This overload will return a result set with 1 column per string in <see cref="propertyNames"/> 
        ///  and <see cref="dataToReturn"/>.Count() rows.
        /// 
        /// The first row of <see cref="dataToReturn"/> will be examined to determine the DataType for each of the <see cref="propertyNames"/>.
        /// 
        /// The columns will be populated from the correspondingly-named properties of <see cref="dataToReturn"/>.
        /// 
        /// Property names such as "ComplexProperty.NestedPropertyValue" will return the corresponding 'nested' property of 
        /// each item in <see cref="dataToReturn"/>. 
        /// 
        /// The properties must be public instance properties and must be both readable and writeable.
        /// 
        /// The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <param name="propertyNames">An array of property names which will be used to (1) supply metadata for the returned result set
        /// and (2) identify properties on <see cref="dataToReturn"/> whose values will populate the result</param>
        /// <returns>Itself, for chaining</returns>
        /// <exception cref="TargetInvocationException">If the given <see cref="propertyNames"/> cannot be read by reflection from the elements of 
        /// <see cref="dataToReturn"/> then an exception will be thrown immediately.</exception>
        public static FakeDbConnection SetUpForQuery<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn, string[] propertyNames)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteQuery(dataToReturn,propertyNames) );
            return fakeDbConnection;
        }

        /// <summary>
        /// Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return <see cref="dataToReturn"/> when 
        /// either the protected <see cref="DbCommand.ExecuteDbDataReader"/> or the public 
        /// <see cref="DbCommand.ExecuteReader()"/> is called on it.
        /// 
        /// This overload will return a result set with 1 column per string in <see cref="propertyNames"/> 
        ///  and <see cref="dataToReturn"/>.Count() rows.
        /// 
        /// The first row of <see cref="dataToReturn"/> will be examined with GetType() to determine the DataType for each column.
        /// (Nulls will result in a column of type object. To specify more exact metadata, use the overload that excepts 
        /// a <see cref="FakeDbResultSet.MetaData"/> array instead).
        /// 
        /// The columns will be populated left-to-right from elements of each row of <see cref="dataToReturn"/> and will be named 
        /// left-to-right from the given <see cref="propertyNames"/>.
        /// 
        /// Property names such as "ComplexProperty.NestedPropertyValue" should work. The properties must be public instance properties and
        /// must be both readable and writeable.
        /// 
        /// The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <param name="propertyNames">An array of property names which will be used to (1) supply metadata for the returned result set
        /// and (2) identify properties on <see cref="dataToReturn"/> whose values will populate the result</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery(this FakeDbConnection fakeDbConnection, IEnumerable<object[]> dataToReturn, params string[] propertyNames)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, propertyNames));
            return fakeDbConnection;
        }

        /// <summary>
        /// Sets up the FakeDbConnection to return a FakeDbCommand which is itself set up to return <see cref="dataToReturn"/> when 
        /// either the protected <see cref="DbCommand.ExecuteDbDataReader"/> or the public 
        /// <see cref="DbCommand.ExecuteReader()"/> is called on it.
        /// 
        /// This overload will return a result set with 1 column per element in <see cref="metaData"/> 
        /// (which should exactly match <see><cref>dataToReturn.Length</cref></see>)
        /// and <see cref="dataToReturn"/>.Count() rows.
        /// 
        /// The columns will be populated left-to-right from elements of each row of <see cref="dataToReturn"/> and will have 
        /// metadata (column name and .Net Type) assigned left-to-right from the given <see cref="metaData"/> array.
        /// 
        /// The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <param name="propertyNames">An array of property names which will be used to (1) supply metadata for the returned result set
        /// and (2) identify properties on <see cref="dataToReturn"/> whose values will populate the result</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery(this FakeDbConnection fakeDbConnection, IEnumerable<object[]> dataToReturn, FakeDbResultSet.MetaData[] metaData)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, metaData));
            return fakeDbConnection;
        }
    }
}