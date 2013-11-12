using System;
using System.Collections.Generic;

namespace TestBase.FakeDb
{
    public static class FakeDbConnectionExtensions
    {
        /// <summary>
        /// Queue a command to the fake database connection which when invoked 
        /// with ExecuteQuery() will return a 1x1 result set containing the given scalar value
        /// </summary>
        public static FakeDbConnection SetUpForExecuteScalar(this FakeDbConnection fakeDbConnection, object scalar)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.forExecuteScalarResult(scalar));
            return fakeDbConnection;
        }

        /// <summary>
        /// Queue a command to the fake database connection which when invoked with
        /// ExecuteNonQuery will return the setup rowsAffected
        /// </summary>
        /// <param name="timesConsecutively">If a command is to be called repeated, use this to set up for multiple consecutive calls</param>
        public static FakeDbConnection SetUpForExecuteNonQuery(this FakeDbConnection fakeDbConnection, int rowsAffected, int timesConsecutively = 1)
        {
            for (int i = 1; i <= timesConsecutively; i++)
            {
                fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteNonQuery(rowsAffected));
            }
            return fakeDbConnection;
        }

        /// <summary>
        /// Queue a command to the fake database connection which when invoked with
        /// ExecuteQuery will return a 1x1 result set containing the dataToReturn.
        /// </summary>
        public static FakeDbConnection SetUpForQueryScalar<T>(this FakeDbConnection fakeDbConnection, T dataToReturn)
        {
            fakeDbConnection.SetUpForQuery(new[] {dataToReturn});
            return fakeDbConnection;
        }

        /// <summary>
        /// Queue a command to the fake database connection which when invoked with
        /// ExecuteQuery will return a result set with rows containing the rows of dataToReturn.
        /// The returned rows will contain one column for each public property of the dataToReturn.
        /// Execution will fail if T does not have a public constructor with either no parameters or else 
        /// one correspondingly-named parameter for each public property
        /// 
        /// Use this overload to setup database queries with columns which exactly correspond to some class defined in your project
        /// 
        /// Example
        /// <code>
        /// fakeDbConnection.SetUpForQuery&lt;KeyValuePair&lt;int,string&gt&gt;(new []{new KeyValuePair&lt;int,string&gt(1,"1")});
        /// </code>
        /// will return a single row with two un-named columns, the first integer, the second string.
        /// When used with Dapper, will simply rehydrate the given dataToReturn
        /// </summary>
        /// <param name="dataToReturn">The given object will be reflected for its public properties.</param>
        public static FakeDbConnection SetUpForQuery<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteSingleColumnQuery(dataToReturn) );
            return fakeDbConnection;
        }

        /// <summary>
        /// Queue a command to the fake database connection which when invoked with
        /// ExecuteQuery will return a result set with rows containing the rows of dataToReturn.
        /// The returned rows will contain one column for each public property of the dataToReturn named in <param name="propertyNames"></param>
        /// Execution will fail if T does not have a public constructor with either no parameters or else 
        /// one correspondingly-named parameter for each public property listed.
        /// 
        /// Use this overload to setup database queries with columns which exactly correspond to some class defined in your project
        /// 
        /// Example
        /// <code>
        /// fakeDbConnection.SetUpForQuery&lt;MyEntity&gt;(new []{new MyEntity(1,"Entity 1")},new[]{"Id","Description");
        /// </code>
        /// will return a single row with columns named Id &amp; Description, containing the values {1, "Entity1"}.
        /// When used with Dapper, will simply rehydrate the given dataToReturn
        /// </summary>
        /// <param name="propertyNames">The columns returned will be just those public properties of typeof(dataToReturn) named in this list</param>
        public static FakeDbConnection SetUpForQuery<T>(this FakeDbConnection fakeDbConnection, IEnumerable<T> dataToReturn, string[] propertyNames)
        {
            fakeDbConnection.QueueCommand( FakeDbCommand.ForExecuteQuery(dataToReturn,propertyNames) );
            return fakeDbConnection;
        }

        /// <summary>
        /// Queue a command to the fake database connection which when invoked with
        /// ExecuteQuery will return a result set with rows containing the rows of dataToReturn.
        /// The returned rows will contain as many columns as the given object[] arrays are long.
        /// The columns will be named from the left with the names given in <param name="columnNames"></param>. If not enough names are
        /// passed in, the remaining columns will be nameless.
        /// 
        /// Use this overload to setup database queries with columns which do not exactly correspond to some class defined in your project
        /// </summary>
        /// <param name="columnNames">The columns will be named from the left with the names given. If fewer names are passed in than
        /// there are columns in <paramref name="dataToReturn"/> then the remaining columns will be nameless.</param>
        /// <exception cref="InvalidOperationException">If dataToReturn is null, and no column names are given, then an exception will be thrown because 
        /// metadata cannot be inferred from no data and no hints.</exception>
        public static FakeDbConnection SetUpForQuery(this FakeDbConnection fakeDbConnection, IEnumerable<object[]> dataToReturn, params string[] columnNames)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, columnNames));
            return fakeDbConnection;
        }

        /// <summary>
        /// Queue a command to the fake database connection which when invoked with
        /// ExecuteQuery will return a result set with rows containing the rows of dataToReturn.
        /// The returned data will have columns as per the given metaData.
        /// 
        /// Use this overload to setup database queries with columns which do not exactly correspond to some class defined in your project
        /// </summary>
        /// <param name="metaData">The returned result will have exactly the given metaData. If the width of <paramref name="dataToReturn"/>
        /// does not match the width of the metaDate, anonomous data will be returned. </param>
        /// <exception cref="InvalidOperationException">If dataToReturn is null, and no column names are given, then an exception will be thrown because 
        /// metadata cannot be inferred from no data and no hints.</exception>
        public static FakeDbConnection SetUpForQuery(this FakeDbConnection fakeDbConnection, IEnumerable<object[]> dataToReturn, FakeDbResultSet.MetaData[] metaData)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, metaData));
            return fakeDbConnection;
        }
    }
}