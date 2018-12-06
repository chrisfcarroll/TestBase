using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace TestBase.AdoNet
{
    public static class FakeDbConnectionExtensions
    {
        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return the <see cref="scalar" />
        ///     when
        ///     <see cref="DbCommand.ExecuteScalar" /> is called on it.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForExecuteScalar(this FakeDbConnection fakeDbConnection, object scalar)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteScalarResult(scalar));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="rowsAffected" /> when
        ///     <see cref="DbCommand.ExecuteNonQuery" /> is called on it.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForExecuteNonQuery(
            this FakeDbConnection fakeDbConnection,
            int                   rowsAffected,
            int                   timesConsecutively = 1)
        {
            for (var i = 1; i <= timesConsecutively; i++)
                fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteNonQuery(rowsAffected));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with 1 column and 1 row.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQueryScalar<T>(this FakeDbConnection fakeDbConnection, T dataToReturn)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteSingleColumnQuery(new[] {dataToReturn}));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection so that subsequent calls to <see cref="SetUpForQuery{T}" /> are interpreted as MARS
        ///     results of a single
        ///     The Queries are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForMarsOn(this FakeDbConnection fakeDbConnection)
        {
            fakeDbConnection.IsQueueingCommandsWithPretendingToBePartOfAsMars = true;
            return fakeDbConnection;
        }

        /// <summary>
        ///     Switches off the <see cref="SetUpForMarsOn" />.
        ///     Sets up the Fake DbConnection so that subsequent calls to <see cref="SetUpForQuery{T}" /> are queued as the results
        ///     to be returned
        ///     as the results of a sequence of commands, not as MARS results of a single command.
        ///     The Queries are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForMarsOff(this FakeDbConnection fakeDbConnection)
        {
            fakeDbConnection.IsQueueingCommandsWithPretendingToBePartOfAsMars = false;
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with a column per public read-writeable property of typeof(T)
        ///     and with <see cref="dataToReturn" />.Count() rows.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery<T>(
            this FakeDbConnection fakeDbConnection,
            IEnumerable<T>        dataToReturn)
        {
            return fakeDbConnection.SetUpForQuery(dataToReturn, typeof(T).GetDbRehydratablePropertyNames().ToArray());
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with a single column named <paramref name="columnName" />
        ///     and with <see cref="dataToReturn" />.Count() rows.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <param name="columnName">The column name used in the metadata of the returned resultset.</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuerySingleColumn<T>(
            this FakeDbConnection fakeDbConnection,
            IEnumerable<T>        dataToReturn,
            string                columnName = "Column1")
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteSingleColumnQuery(dataToReturn, columnName));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with a column per public read-writeable property of typeof(T1)
        ///     followed by a column per public read-writeable property of typeof(T2)
        ///     and with <see cref="dataToReturn" />.Count() rows.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery<T1, T2>(
            this FakeDbConnection      fakeDbConnection,
            IEnumerable<Tuple<T1, T2>> dataToReturn)
        {
            var itemTypes = new[]
                            {
                            new Tuple<Type, PropertyInfo>(typeof(T1), typeof(Tuple<T1, T2>).GetProperty("Item1")),
                            new Tuple<Type, PropertyInfo>(typeof(T2), typeof(Tuple<T1, T2>).GetProperty("Item2"))
                            };
            fakeDbConnection.QueueCommand(FakeDbCommand
                                         .ForExecuteQueryReturningDataFromTuples(dataToReturn, itemTypes));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with a column per public read-writeable property of typeof(T1)
        ///     followed by a column per public read-writeable property of typeof(T2)
        ///     followed by a column per public read-writeable property of typeof(T3)
        ///     and with <see cref="dataToReturn" />.Count() rows.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery<T1, T2, T3>(
            this FakeDbConnection          fakeDbConnection,
            IEnumerable<Tuple<T1, T2, T3>> dataToReturn)
        {
            var itemTypes = new[]
                            {
                            new Tuple<Type, PropertyInfo>(typeof(T1), typeof(Tuple<T1, T2, T3>).GetProperty("Item1")),
                            new Tuple<Type, PropertyInfo>(typeof(T2), typeof(Tuple<T1, T2, T3>).GetProperty("Item2")),
                            new Tuple<Type, PropertyInfo>(typeof(T3), typeof(Tuple<T1, T2, T3>).GetProperty("Item3"))
                            };
            fakeDbConnection.QueueCommand(FakeDbCommand
                                         .ForExecuteQueryReturningDataFromTuples(dataToReturn, itemTypes));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with a column per public read-writeable property of typeof(T1)
        ///     followed by a column per public read-writeable property of typeof(T2)
        ///     followed by a column per public read-writeable property of typeof(T3)
        ///     followed by a column per public read-writeable property of typeof(T4)
        ///     and with <see cref="dataToReturn" />.Count() rows.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery<T1, T2, T3, T4>(
            this FakeDbConnection              fakeDbConnection,
            IEnumerable<Tuple<T1, T2, T3, T4>> dataToReturn)
        {
            var itemTypes = new[]
                            {
                            new Tuple<Type, PropertyInfo>(typeof(T1), typeof(Tuple<T1, T2, T3>).GetProperty("Item1")),
                            new Tuple<Type, PropertyInfo>(typeof(T2), typeof(Tuple<T1, T2, T3>).GetProperty("Item2")),
                            new Tuple<Type, PropertyInfo>(typeof(T3), typeof(Tuple<T1, T2, T3>).GetProperty("Item3")),
                            new Tuple<Type, PropertyInfo>(typeof(T4),
                                                          typeof(Tuple<T1, T2, T3, T4>).GetProperty("Item4"))
                            };
            fakeDbConnection.QueueCommand(FakeDbCommand
                                         .ForExecuteQueryReturningDataFromTuples(dataToReturn, itemTypes));
            return fakeDbConnection;
        }


        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with 1 column per string in <see cref="propertyNames" />
        ///     and <see cref="dataToReturn" />.Count() rows.
        ///     The first row of <see cref="dataToReturn" /> will be examined to determine the DataType for each of the
        ///     <see cref="propertyNames" />.
        ///     The columns will be populated from the correspondingly-named properties of <see cref="dataToReturn" />.
        ///     Property names such as "ComplexProperty.NestedPropertyValue" will return the corresponding 'nested' property of
        ///     each item in <see cref="dataToReturn" />.
        ///     The properties must be public instance properties and must be both readable and writeable.
        ///     Execution will fail if T does not have a public constructor with either no parameters or else
        ///     one correspondingly-named parameter for each public property.
        ///     Use this overload to setup database queries with columns which exactly correspond to some class defined in your
        ///     project
        ///     Example
        ///     <code>
        /// fakeDbConnection.SetUpForQuery&lt;KeyValuePair&lt;int,string&gt&gt;(new []{new KeyValuePair&lt;int,string&gt(1,"1")});
        /// </code>
        ///     will return a single row with two un-named columns, the first integer, the second string.
        ///     When used with Dapper, will simply rehydrate the given dataToReturn
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <param name="propertyNames">
        ///     An array of property names which will be used to (1) supply metadata for the returned result set
        ///     and (2) identify properties on <see cref="dataToReturn" /> whose values will populate the result
        /// </param>
        /// <returns>Itself, for chaining</returns>
        /// <exception cref="TargetInvocationException">
        ///     If the given <see cref="propertyNames" /> cannot be read by reflection from the elements of
        ///     <see cref="dataToReturn" /> then an exception will be thrown immediately.
        /// </exception>
        public static FakeDbConnection SetUpForQuery<T>(
            this FakeDbConnection fakeDbConnection,
            IEnumerable<T>        dataToReturn,
            string[]              propertyNames)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, propertyNames));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the Fake DbConnection to return a FakeDbCommand which is itself set up to return
        ///     <see cref="dataToReturn" /> when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     The returned rows will have
        ///     <see>
        ///         <cref>dataToReturn.Length</cref>
        ///     </see>
        ///     columns. The columns will be
        ///     named from the left with the names given in
        ///     <param name="columnNames"></param>
        ///     . If not enough names are
        ///     passed in, the remaining columns will be nameless.
        ///     Use this overload to setup database queries with columns which do not exactly correspond to some class defined in
        ///     your project
        ///     The first row of <see cref="dataToReturn" /> will be examined with GetType() to determine the DataType for each
        ///     column.
        ///     (Nulls will result in a column of type object. To specify more exact metadata, use the overload that excepts
        ///     a <see cref="FakeDbResultSet.MetaData" /> array instead).
        ///     The columns will be populated left-to-right from elements of each row of <see cref="dataToReturn" /> and will be
        ///     named
        ///     left-to-right from the given <see cref="propertyNames" />.
        ///     Property names such as "ComplexProperty.NestedPropertyValue" should work. The properties must be public instance
        ///     properties and
        ///     must be both readable and writeable.
        ///     Use this overload to setup database queries with columns which exactly correspond to some class defined in your
        ///     project
        ///     Example
        ///     <code>
        /// fakeDbConnection.SetUpForQuery&lt;MyEntity&gt;(new []{new MyEntity(1,"Entity 1")},new[]{"Id","Description");
        /// </code>
        ///     will return a single row with columns named Id &amp; Description, containing the values {1, "Entity1"}.
        ///     When used with Dapper, will simply rehydrate the given dataToReturn.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <param name="propertyNames">
        ///     An array of property names which will be used to (1) supply metadata for the returned result set
        ///     and (2) identify properties on <see cref="dataToReturn" /> whose values will populate the result
        /// </param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery(
            this FakeDbConnection fakeDbConnection,
            IEnumerable<object[]> dataToReturn,
            params string[]       propertyNames)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, propertyNames));
            return fakeDbConnection;
        }

        /// <summary>
        ///     Sets up the FakeDbConnection to return a FakeDbCommand which is itself set up to return <see cref="dataToReturn" />
        ///     when
        ///     either the protected <see cref="DbCommand.ExecuteDbDataReader" /> or the public
        ///     <see cref="DbCommand.ExecuteReader()" /> is called on it.
        ///     This overload will return a result set with 1 column per element in <see cref="metaData" />
        ///     (which should exactly match
        ///     <see>
        ///         <cref>dataToReturn.Length</cref>
        ///     </see>
        ///     )
        ///     and <see cref="dataToReturn" />.Count() rows.
        ///     Use this overload to setup database queries with columns which do not exactly correspond to some class defined in
        ///     your project
        ///     The columns will be populated left-to-right from elements of each row of <see cref="dataToReturn" /> and will have
        ///     metadata (column name and .Net Type) assigned left-to-right from the given <see cref="metaData" /> array.
        ///     The FakeDbCommands are (in the current version) queued and must be invoked in the order they were setup.
        /// </summary>
        /// <param name="dataToReturn">A scalar data item</param>
        /// <param name="propertyNames">
        ///     An array of property names which will be used to (1) supply metadata for the returned result set
        ///     and (2) identify properties on <see cref="dataToReturn" /> whose values will populate the result
        /// </param>
        /// <returns>Itself, for chaining</returns>
        public static FakeDbConnection SetUpForQuery(
            this FakeDbConnection      fakeDbConnection,
            IEnumerable<object[]>      dataToReturn,
            FakeDbResultSet.MetaData[] metaData)
        {
            fakeDbConnection.QueueCommand(FakeDbCommand.ForExecuteQuery(dataToReturn, metaData));
            return fakeDbConnection;
        }
    }
}
